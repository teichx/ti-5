import { FC } from "react";

export type SidebarButtonProps = {
  text: string;
  href: string;
  isSelected?: boolean;
  startIcon: FC;
};
