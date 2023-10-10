import { ListItemButton, styled } from "@mui/material";

export const StyledListItemButton = styled(ListItemButton)(({ theme }) => ({
  borderRadius: theme.spacing(1),
  color:
    theme.palette.mode === "light"
      ? theme.palette.grey[800]
      : theme.palette.grey[400],
}));
