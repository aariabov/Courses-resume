import { ArticleRegistryRecord, CourseDto, CourseRegistryRecord } from "./src/api/Api";
import { GetArticleUrl, GetCourseUrl, GetLessonUrl, GetStepUrl } from "./src/helpers/UrlHelper";

const fs = require("fs");

const baseUrl = "https://devpull.courses";

function getUrlSection(url: string) {
  return `
    <url>
        <loc>${baseUrl}${url}</loc>
    </url>`;
}

async function fetchData(url: string, body: string) {
  const response = await fetch(url, {
    method: "POST",
    headers: {
      accept: "text/plain",
      "Content-Type": "application/json",
    },
    body: body,
  });

  if (!response.ok) {
    console.error("Ошибка при получении статей:", response.status, response.statusText);
    return [];
  }

  const data = await response.json();
  return data.data ?? [];
}

async function generateSitemap() {
  const courses: CourseRegistryRecord[] = await fetchData("http://localhost:5002/api/course/get-courses", "");
  let res = "";
  for (const courseRecord of courses) {
    let body = JSON.stringify({
      courseUrl: courseRecord.url,
      lessonUrl: null,
    });
    let course: CourseDto = await fetchData("http://localhost:5002/api/course/get-course-by-url", body);
    res += getUrlSection(GetCourseUrl(course.url));
    for (const step of course.steps) {
      res += getUrlSection(GetStepUrl(course.url, step.url));
      for (const lesson of step.lessons) {
        res += getUrlSection(GetLessonUrl(course.url, step.url, lesson.url));
      }
    }
  }

  const articles: ArticleRegistryRecord[] = await fetchData("http://localhost:5002/api/article/get-articles", "");
  for (const article of articles) {
    res += getUrlSection(GetArticleUrl(article.url));
  }

  let sitemap = `<?xml version="1.0" encoding="UTF-8"?>
<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">
    <url>
        <loc>${baseUrl}/</loc>
    </url>
    <url>
        <loc>${baseUrl}/courses</loc>
    </url>
    <url>
        <loc>${baseUrl}/articles</loc>
    </url>
    ${res}
</urlset>
`;

  console.log(sitemap);
  fs.writeFileSync("./public/sitemap.xml", sitemap);
}

await generateSitemap();
