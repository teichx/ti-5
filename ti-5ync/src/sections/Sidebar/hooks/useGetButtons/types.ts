import { FC } from "react";

export type UseGetButtonsResult = {
  buttons: (SidebarButtonDefinition & { isSelected: boolean })[];
};

export type SidebarButtonDefinition = {
  translationKey: string;
  icon: FC;
  visible: boolean;
  href: string;
};
