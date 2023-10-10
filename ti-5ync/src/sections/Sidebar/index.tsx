import { Box, Button, Divider, List } from "@mui/material";
import React from "react";
import { SidebarButton } from "./components";

import { useTranslation } from "react-i18next";
import { useGetButtons } from "./hooks";
import { StorageUsed, Logo } from "../../components";

export const Sidebar = () => {
  const { t } = useTranslation(undefined, {
    keyPrefix: "sidebar.buttons",
  });
  const { buttons } = useGetButtons();

  return (
    <Box
      width="100%"
      height="100%"
      display="flex"
      flexDirection="column"
      justifyContent="space-between"
    >
      <Box>
        <Logo.IconWithText />

        <Box my={2}>
          <Divider />
        </Box>

        <List>
          {buttons.map((x) => (
            <SidebarButton
              key={x.translationKey}
              text={t(x.translationKey)}
              href={x.href}
              startIcon={x.icon}
              isSelected={x.isSelected}
            />
          ))}
        </List>
      </Box>

      <Box>
        <Box mb={2}>
          <Divider />
        </Box>

        <Box my={2}>
          <StorageUsed used={395000000000} total={512000000000} decimals={2} />
        </Box>

        <Button fullWidth variant="contained">
          Foo
        </Button>
      </Box>
    </Box>
  );
};
