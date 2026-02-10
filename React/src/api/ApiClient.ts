import { message } from "antd";
import userStore, { UserStore } from "../stores/UserStore";
import { Api, FullRequestParams, HttpResponse, RefreshTokenModel, Status, TokensModelOperationResult } from "./Api";

export const apiClient = new Api();
const initialRequest = apiClient.request; // to prevent infinite loop

apiClient.request = (oldParams) =>
  makeRequest(oldParams, initialRequest, apiClient.api.authRefreshTokens, message.error, userStore, 2);

export async function makeRequest(
  params: FullRequestParams,
  request: (oldParams: FullRequestParams) => Promise<HttpResponse<any, any>>,
  refreshTokensRequest: (data: RefreshTokenModel) => Promise<HttpResponse<TokensModelOperationResult, any>>,
  showError: (error: string) => void,
  userStore: UserStore,
  attemptCount: number,
): Promise<HttpResponse<any, any>> {
  if (attemptCount === 0) {
    throw new Error("Количество попыток исчерпано");
  }

  const headers: Record<string, string> = (params.headers as Record<string, string>) ?? {};

  if (userStore.token) {
    headers.Authorization = `Bearer ${userStore.token}`;
  }

  const parameters = { ...params, headers };

  try {
    const response = await requestWithRetry(parameters, request, attemptCount);
    const status = (response.data as { status: Status })?.status;

    if (status === "NotFound") {
      throw new Error(response.data.error);
    }

    if (status === "AuthenticationFailed" && params.path !== "/api/auth/refresh-tokens") {
      const isRefreshTokenSuccess = await refreshToken(refreshTokensRequest, userStore);
      if (isRefreshTokenSuccess) {
        return await makeRequest(params, request, refreshTokensRequest, showError, userStore, attemptCount - 1);
      } else {
        userStore.logout();
        if (params.path !== "/api/auth/logout") {
          userStore.showLoginForm();
          throw new Error("Ошибка аутентификации");
        }
      }
    }

    if (status === "Error") {
      showError(response.data.error);
    }

    return response;
  } catch (e) {
    if (e instanceof Error) {
      showError(e.message);
    }

    throw e;
  }
}

async function requestWithRetry(
  params: FullRequestParams,
  request: (oldParams: FullRequestParams) => Promise<HttpResponse<any, any>>,
  attemptCount: number,
): Promise<HttpResponse<any, any>> {
  while (attemptCount > 0) {
    try {
      const response = await request(params);
      return response;
    } catch {
      attemptCount -= 1;
    }
  }

  throw new Error("Количество попыток исчерпано");
}

let fetchPromise: Promise<HttpResponse<TokensModelOperationResult, any>> | null;

async function refreshToken(
  authRefreshTokens: (data: RefreshTokenModel) => Promise<HttpResponse<TokensModelOperationResult, any>>,
  userStore: UserStore,
): Promise<boolean> {
  if (!fetchPromise) {
    if (!userStore.refreshToken) {
      return false;
    }

    fetchPromise = authRefreshTokens({
      refreshToken: userStore.refreshToken,
    });
  }

  let result = false;
  try {
    const response = await fetchPromise;
    if (response.data.status === "Success") {
      userStore.setTokens(response.data.data);
      result = true;
    }
  } catch {
  } finally {
    fetchPromise = null;
  }

  return result;
}
