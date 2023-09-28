import { Box, Button, Container } from "@mui/material";
import React from "react";
import { useNavigate } from "react-router-dom";

export const Onboarding = () => {
  const navigate = useNavigate();

  return (
    <Box>
      <Container>
        <Button variant="contained" size="small" onClick={() => navigate(-1)}>
          Go back
        </Button>
      </Container>
    </Box>
  );
};
