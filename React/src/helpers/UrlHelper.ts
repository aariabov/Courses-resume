export const coursesUrl = "courses";
export const articlesUrl = "articles";
export const exercisesUrl = "exercises";

export function GetCourseUrl(courseUrl: string) {
  return `/${coursesUrl}/${courseUrl}`;
}

export function GetStepUrl(courseUrl: string, stepUrl: string) {
  return `/${coursesUrl}/${courseUrl}/${stepUrl}`;
}

export function GetLessonUrl(courseUrl: string, stepUrl: string, lessonUrl: string) {
  return `/${coursesUrl}/${courseUrl}/${stepUrl}/${lessonUrl}`;
}

export function GetArticleUrl(articleUrl: string) {
  return `/${articlesUrl}/${articleUrl}`;
}

export function GetExerciseUrl(exerciseUrl: string) {
  return `/${exercisesUrl}/${exerciseUrl}`;
}
