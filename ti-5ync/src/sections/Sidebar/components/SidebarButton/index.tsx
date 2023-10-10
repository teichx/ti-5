import React, { FC } from "react";
import { SidebarButtonProps } from "./types";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import { StyledListItemButton } from "./styles";
import { ListItem, ListItemIcon, ListItemText } from "@mui/material";
import { Link } from "react-router-dom";

export const SidebarButton: FC<SidebarButtonProps> = ({
  text,
  href,
  isSelected,
  startIcon: StartIcon,
}) => (
  <ListItem component={Link} to={href} disablePadding>
    <StyledListItemButton selected={isSelected}>
      <ListItemIcon sx={{ minWidth: 40 }}>
        {isSelected ? <ChevronRightIcon /> : <StartIcon />}
      </ListItemIcon>

      <ListItemText primary={text} />
    </StyledListItemButton>
  </ListItem>
);
