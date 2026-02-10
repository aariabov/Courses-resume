import { TestStatusView } from "../api/Api";

export const nameof = <T>(key: keyof T): keyof T => key;

export function delay(ms: number) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

export function randomString(): string {
  return Math.random().toString().substring(2);
}

export function formatDate(dateStr: string): string {
  const date = parseDate(dateStr);
  return date ? date.toLocaleDateString("ru-RU") : "";
}

export function formatDateTime(dateStr: string): string {
  const date = parseDate(dateStr);
  return date ? date.toLocaleString("ru-RU") : "";
}

export function getStatusText(status: TestStatusView): string {
  if (status === "Success") {
    return "Успешно";
  } else if (status === "Fail") {
    return "Неправильно";
  } else {
    return "Ошибка";
  }
}

export function parseDate(str: string): Date | null {
  const date = new Date(str);
  return isNaN(date.getTime()) ? null : date;
}

export function pluralize(count: number, forms: [one: string, few: string, many: string]): string {
  const mod10 = count % 10;
  const mod100 = count % 100;

  if (mod10 === 1 && mod100 !== 11) return forms[0];
  if (mod10 >= 2 && mod10 <= 4 && (mod100 < 12 || mod100 > 14)) return forms[1];
  return forms[2];
}

export const pluralizeDays = (count: number) => pluralize(count, ["день", "дня", "дней"]);
