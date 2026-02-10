/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

export interface AnalyzeCodeModel {
  /** @minLength 1 */
  code: string;
}

export interface Article {
  /** @format uuid */
  id: string;
  url: string;
  title: string;
  text: string;
  shortText: string;
  /** @format date-time */
  createDate: string;
  /** @format date-time */
  updateDate: string;
}

export interface ArticleOperationResult {
  status: Status;
  error: string;
  data: Article;
  validationErrors: Record<string, string[]>;
}

export interface ArticleRegistryRecord {
  /** @format uuid */
  id: string;
  url: string;
  title: string;
  shortText: string;
  /** @format date-time */
  createDate: string;
  /** @format date-time */
  updateDate: string;
}

export interface ArticleRegistryRecordArrayOperationResult {
  status: Status;
  error: string;
  data: ArticleRegistryRecord[];
  validationErrors: Record<string, string[]>;
}

export interface BooleanOperationResult {
  status: Status;
  error: string;
  data: boolean;
  validationErrors: Record<string, string[]>;
}

export type CharacterSetModificationKind = "Add" | "Remove" | "Replace";

export interface CharacterSetModificationRule {
  kind: CharacterSetModificationKind;
  characters: string[];
}

export interface ClientCodeExecResult {
  result: string | null;
  compileError: string | null;
  runtimeError: string | null;
  error: string | null;
  inProgress: boolean;
}

export interface ClientCodeExecResultOperationResult {
  status: Status;
  error: string;
  data: ClientCodeExecResult;
  validationErrors: Record<string, string[]>;
}

export interface ClientCodeModel {
  /** @minLength 1 */
  code: string;
  inputs: string[];
}

export interface CompletionItem {
  displayText: string;
  displayTextPrefix: string;
  displayTextSuffix: string;
  filterText: string;
  sortText: string;
  inlineDescription: string;
  span: TextSpan;
  properties: Record<string, string>;
  tags: string[];
  rules: CompletionItemRules;
  isComplexTextEdit: boolean;
}

export interface CompletionItemArrayOperationResult {
  status: Status;
  error: string;
  data: CompletionItem[];
  validationErrors: Record<string, string[]>;
}

export interface CompletionItemRules {
  filterCharacterRules: CharacterSetModificationRule[];
  commitCharacterRules: CharacterSetModificationRule[];
  enterKeyRule: EnterKeyRule;
  formatOnCommit: boolean;
  /** @format int32 */
  matchPriority: number;
  selectionBehavior: CompletionItemSelectionBehavior;
}

export type CompletionItemSelectionBehavior = "Default" | "SoftSelection" | "HardSelection";

export interface CompletionRequest {
  code: string;
  /** @format int32 */
  position: number;
}

export interface ConfirmEmailModel {
  /**
   * @format email
   * @minLength 1
   */
  email: string;
  /**
   * @minLength 6
   * @maxLength 6
   */
  code: string;
  /** @minLength 1 */
  deviceFingerprint: string;
}

export interface CourseDto {
  id: string;
  name: string;
  url: string;
  description: string;
  steps: StepDto[];
}

export interface CourseDtoOperationResult {
  status: Status;
  error: string;
  data: CourseDto;
  validationErrors: Record<string, string[]>;
}

export interface CourseParam {
  courseUrl: string;
  lessonUrl: string | null;
}

export interface CourseRegistryRecord {
  /** @format uuid */
  id: string;
  name: string;
  url: string;
  shortDescription: string;
  /** @format int32 */
  stepsCount: number;
  /** @format int32 */
  lessonsCount: number;
}

export interface CourseRegistryRecordArrayOperationResult {
  status: Status;
  error: string;
  data: CourseRegistryRecord[];
  validationErrors: Record<string, string[]>;
}

export interface DiagnosticMsg {
  message: string;
  /** @format int32 */
  startLine: number;
  /** @format int32 */
  startColumn: number;
  /** @format int32 */
  endLine: number;
  /** @format int32 */
  endColumn: number;
  severity: DiagnosticSeverity;
}

export interface DiagnosticMsgArrayOperationResult {
  status: Status;
  error: string;
  data: DiagnosticMsg[];
  validationErrors: Record<string, string[]>;
}

export type DiagnosticSeverity = "Hidden" | "Info" | "Warning" | "Error";

export type EnterKeyRule = "Default" | "Never" | "Always" | "AfterFullyTypedWord";

