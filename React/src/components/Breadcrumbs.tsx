import { Breadcrumb, theme } from "antd";
import type { ItemType } from "antd/es/breadcrumb/Breadcrumb";
import { useLocation } from "react-router-dom";
import { articlesUrl, coursesUrl, exercisesUrl } from "../helpers/UrlHelper";

type BreadcrumbsProps = {
  customItems?: ItemType[];
};

const routeNameMap: Record<string, string> = {
  "": "Главная",
  [articlesUrl]: "Статьи",
  [exercisesUrl]: "Упражнения",
  [coursesUrl]: "Курсы",
  donate: "Донат",
  offer: "Оферта",
  subscribe: "Подписка",
  "payment-result": "Результат оплаты",
  subscription: "Подписка",
  history: "История",
  "privacy-policy": "Политика конфиденциальности",
};

const Breadcrumbs: React.FC<BreadcrumbsProps> = ({ customItems }) => {
  const location = useLocation();
  const { token } = theme.useToken();

  if (customItems) {
    return <Breadcrumb style={{ padding: `${token.paddingContentVerticalLG}px 0` }} items={customItems} />;
  }

  const pathSegments = location.pathname.split("/").filter(Boolean);

  if (pathSegments.length === 0) {
    return null;
  }

  const items: ItemType[] = [
    {
      title: "Главная",
      href: "/",
    },
  ];

  let currentPath = "";

  pathSegments.forEach((segment, index) => {
    currentPath += `/${segment}`;

    if (routeNameMap[segment]) {
      const isLast = index === pathSegments.length - 1;

      items.push({
        title: routeNameMap[segment],
        href: isLast ? undefined : currentPath,
      });
    } else {
      if (index === pathSegments.length - 1 && pathSegments.length > 1) {
        items.push({
          title: segment,
        });
      }
    }
  });

  if (items.length === 1) {
    return null;
  }

  return <Breadcrumb style={{ padding: `${token.paddingContentVerticalLG}px 0` }} items={items} />;
};

export default Breadcrumbs;
