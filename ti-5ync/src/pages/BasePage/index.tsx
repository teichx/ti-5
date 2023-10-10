import { Box } from "@mui/material";
import React, { FC, PropsWithChildren } from "react";
import { Sidebar } from "../../sections";

export const BasePage: FC<PropsWithChildren> = ({ children }) => (
  <Box
    p={2}
    height="100vh"
    display="flex"
    flexWrap="nowrap"
    alignItems="stretch"
    bgcolor="common.white"
  >
    <Box mr={2} pt={3} width="100%" maxWidth={240}>
      <Sidebar />
    </Box>

    <Box p={3} width="100%" borderRadius={6} bgcolor="background.paper">
      {children}
    </Box>
  </Box>
);
