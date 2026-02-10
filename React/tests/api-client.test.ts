import { expect, test, vi } from "vitest";
import { BooleanOperationResult, FullRequestParams, HttpResponse, TokensModelOperationResult } from "../src/api/Api";
import { makeRequest } from "../src/api/ApiClient";
import { UserStore } from "../src/stores/UserStore";

test("валидный токен - один запрос с токеном", async () => {
  const mockResponse: BooleanOperationResult = {
    status: "Success",
    data: true,
    error: "",
    validationErrors: {},
  };

  const params: FullRequestParams = { path: "" };
  const requestMock = vi.fn().mockResolvedValue({
    data: mockResponse,
  });

  const store = new UserStore("42", null);
  store.setTokens({ token: "42", refreshToken: "4242" });
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");

  const refreshTokensRequestMock = vi.fn();
  const showErrorMock = vi.fn();

  await makeRequest(params, requestMock, refreshTokensRequestMock, showErrorMock, store, 2);
  expect(requestMock).toHaveBeenCalledTimes(1);
  expect(requestMock).toHaveBeenCalledWith({ path: "", headers: { Authorization: "Bearer 42" } });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(0);
  expect(showErrorMock).toHaveBeenCalledTimes(0);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(0);
  expect(logoutSpy).toHaveBeenCalledTimes(0);
  expect(setTokensSpy).toHaveBeenCalledTimes(0);
});

test("нет токена, нет refresh токена - показать форму логина", async () => {
  const mockResponse: BooleanOperationResult = {
    status: "AuthenticationFailed",
    data: true,
    error: "",
    validationErrors: {},
  };

  const params: FullRequestParams = { path: "" };
  const requestMock = vi.fn().mockResolvedValue({
    data: mockResponse,
  });

  const store = new UserStore("42", null);
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");

  const refreshTokensRequestMock = vi.fn();
  const showErrorMock = vi.fn();

  const promise = makeRequest(params, requestMock, refreshTokensRequestMock, showErrorMock, store, 2);
  await expect(promise).rejects.toThrowError();
  expect(requestMock).toHaveBeenCalledTimes(1);
  expect(requestMock).toHaveBeenCalledWith({ path: "", headers: {} });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(0);
  expect(showErrorMock).toHaveBeenCalledTimes(1);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(1);
  expect(logoutSpy).toHaveBeenCalledTimes(1);
  expect(setTokensSpy).toHaveBeenCalledTimes(0);
});

test("нет токена, невалидный refresh токен - refresh, показать форму логина", async () => {
  const mockResponse: BooleanOperationResult = {
    status: "AuthenticationFailed",
    data: true,
    error: "",
    validationErrors: {},
  };

  const params: FullRequestParams = { path: "" };
  const requestMock = vi.fn().mockResolvedValue({
    data: mockResponse,
  });

  const deviceFingerprint = "42";
  const invalidRefreshToken = "4242";
  const store = new UserStore(deviceFingerprint, invalidRefreshToken);
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");

  const refreshTokensRequestMock = vi.fn().mockResolvedValue({
    data: mockResponse,
  });
  const showErrorMock = vi.fn();

  const promise = makeRequest(params, requestMock, refreshTokensRequestMock, showErrorMock, store, 2);
  await expect(promise).rejects.toThrowError();
  expect(requestMock).toHaveBeenCalledTimes(1);
  expect(requestMock).toHaveBeenCalledWith({ path: "", headers: {} });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(1);
  expect(refreshTokensRequestMock).toHaveBeenCalledWith({
    refreshToken: invalidRefreshToken,
  });
  expect(showErrorMock).toHaveBeenCalledTimes(1);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(1);
  expect(logoutSpy).toHaveBeenCalledTimes(1);
  expect(setTokensSpy).toHaveBeenCalledTimes(0);
});