export interface ExerciseDto {
  /** @format uuid */
  id: string;
  /** @format int32 */
  levelId: number;
  /** @format int32 */
  number: number;
  level: string;
  shortName: string;
  url: string;
  template: string;
  description: string;
  isAccepted: boolean;
  examples: ExerciseExampleDto[];
  functionName: string;
}

export interface ExerciseDtoOperationResult {
  status: Status;
  error: string;
  data: ExerciseDto;
  validationErrors: Record<string, string[]>;
}

export interface ExerciseExampleDto {
  /** @format uuid */
  id: string;
  input: string;
  output: string;
  explanation: string;
}

export interface ExerciseRegistryRecord {
  /** @format uuid */
  id: string;
  /** @format int32 */
  levelId: number;
  /** @format int32 */
  number: number;
  level: string;
  shortName: string;
  url: string;
  isAccepted: boolean;
}

export interface ExerciseRegistryRecordArrayOperationResult {
  status: Status;
  error: string;
  data: ExerciseRegistryRecord[];
  validationErrors: Record<string, string[]>;
}

export interface ForgotPasswordModel {
  /**
   * @format email
   * @minLength 1
   */
  email: string;
}

export interface FunctionTestModel {
  /** @format uuid */
  exerciseId: string;
  /** @minLength 1 */
  code: string;
}

export interface FunctionTestingResultView {
  status: TestStatusView;
  testErrors: TestErrorView[];
  error: string | null;
}

export interface FunctionTestingResultViewOperationResult {
  status: Status;
  error: string;
  data: FunctionTestingResultView;
  validationErrors: Record<string, string[]>;
}

export interface Int32OperationResult {
  status: Status;
  error: string;
  /** @format int32 */
  data: number;
  validationErrors: Record<string, string[]>;
}

export interface LessonDto {
  id: string;
  name: string;
  url: string;
  content: string | null;
  exercise: ExerciseDto;
}

export interface LoginModel {
  /**
   * @format email
   * @minLength 1
   */
  email: string;
  /** @minLength 1 */
  password: string;
  /** @minLength 1 */
  deviceFingerprint: string;
  rememberMe: boolean;
}

export interface LogoutModel {
  /** @minLength 1 */
  refreshToken: string;
}

export interface PaymentModel {
  type: PaymentType;
  /** @minLength 1 */
  returnUrl: string;
}

export type PaymentType = "PerMonth" | "PerYear";

export interface PricesInfo {
  /** @format double */
  perMonth: number;
  /** @format double */
  perYear: number;
}

export interface PricesInfoOperationResult {
  status: Status;
  error: string;
  data: PricesInfo;
  validationErrors: Record<string, string[]>;
}

export interface RefreshTokenModel {
  /** @minLength 1 */
  refreshToken: string;
}

export interface RegisterModel {
  /** @minLength 1 */
  email: string;
  /** @minLength 1 */
  password: string;
  /** @minLength 1 */
  confirmPassword: string;
}

export interface ResetPasswordModel {
  /** @minLength 1 */
  email: string;
  /**
   * @minLength 6
   * @maxLength 6
   */
  code: string;
  /** @minLength 1 */
  password: string;
  /** @minLength 1 */
  confirmPassword: string;
  /** @minLength 1 */
  deviceFingerprint: string;
}

export interface RunExerciseHistoryDto {
  /** @format uuid */
  id: string;
  code: string;
  /** @format date-time */
  date: string;
  functionName: string;
  result: FunctionTestingResultView;
}

export interface RunExerciseHistoryDtoArrayOperationResult {
  status: Status;
  error: string;
  data: RunExerciseHistoryDto[];
  validationErrors: Record<string, string[]>;
}

export type Status =
  | "Success"
  | "Error"
  | "ValidationFailed"
  | "AuthenticationFailed"
  | "AuthorizationFailed"
  | "NotFound";

export interface StepDto {
  id: string;
  name: string;
  description: string;
  shortDescription: string;
  url: string;
  lessons: LessonDto[];
}

export interface StringOperationResult {
  status: Status;
  error: string;
  data: string;
  validationErrors: Record<string, string[]>;
}

export interface SubscriptionInfo {
  /** @format date-time */
  startDate: string;
  /** @format date-time */
  endDate: string;
  /** @format double */
  amount: number;
  type: PaymentType;
  isActive: boolean;
  /** @format int32 */
  daysLeft: number;
  status: SubscriptionStatus;
}

export type SubscriptionStatus = "Expired" | "Active" | "Pending";

export interface TestErrorView {
  error: string;
  parameters: string[];
}

export type TestStatusView = "Success" | "Fail" | "Error";

