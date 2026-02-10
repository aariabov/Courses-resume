import { afterEach, expect, test, vi } from "vitest";
import { BooleanOperationResult, FullRequestParams, HttpResponse } from "../src/api/Api";
import { makeRequest } from "../src/api/ApiClient";
import { UserStore } from "../src/stores/UserStore";

afterEach(() => {
  vi.restoreAllMocks();
});

test("анонимный метод, нет токена, успех - один запрос", async () => {
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
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");

  const refreshTokensRequestMock = vi.fn();
  const showErrorMock = vi.fn();

  await makeRequest(params, requestMock, refreshTokensRequestMock, showErrorMock, store, 2);
  expect(requestMock).toHaveBeenCalledTimes(1);
  expect(requestMock).toHaveBeenCalledWith({ path: "", headers: {} });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(0);
  expect(showErrorMock).toHaveBeenCalledTimes(0);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(0);
  expect(logoutSpy).toHaveBeenCalledTimes(0);
  expect(setTokensSpy).toHaveBeenCalledTimes(0);
});

test("анонимный метод, есть токен, успех - один запрос", async () => {
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

test("анонимный метод, NotFound - показать ошибку", async () => {
  const mockResponse: BooleanOperationResult = {
    status: "NotFound",
    data: true,
    error: "NotFound",
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

  await expect(makeRequest(params, requestMock, refreshTokensRequestMock, showErrorMock, store, 2)).rejects.toThrow(
    "NotFound",
  );

  expect(requestMock).toHaveBeenCalledTimes(1);
  expect(requestMock).toHaveBeenCalledWith({ path: "", headers: {} });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(0);
  expect(showErrorMock).toHaveBeenCalledTimes(1);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(0);
  expect(logoutSpy).toHaveBeenCalledTimes(0);
  expect(setTokensSpy).toHaveBeenCalledTimes(0);
});

test("анонимный метод, исключение - показать ошибку", async () => {
  const mockResponse: BooleanOperationResult = {
    status: "Error",
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

  await makeRequest(params, requestMock, refreshTokensRequestMock, showErrorMock, store, 2);
  expect(requestMock).toHaveBeenCalledTimes(1);
  expect(requestMock).toHaveBeenCalledWith({ path: "", headers: {} });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(0);
  expect(showErrorMock).toHaveBeenCalledTimes(1);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(0);
  expect(logoutSpy).toHaveBeenCalledTimes(0);
  expect(setTokensSpy).toHaveBeenCalledTimes(0);
});

test("сервер недоступен, retry, ошибка - показать ошибку", async () => {
  const mockResponse: BooleanOperationResult = {
    status: "Error",
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
  const requestMock: (oldParams: FullRequestParams) => Promise<HttpResponse<any, any>> = vi.fn(() =>
    Promise.reject(res),
  );

  const store = new UserStore("42", null);
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");

  const refreshTokensRequestMock = vi.fn();
  const showErrorMock = vi.fn();

  const promise = makeRequest(params, requestMock, refreshTokensRequestMock, showErrorMock, store, 2);
  await expect(promise).rejects.toThrowError();
  expect(requestMock).toHaveBeenCalledTimes(2);
  expect(requestMock).toHaveBeenCalledWith({ path: "", headers: {} });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(0);
  expect(showErrorMock).toHaveBeenCalledTimes(1);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(0);
  expect(logoutSpy).toHaveBeenCalledTimes(0);
  expect(setTokensSpy).toHaveBeenCalledTimes(0);
});

test("сервер недоступен, 2 retry, ошибка - показать ошибку", async () => {
  const mockResponse: BooleanOperationResult = {
    status: "Error",
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
  const requestMock: (oldParams: FullRequestParams) => Promise<HttpResponse<any, any>> = vi.fn(() =>
    Promise.reject(res),
  );

  const store = new UserStore("42", null);
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");

  const refreshTokensRequestMock = vi.fn();
  const showErrorMock = vi.fn();

  const promise = makeRequest(params, requestMock, refreshTokensRequestMock, showErrorMock, store, 3);
  await expect(promise).rejects.toThrowError();
  expect(requestMock).toHaveBeenCalledTimes(3);
  expect(requestMock).toHaveBeenCalledWith({ path: "", headers: {} });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(0);
  expect(showErrorMock).toHaveBeenCalledTimes(1);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(0);
  expect(logoutSpy).toHaveBeenCalledTimes(0);
  expect(setTokensSpy).toHaveBeenCalledTimes(0);
});

test("сервер недоступен, retry, ок - ок", async () => {
  const mockResponse: BooleanOperationResult = {
    status: "Success",
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
    .mockRejectedValueOnce(res)
    .mockResolvedValueOnce(mockResponse);

  const store = new UserStore("42", null);
  const showLoginFormSpy = vi.spyOn(store, "showLoginForm");
  const logoutSpy = vi.spyOn(store, "logout");
  const setTokensSpy = vi.spyOn(store, "setTokens");

  const refreshTokensRequestMock = vi.fn();
  const showErrorMock = vi.fn();

  await makeRequest(params, requestMock, refreshTokensRequestMock, showErrorMock, store, 2);
  expect(requestMock).toHaveBeenCalledTimes(2);
  expect(requestMock).toHaveBeenCalledWith({ path: "", headers: {} });
  expect(refreshTokensRequestMock).toHaveBeenCalledTimes(0);
  expect(showErrorMock).toHaveBeenCalledTimes(0);
  expect(showLoginFormSpy).toHaveBeenCalledTimes(0);
  expect(logoutSpy).toHaveBeenCalledTimes(0);
  expect(setTokensSpy).toHaveBeenCalledTimes(0);
});