test("нет токена, валидный refresh токен - refresh, ок", async () => {
  const mockAuthenticationFailedResponse: BooleanOperationResult = {
    status: "AuthenticationFailed",
    data: true,
    error: "",
    validationErrors: {},
  };

  const mockSuccessResponse: BooleanOperationResult = {
    status: "Success",
    data: true,
    error: "",
    validationErrors: {},
  };

  const params: FullRequestParams = { path: "" };
  const requestMock = vi
    .fn()
    .mockResolvedValueOnce({
      data: mockAuthenticationFailedResponse,
    })
    .mockResolvedValueOnce({
      data: mockSuccessResponse,
    });

  const deviceFingerprint = "42";
  const validToken = "42";
  const validRefreshToken = "4242";
  const store = new UserStore(deviceFingerprint, validRefreshToken);
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");

  const refreshResponse: TokensModelOperationResult = {
    status: "Success",
    data: {
      token: validToken,
      refreshToken: validRefreshToken,
    },
    error: "",
    validationErrors: {},
  };
  const refreshTokensRequestMock = vi.fn().mockResolvedValue({
    data: refreshResponse,
  });
  const showErrorMock = vi.fn();

  await makeRequest(params, requestMock, refreshTokensRequestMock, showErrorMock, store, 2);
  expect(requestMock).toHaveBeenCalledTimes(2);
  expect(requestMock.mock.calls).toEqual([
    [{ path: "", headers: {} }],
    [{ path: "", headers: { Authorization: `Bearer ${validToken}` } }],
  ]);
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(1);
  expect(refreshTokensRequestMock).toHaveBeenCalledWith({
    refreshToken: validRefreshToken,
  });
  expect(showErrorMock).toHaveBeenCalledTimes(0);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(0);
  expect(logoutSpy).toHaveBeenCalledTimes(0);
  expect(setTokensSpy).toHaveBeenCalledTimes(1);
  expect(setTokensSpy).toHaveBeenCalledWith({ token: validToken, refreshToken: validRefreshToken });
});

test("невалидный токен, невалидный refresh токен - показать форму логина", async () => {
  const mockAuthenticationFailedResponse: BooleanOperationResult = {
    status: "AuthenticationFailed",
    data: true,
    error: "",
    validationErrors: {},
  };

  const params: FullRequestParams = { path: "" };
  const requestMock = vi.fn().mockResolvedValue({
    data: mockAuthenticationFailedResponse,
  });

  const deviceFingerprint = "42";
  const invalidToken = "42";
  const invalidRefreshToken = "4242";
  const store = new UserStore(deviceFingerprint, invalidRefreshToken);
  store.setTokens({ token: invalidToken, refreshToken: invalidRefreshToken });
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");

  const refreshResponse: TokensModelOperationResult = {
    status: "AuthenticationFailed",
    data: {
      token: "",
      refreshToken: "",
    },
    error: "",
    validationErrors: {},
  };
  const refreshTokensRequestMock = vi.fn().mockResolvedValue({
    data: refreshResponse,
  });
  const showErrorMock = vi.fn();

  const promise = makeRequest(
    params,
    requestMock,
    () =>
      makeRequest({ path: "/api/auth/refresh-tokens" }, requestMock, refreshTokensRequestMock, showErrorMock, store, 2),
    showErrorMock,
    store,
    2,
  );
  await expect(promise).rejects.toThrowError();
  expect(requestMock).toHaveBeenCalledTimes(2);
  expect(requestMock).toHaveBeenCalledWith({ path: "", headers: { Authorization: `Bearer ${invalidToken}` } });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(0);
  expect(showErrorMock).toHaveBeenCalledTimes(1);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(1);
  expect(logoutSpy).toHaveBeenCalledTimes(1);
  expect(setTokensSpy).toHaveBeenCalledTimes(0);
});

