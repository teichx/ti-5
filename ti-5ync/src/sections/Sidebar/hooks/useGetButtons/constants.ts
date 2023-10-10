import DescriptionIcon from "@mui/icons-material/Description";
import PeopleIcon from "@mui/icons-material/People";
import { SidebarButtonDefinition } from "./types";
import ImageIcon from "@mui/icons-material/Image";
import FavoriteIcon from "@mui/icons-material/Favorite";
import WatchLaterIcon from "@mui/icons-material/WatchLater";

export const ButtonsDefinition: readonly SidebarButtonDefinition[] = [
  {
    translationKey: "allFiles",
    icon: DescriptionIcon,
    visible: true,
    href: "/",
  },
  {
    translationKey: "shared",
    icon: PeopleIcon,
    visible: true,
    href: "/shared",
  },
  {
    translationKey: "images",
    icon: ImageIcon,
    visible: true,
    href: "/images",
  },
  {
    translationKey: "recent",
    icon: WatchLaterIcon,
    visible: true,
    href: "/recent",
  },
  {
    translationKey: "favorites",
    icon: FavoriteIcon,
    visible: true,
    href: "/favorites",
  },
];