export interface TextSpan {
  /** @format int32 */
  start: number;
  /** @format int32 */
  end: number;
  /** @format int32 */
  length: number;
  isEmpty: boolean;
}

export interface TokensModel {
  token: string;
  refreshToken: string;
}

export interface TokensModelOperationResult {
  status: Status;
  error: string;
  data: TokensModel;
  validationErrors: Record<string, string[]>;
}

export interface UserInfo {
  email: string;
  subscriptions: SubscriptionInfo[];
}

export interface UserInfoOperationResult {
  status: Status;
  error: string;
  data: UserInfo;
  validationErrors: Record<string, string[]>;
}

export type QueryParamsType = Record<string | number, any>;
export type ResponseFormat = keyof Omit<Body, "body" | "bodyUsed">;

export interface FullRequestParams extends Omit<RequestInit, "body"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseFormat;
  /** request body */
  body?: unknown;
  /** base url */
  baseUrl?: string;
  /** request cancellation token */
  cancelToken?: CancelToken;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> {
  baseUrl?: string;
  baseApiParams?: Omit<RequestParams, "baseUrl" | "cancelToken" | "signal">;
  securityWorker?: (securityData: SecurityDataType | null) => Promise<RequestParams | void> | RequestParams | void;
  customFetch?: typeof fetch;
}

export interface HttpResponse<D extends unknown, E extends unknown = unknown> extends Response {
  data: D;
  error: E;
}

type CancelToken = Symbol | string | number;

export enum ContentType {
  Json = "application/json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public baseUrl: string = "";
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private abortControllers = new Map<CancelToken, AbortController>();
  private customFetch = (...fetchParams: Parameters<typeof fetch>) => fetch(...fetchParams);

  private baseApiParams: RequestParams = {
    credentials: "same-origin",
    headers: {},
    redirect: "follow",
    referrerPolicy: "no-referrer",
  };

