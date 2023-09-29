import React, { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { RouterProvider } from "react-router-dom";
import { Router } from "./routes/Router";
import { ThemeProvider } from "@mui/material";
import { Theme } from "./Theme";
import CssBaseline from "@mui/material/CssBaseline";
import "./i18n";

const root = createRoot(document.getElementById("root"));

root.render(
  <StrictMode>
    <ThemeProvider theme={Theme}>
      <CssBaseline />

      <RouterProvider router={Router} />
    </ThemeProvider>
  </StrictMode>
);
