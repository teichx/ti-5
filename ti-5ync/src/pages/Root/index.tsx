import { Box, Button, Container } from "@mui/material";
import React from "react";
import { useNavigate } from "react-router-dom";

export const Root = () => {
  const navigate = useNavigate();

  return (
    <Box>
      <Container>
        <Button
          variant="contained"
          size="small"
          onClick={() => navigate("onboarding")}
        >
          Ok
        </Button>
      </Container>
    </Box>
  );
};
