import { Stack, Typography } from "@mui/material";
import { LogoIcon } from "./LogoIcon";
import React from "react";

export const LogoIconWithText = () => (
  <Stack direction="row" alignItems="center">
    <LogoIcon />

    <Typography ml={1}>TI-5ync</Typography>
  </Stack>
);
