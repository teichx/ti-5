import { Button } from "@mui/material";
import React from "react";
import { useNavigate } from "react-router-dom";
import { BasePage } from "../BasePage";

export const Root = () => {
  const navigate = useNavigate();

  return (
    <BasePage>
      <Button
        variant="contained"
        size="small"
        onClick={() => navigate("onboarding")}
      >
        Ok
      </Button>
    </BasePage>
  );
};
