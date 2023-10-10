import { SvgIcon, styled } from "@mui/material";
import { keyframes, css } from "@emotion/react";
import { LogoIconProps } from "./types";
import { FC } from "react";

const animation = keyframes`
  100% {
    stroke-dashoffset: -80;
  }
`;

export const animatedCss = css`
  g: {
    stroke-dasharray: "70 10";
    animation-name: ${animation};
    animation-duration: "6s";
    animation-play-state: "running";
    animation-timing-function: "linear";
    animation-iteration-count: "infinite";
  }
`;

export const StyledSvgIcon = styled<FC<LogoIconProps>>(SvgIcon)(
  ({ width, height, theme }) => ({
    width: width || "40px",
    height: height || "40px",
    g: {
      fill: "none",
      strokeWidth: "2",
      strokeLinejoin: "round",
      strokeDasharray: "0 0",
      strokeDashoffset: "0",
      stroke: theme.palette.primary.main,
    },
  })
);
