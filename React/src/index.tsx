import { ConfigProvider } from "antd";
import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import "./App.css";
import AppLayout from "./AppLayout";
import { articlesUrl, coursesUrl, exercisesUrl } from "./helpers/UrlHelper";
import "./index.css";
import ArticlePage from "./pages/ArticlePage";
import ArticlesPage from "./pages/Articles/ArticlesPage";
import ExercisesPage from "./pages/Exercises/ExercisesPage";
import CourseMainPage from "./pages/CourseMainPage/CourseMainPage";
import CoursesPage from "./pages/Courses/CoursesPage";
import LessonPage from "./pages/LessonPage";
import MainPage from "./pages/MainPage/MainPage";
import NotFound from "./pages/NotFound";
import OfferPage from "./pages/offer/OfferPage";
import PaymentResultPage from "./pages/PaymentResult/PaymentResult";
import PrivacyPolicyPage from "./pages/PrivacyPolicy/PrivacyPolicyPage";
import SubscribePage from "./pages/Subscribe/SubscribePage";
import SubscriptionHistoryPage from "./pages/Subscription/pages/SubscriptionHistoryPage";
import SubscriptionPage from "./pages/Subscription/SubscriptionPage";
import reportWebVitals from "./reportWebVitals";
import ExercisePage from "./pages/Exercises/Exercise/ExercisePage";

const root = ReactDOM.createRoot(document.getElementById("root") as HTMLElement);

root.render(
  <React.StrictMode>
    <ConfigProvider
      theme={{
        token: {
          fontSize: 16,
        },
        components: {
          Layout: {
            bodyBg: "white",
          },
          Menu: {
            darkItemSelectedBg: "transparent",
          },
        },
      }}
    >
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<AppLayout />}>
            <Route path="/" element={<MainPage />} />
            <Route path={articlesUrl}>
              <Route path="" element={<ArticlesPage />} />
              <Route path=":articleUrl" element={<ArticlePage />} />
            </Route>
            <Route path={exercisesUrl}>
              <Route path="" element={<ExercisesPage />} />
              <Route path=":exerciseUrl" element={<ExercisePage />} />
            </Route>
            <Route path="donate" element={<h2>donate</h2>} />
            <Route path="offer" element={<OfferPage />} />
            <Route path="subscribe" element={<SubscribePage />} />
            <Route path="payment-result" element={<PaymentResultPage />} />
            <Route path="subscription">
              <Route path="" element={<SubscriptionPage />} />
              <Route path="history" element={<SubscriptionHistoryPage />} />
            </Route>
            <Route path="privacy-policy" element={<PrivacyPolicyPage />} />
            <Route path={coursesUrl}>
              <Route path="" element={<CoursesPage />} />
              <Route path=":courseUrl">
                <Route index={true} element={<CourseMainPage />} />
                <Route path=":stepUrl">
                  <Route index={true} element={<CourseMainPage />} />
                  <Route path=":lessonUrl" element={<LessonPage />} />
                </Route>
              </Route>
            </Route>
            <Route path="not-found" element={<NotFound />} />
            <Route path="*" element={<Navigate replace to="/not-found" />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </ConfigProvider>
  </React.StrictMode>,
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
