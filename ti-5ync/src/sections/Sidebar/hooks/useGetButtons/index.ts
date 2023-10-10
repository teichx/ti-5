import { ButtonsDefinition } from "./constants";
import { UseGetButtonsResult } from "./types";
import { useLocation } from "react-router-dom";

export const useGetButtons = (): UseGetButtonsResult => {
  const location = useLocation();

  return {
    buttons: ButtonsDefinition.filter((x) => x.visible).map((x) => ({
      ...x,
      isSelected: x.href === location.pathname,
    })),
  };
};
