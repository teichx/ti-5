import { useTranslation } from "react-i18next";
import { availableSizes } from "./constants";
import { UseFormat } from "./types";

export const useFormat: UseFormat = () => {
  const { t } = useTranslation(undefined, {
    keyPrefix: "hooks.useFormat",
  });

  const formatBytes = (bytes: number, decimals = 2) => {
    if (!+bytes) return "0 Bytes";

    const intervalInRange = 1024;
    const decimalPrecision = decimals < 0 ? 0 : decimals;

    const scale = Math.floor(Math.log(bytes) / Math.log(intervalInRange));
    const value = parseFloat(
      (bytes / Math.pow(intervalInRange, scale)).toFixed(decimalPrecision)
    );

    return t(`formatBytes.${availableSizes[scale]}`, {
      value,
    });
  };

  return {
    formatBytes,
  };
};