test("logout, невалидный токен, невалидный refresh токен - успешный выход", async () => {
  const mockAuthenticationFailedResponse: BooleanOperationResult = {
    status: "AuthenticationFailed",
    data: true,
    error: "",
    validationErrors: {},
  };

  const params: FullRequestParams = { path: "/api/auth/logout" };
  const requestMock = vi.fn().mockResolvedValue({
    data: mockAuthenticationFailedResponse,
  });

  const deviceFingerprint = "42";
  const invalidToken = "42";
  const invalidRefreshToken = "4242";
  const store = new UserStore(deviceFingerprint, invalidRefreshToken);
  store.setTokens({ token: invalidToken, refreshToken: invalidRefreshToken });
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");

  const refreshResponse: TokensModelOperationResult = {
    status: "AuthenticationFailed",
    data: {
      token: "",
      refreshToken: "",
    },
    error: "",
    validationErrors: {},
  };
  const refreshTokensRequestMock = vi.fn().mockResolvedValue({
    data: refreshResponse,
  });
  const showErrorMock = vi.fn();

  await makeRequest(
    params,
    requestMock,
    () =>
      makeRequest({ path: "/api/auth/refresh-tokens" }, requestMock, refreshTokensRequestMock, showErrorMock, store, 2),
    showErrorMock,
    store,
    2,
  );
  expect(requestMock).toHaveBeenCalledTimes(2);
  expect(requestMock).toHaveBeenCalledWith({
    path: "/api/auth/refresh-tokens",
    headers: { Authorization: `Bearer ${invalidToken}` },
  });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(0);
  expect(showErrorMock).toHaveBeenCalledTimes(0);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(0);
  expect(logoutSpy).toHaveBeenCalledTimes(1);
  expect(setTokensSpy).toHaveBeenCalledTimes(0);
});

test("невалидный токен, валидный refresh токен - refresh, ок", async () => {
  const mockAuthenticationFailedResponse: BooleanOperationResult = {
    status: "AuthenticationFailed",
    data: true,
    error: "",
    validationErrors: {},
  };

  const mockSuccessResponse: BooleanOperationResult = {
    status: "Success",
    data: true,
    error: "",
    validationErrors: {},
  };

  const params: FullRequestParams = { path: "" };
  const requestMock = vi
    .fn()
    .mockResolvedValueOnce({
      data: mockAuthenticationFailedResponse,
    })
    .mockResolvedValueOnce({
      data: mockSuccessResponse,
    });

  const deviceFingerprint = "42";
  const validToken = "42";
  const validRefreshToken = "4242";
  const store = new UserStore(deviceFingerprint, validRefreshToken);
  store.setTokens({ token: validToken, refreshToken: validRefreshToken });
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");

  const refreshResponse: TokensModelOperationResult = {
    status: "Success",
    data: {
      token: validToken,
      refreshToken: validRefreshToken,
    },
    error: "",
    validationErrors: {},
  };
  const refreshTokensRequestMock = vi.fn().mockResolvedValue({
    data: refreshResponse,
  });
  const showErrorMock = vi.fn();

  await makeRequest(params, requestMock, refreshTokensRequestMock, showErrorMock, store, 2);
  expect(requestMock).toHaveBeenCalledTimes(2);
  expect(requestMock.mock.calls).toEqual([
    [{ path: "", headers: { Authorization: `Bearer ${validToken}` } }],
    [{ path: "", headers: { Authorization: `Bearer ${validToken}` } }],
  ]);
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(1);
  expect(refreshTokensRequestMock).toHaveBeenCalledWith({
    refreshToken: validRefreshToken,
  });
  expect(showErrorMock).toHaveBeenCalledTimes(0);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(0);
  expect(logoutSpy).toHaveBeenCalledTimes(0);
  expect(setTokensSpy).toHaveBeenCalledTimes(1);
  expect(setTokensSpy).toHaveBeenCalledWith({ token: validToken, refreshToken: validRefreshToken });
});