  constructor(apiConfig: ApiConfig<SecurityDataType> = {}) {
    Object.assign(this, apiConfig);
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected encodeQueryParam(key: string, value: any) {
    const encodedKey = encodeURIComponent(key);
    return `${encodedKey}=${encodeURIComponent(typeof value === "number" ? value : `${value}`)}`;
  }

  protected addQueryParam(query: QueryParamsType, key: string) {
    return this.encodeQueryParam(key, query[key]);
  }

  protected addArrayQueryParam(query: QueryParamsType, key: string) {
    const value = query[key];
    return value.map((v: any) => this.encodeQueryParam(key, v)).join("&");
  }

  protected toQueryString(rawQuery?: QueryParamsType): string {
    const query = rawQuery || {};
    const keys = Object.keys(query).filter((key) => "undefined" !== typeof query[key]);
    return keys
      .map((key) => (Array.isArray(query[key]) ? this.addArrayQueryParam(query, key) : this.addQueryParam(query, key)))
      .join("&");
  }

  protected addQueryParams(rawQuery?: QueryParamsType): string {
    const queryString = this.toQueryString(rawQuery);
    return queryString ? `?${queryString}` : "";
  }

  private contentFormatters: Record<ContentType, (input: any) => any> = {
    [ContentType.Json]: (input: any) =>
      input !== null && (typeof input === "object" || typeof input === "string") ? JSON.stringify(input) : input,
    [ContentType.Text]: (input: any) => (input !== null && typeof input !== "string" ? JSON.stringify(input) : input),
    [ContentType.FormData]: (input: any) =>
      Object.keys(input || {}).reduce((formData, key) => {
        const property = input[key];
        formData.append(
          key,
          property instanceof Blob
            ? property
            : typeof property === "object" && property !== null
              ? JSON.stringify(property)
              : `${property}`,
        );
        return formData;
      }, new FormData()),
    [ContentType.UrlEncoded]: (input: any) => this.toQueryString(input),
  };

  protected mergeRequestParams(params1: RequestParams, params2?: RequestParams): RequestParams {
    return {
      ...this.baseApiParams,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...(this.baseApiParams.headers || {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected createAbortSignal = (cancelToken: CancelToken): AbortSignal | undefined => {
    if (this.abortControllers.has(cancelToken)) {
      const abortController = this.abortControllers.get(cancelToken);
      if (abortController) {
        return abortController.signal;
      }
      return void 0;
    }

    const abortController = new AbortController();
    this.abortControllers.set(cancelToken, abortController);
    return abortController.signal;
  };

  public abortRequest = (cancelToken: CancelToken) => {
    const abortController = this.abortControllers.get(cancelToken);

    if (abortController) {
      abortController.abort();
      this.abortControllers.delete(cancelToken);
    }
  };

  public request = async <T = any, E = any>({
    body,
    secure,
    path,
    type,
    query,
    format,
    baseUrl,
    cancelToken,
    ...params
  }: FullRequestParams): Promise<HttpResponse<T, E>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.baseApiParams.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const queryString = query && this.toQueryString(query);
    const payloadFormatter = this.contentFormatters[type || ContentType.Json];
    const responseFormat = format || requestParams.format;

    return this.customFetch(`${baseUrl || this.baseUrl || ""}${path}${queryString ? `?${queryString}` : ""}`, {
      ...requestParams,
      headers: {
        ...(requestParams.headers || {}),
        ...(type && type !== ContentType.FormData ? { "Content-Type": type } : {}),
      },
      signal: (cancelToken ? this.createAbortSignal(cancelToken) : requestParams.signal) || null,
      body: typeof body === "undefined" || body === null ? null : payloadFormatter(body),
    }).then(async (response) => {
      const r = response.clone() as HttpResponse<T, E>;
      r.data = null as unknown as T;
      r.error = null as unknown as E;

      const data = !responseFormat
        ? r
        : await response[responseFormat]()
            .then((data) => {
              if (r.ok) {
                r.data = data;
              } else {
                r.error = data;
              }
              return r;
            })
            .catch((e) => {
              r.error = e;
              return r;
            });

      if (cancelToken) {
        this.abortControllers.delete(cancelToken);
      }

      if (!response.ok) throw data;
      return data;
    });
  };
}

/**
 * @title Devpull
 * @version 1.0
 */
export class Api<SecurityDataType extends unknown> extends HttpClient<SecurityDataType> {
  api = {
    /**
     * No description
     *
     * @tags Article
     * @name ArticleGetArticleByUrl
     * @request POST:/api/article/get-article-by-url
     */
    articleGetArticleByUrl: (data: string, params: RequestParams = {}) =>
      this.request<ArticleOperationResult, any>({
        path: `/api/article/get-article-by-url`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Article
     * @name ArticleGetArticles
     * @request POST:/api/article/get-articles
     */
    articleGetArticles: (params: RequestParams = {}) =>
      this.request<ArticleRegistryRecordArrayOperationResult, any>({
        path: `/api/article/get-articles`,
        method: "POST",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags ClientCode
     * @name ClientCodeExecute
     * @request POST:/api/client-code/execute
     */
    clientCodeExecute: (data: ClientCodeModel, params: RequestParams = {}) =>
      this.request<ClientCodeExecResultOperationResult, any>({
        path: `/api/client-code/execute`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags ClientCode
     * @name ClientCodeAnalyze
     * @request POST:/api/client-code/analyze
     */
    clientCodeAnalyze: (data: AnalyzeCodeModel, params: RequestParams = {}) =>
      this.request<DiagnosticMsgArrayOperationResult, any>({
        path: `/api/client-code/analyze`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags ClientCode
     * @name ClientCodeGetCompletion
     * @request POST:/api/client-code/get-completion
     */
    clientCodeGetCompletion: (data: CompletionRequest, params: RequestParams = {}) =>
      this.request<CompletionItemArrayOperationResult, any>({
        path: `/api/client-code/get-completion`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Course
     * @name CourseGetCourses
     * @request POST:/api/course/get-courses
     */
    courseGetCourses: (params: RequestParams = {}) =>
      this.request<CourseRegistryRecordArrayOperationResult, any>({
        path: `/api/course/get-courses`,
        method: "POST",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Course
     * @name CourseGetCourseByUrl
     * @request POST:/api/course/get-course-by-url
     */
    courseGetCourseByUrl: (data: CourseParam, params: RequestParams = {}) =>
      this.request<CourseDtoOperationResult, any>({
        path: `/api/course/get-course-by-url`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Course
     * @name CourseTestAdd
     * @request POST:/api/course/test-add
     */
    courseTestAdd: (params: RequestParams = {}) =>
      this.request<FunctionTestingResultViewOperationResult, any>({
        path: `/api/course/test-add`,
        method: "POST",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Course
     * @name CourseTest
     * @request POST:/api/course/test
     */
    courseTest: (data: FunctionTestModel, params: RequestParams = {}) =>
      this.request<FunctionTestingResultViewOperationResult, any>({
        path: `/api/course/test`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Course
     * @name CourseTest1
     * @request POST:/api/course/test1
     */
    courseTest1: (params: RequestParams = {}) =>
      this.request<BooleanOperationResult, any>({
        path: `/api/course/test1`,
        method: "POST",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Exercise
     * @name ExerciseGetExercises
     * @request POST:/api/exercise/get-exercises
     */
    exerciseGetExercises: (params: RequestParams = {}) =>
      this.request<ExerciseRegistryRecordArrayOperationResult, any>({
        path: `/api/exercise/get-exercises`,
        method: "POST",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Exercise
     * @name ExerciseGetExerciseByUrl
     * @request POST:/api/exercise/get-exercise-by-url
     */
    exerciseGetExerciseByUrl: (data: string, params: RequestParams = {}) =>
      this.request<ExerciseDtoOperationResult, any>({
        path: `/api/exercise/get-exercise-by-url`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Exercise
     * @name ExerciseGetRunExerciseHistory
     * @request POST:/api/exercise/get-run-exercise-history
     */
    exerciseGetRunExerciseHistory: (data: string, params: RequestParams = {}) =>
      this.request<RunExerciseHistoryDtoArrayOperationResult, any>({
        path: `/api/exercise/get-run-exercise-history`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Payment
     * @name PaymentGetPrices
     * @request POST:/api/payment/get-prices
     */
    paymentGetPrices: (params: RequestParams = {}) =>
      this.request<PricesInfoOperationResult, any>({
        path: `/api/payment/get-prices`,
        method: "POST",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Payment
     * @name PaymentCreatePayment
     * @request POST:/api/payment/create-payment
     */
    paymentCreatePayment: (data: PaymentModel, params: RequestParams = {}) =>
      this.request<StringOperationResult, any>({
        path: `/api/payment/create-payment`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Payment
     * @name PaymentIsLastPaymentSuccess
     * @request POST:/api/payment/is-last-payment-success
     */
    paymentIsLastPaymentSuccess: (params: RequestParams = {}) =>
      this.request<BooleanOperationResult, any>({
        path: `/api/payment/is-last-payment-success`,
        method: "POST",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Payment
     * @name PaymentWebhook
     * @request POST:/api/payment/webhook
     */
    paymentWebhook: (params: RequestParams = {}) =>
      this.request<Int32OperationResult, any>({
        path: `/api/payment/webhook`,
        method: "POST",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name AuthGetUserInfo
     * @request POST:/api/auth/get-user-info
     */
    authGetUserInfo: (params: RequestParams = {}) =>
      this.request<UserInfoOperationResult, any>({
        path: `/api/auth/get-user-info`,
        method: "POST",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name AuthRegister
     * @request POST:/api/auth/register
     */
    authRegister: (data: RegisterModel, params: RequestParams = {}) =>
      this.request<BooleanOperationResult, any>({
        path: `/api/auth/register`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name AuthConfirmEmail
     * @request POST:/api/auth/confirm-email
     */
    authConfirmEmail: (data: ConfirmEmailModel, params: RequestParams = {}) =>
      this.request<TokensModelOperationResult, any>({
        path: `/api/auth/confirm-email`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name AuthLogin
     * @request POST:/api/auth/login
     */
    authLogin: (data: LoginModel, params: RequestParams = {}) =>
      this.request<TokensModelOperationResult, any>({
        path: `/api/auth/login`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name AuthLogout
     * @request POST:/api/auth/logout
     */
    authLogout: (data: LogoutModel, params: RequestParams = {}) =>
      this.request<Int32OperationResult, any>({
        path: `/api/auth/logout`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name AuthRefreshTokens
     * @request POST:/api/auth/refresh-tokens
     */
    authRefreshTokens: (data: RefreshTokenModel, params: RequestParams = {}) =>
      this.request<TokensModelOperationResult, any>({
        path: `/api/auth/refresh-tokens`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name AuthForgotPassword
     * @request POST:/api/auth/forgot-password
     */
    authForgotPassword: (data: ForgotPasswordModel, params: RequestParams = {}) =>
      this.request<BooleanOperationResult, any>({
        path: `/api/auth/forgot-password`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name AuthResetPassword
     * @request POST:/api/auth/reset-password
     */
    authResetPassword: (data: ResetPasswordModel, params: RequestParams = {}) =>
      this.request<TokensModelOperationResult, any>({
        path: `/api/auth/reset-password`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags User
     * @name AuthTest
     * @request POST:/api/auth/test
     */
    authTest: (params: RequestParams = {}) =>
      this.request<BooleanOperationResult, any>({
        path: `/api/auth/test`,
        method: "POST",
        format: "json",
        ...params,
      }),
  };
}
