import { Box, LinearProgress, Stack, Typography } from "@mui/material";
import React, { FC } from "react";
import { useTranslation } from "react-i18next";
import { useFormat } from "../../hooks";
import { StorageUsedProps } from "./types";

export const StorageUsed: FC<StorageUsedProps> = ({
  total,
  used,
  decimals = 2,
}) => {
  const { t } = useTranslation(undefined, {
    keyPrefix: "components.StorageUsed",
  });
  const { formatBytes } = useFormat();
  const percent = Number(((used / total) * 100).toFixed(2));

  const color = percent < 85 ? "primary" : percent < 95 ? "warning" : "error";

  return (
    <Box>
      <Stack
        mb={1}
        spacing={1}
        direction="row"
        alignItems="center"
        justifyContent="space-between"
        color="text.primary"
      >
        <Typography variant="caption">
          {t("used", {
            used: formatBytes(used, decimals),
            total: formatBytes(total, decimals),
          })}
        </Typography>

        <Typography variant="caption">{t("percent", { percent })}</Typography>
      </Stack>

      <Box width="100%">
        <LinearProgress variant="determinate" value={percent} color={color} />
      </Box>
    </Box>
  );
};