test("невалидный токен, валидный refresh токен - ошибка, refresh, ошибка, retry, ошибка, показать форму логина", async () => {
  const mockAuthenticationFailedResponse: BooleanOperationResult = {
    status: "AuthenticationFailed",
    data: true,
    error: "",
    validationErrors: {},
  };

  const params: FullRequestParams = { path: "" };
  const res = {
    ok: false,
    status: 500,
    json: () => Promise.resolve({ message: "Internal Server Error" }),
  } as HttpResponse<any, any>;
  const requestMock: (oldParams: FullRequestParams) => Promise<HttpResponse<any, any>> = vi
    .fn()
    .mockResolvedValueOnce({
      data: mockAuthenticationFailedResponse,
    })
    .mockRejectedValueOnce(res)
    .mockRejectedValueOnce(res);

  const deviceFingerprint = "42";
  const invalidToken = "42";
  const invalidRefreshToken = "4242";
  const store = new UserStore(deviceFingerprint, invalidRefreshToken);
  store.setTokens({ token: invalidToken, refreshToken: invalidRefreshToken });
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");

  const refreshResponse: TokensModelOperationResult = {
    status: "AuthenticationFailed",
    data: {
      token: "",
      refreshToken: "",
    },
    error: "",
    validationErrors: {},
  };
  const refreshTokensRequestMock = vi.fn().mockResolvedValue({
    data: refreshResponse,
  });
  const showErrorMock = vi.fn();

  const promise = makeRequest(
    params,
    requestMock,
    () =>
      makeRequest({ path: "/api/auth/refresh-tokens" }, requestMock, refreshTokensRequestMock, showErrorMock, store, 2),
    showErrorMock,
    store,
    2,
  );
  await expect(promise).rejects.toThrowError();
  expect(requestMock).toHaveBeenCalledTimes(3);
  expect(requestMock).toHaveBeenCalledWith({ path: "", headers: { Authorization: `Bearer ${invalidToken}` } });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(0);
  expect(showErrorMock).toHaveBeenCalledTimes(2);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(1);
  expect(logoutSpy).toHaveBeenCalledTimes(1);
  expect(setTokensSpy).toHaveBeenCalledTimes(0);
});

test("невалидный токен, валидный refresh токен - ошибка, refresh, ошибка, retry, ок", async () => {
  const mockAuthenticationFailedResponse: BooleanOperationResult = {
    status: "AuthenticationFailed",
    data: true,
    error: "",
    validationErrors: {},
  };

  const deviceFingerprint = "42";
  const validToken = "42";
  const validRefreshToken = "4242";
  const refreshResponse: TokensModelOperationResult = {
    status: "Success",
    data: {
      token: validToken,
      refreshToken: validRefreshToken,
    },
    error: "",
    validationErrors: {},
  };

  const params: FullRequestParams = { path: "" };
  const res = {
    ok: false,
    status: 500,
    json: () => Promise.resolve({ message: "Internal Server Error" }),
  } as HttpResponse<any, any>;
  const requestMock: (oldParams: FullRequestParams) => Promise<HttpResponse<any, any>> = vi
    .fn()
    .mockResolvedValueOnce({
      data: mockAuthenticationFailedResponse,
    })
    .mockRejectedValueOnce(res)
    .mockResolvedValueOnce({
      data: refreshResponse,
    });
  const store = new UserStore(deviceFingerprint, validRefreshToken);
  store.setTokens({ token: validToken, refreshToken: validRefreshToken });
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");
  const refreshTokensRequestMock = vi.fn().mockResolvedValue({
    data: refreshResponse,
  });
  const showErrorMock = vi.fn();

  const promise = makeRequest(
    params,
    requestMock,
    () =>
      makeRequest({ path: "/api/auth/refresh-tokens" }, requestMock, refreshTokensRequestMock, showErrorMock, store, 2),
    showErrorMock,
    store,
    2,
  );
  await expect(promise).rejects.toThrowError();
  expect(requestMock).toHaveBeenCalledTimes(4);
  expect(requestMock).toHaveBeenCalledWith({ path: "", headers: { Authorization: `Bearer ${validToken}` } });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(0);
  expect(showErrorMock).toHaveBeenCalledTimes(2);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(0);
  expect(logoutSpy).toHaveBeenCalledTimes(0);
  expect(setTokensSpy).toHaveBeenCalledTimes(1);
});
