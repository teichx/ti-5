import { RouteObject } from "react-router-dom";
import { Onboarding, Root } from "../pages";

export const RoutesDefinition: RouteObject[] = [
  {
    path: "/",
    Component: Root,
  },
  {
    path: "/onboarding",
    Component: Onboarding,
  },
];
