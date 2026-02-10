import { ClientJS } from "clientjs";
import { makeAutoObservable } from "mobx";
import { SubscriptionInfo, TokensModel, UserInfo } from "../api/Api";
import { apiClient } from "../api/ApiClient";

const refreshTokenKey = "refreshToken" as const;

export class UserStore {
  private _token: string | null;
  private _refreshToken: string | null;
  private readonly _deviceFingerprint: string;
  private _showLoginForm: boolean = false;
  private _userInfo: UserInfo | null;

  get userInfo(): UserInfo | null {
    return this._userInfo;
  }

  get isLogged(): boolean {
    return !!(this._token || this._refreshToken);
  }

  get hasActiveSubscription(): boolean {
    if (this.activeSubscription) {
      return true;
    }
    return false;
  }

  get activeSubscription(): SubscriptionInfo | undefined {
    return this._userInfo?.subscriptions.find((s) => s.isActive);
  }

  get subscriptions(): SubscriptionInfo[] | undefined {
    return this._userInfo?.subscriptions;
  }

  get isLoginFormShow(): boolean {
    return this._showLoginForm;
  }

  get token(): string | null {
    return this._token;
  }

  get refreshToken(): string | null {
    return this._refreshToken;
  }

  get deviceFingerprint(): string {
    return this._deviceFingerprint;
  }

  constructor(deviceFingerprint: string, refreshToken: string | null) {
    this._deviceFingerprint = deviceFingerprint;
    this._token = null;
    this._refreshToken = refreshToken;
    this._userInfo = null;
    makeAutoObservable(this);
  }

  async loadUserInfo() {
    const getUserInfoResult = await apiClient.api.authGetUserInfo();
    this._userInfo = getUserInfoResult.data.data;
  }

  logout() {
    window.localStorage.removeItem(refreshTokenKey);
    this._token = null;
    this._refreshToken = null;
  }

  setTokens(tokens: TokensModel) {
    window.localStorage.setItem(refreshTokenKey, tokens.refreshToken);
    this._token = tokens.token;
    this._refreshToken = tokens.refreshToken;
  }

  showLoginForm() {
    this._showLoginForm = true;
  }

  hideLoginForm() {
    this._showLoginForm = false;
  }
}

const isTest = !!import.meta.env.VITEST;
const userStore: UserStore = isTest
  ? new UserStore("42", null)
  : new UserStore(new ClientJS().getFingerprint().toString(), window.localStorage.getItem(refreshTokenKey));

export default userStore;
