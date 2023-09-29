import i18n, { use } from "i18next";
import { initReactI18next } from "react-i18next";
import ptBR from "./pt-BR/translations.json";

use(initReactI18next).init({
  resources: {
    "pt-BR": ptBR,
  },
  lng: "pt-BR",
  fallbackLng: "pt-BR",
  ns: ["translations"],
  defaultNS: "translations",
  saveMissing: true,
  saveMissingTo: "all",
  interpolation: {
    escapeValue: false,
  },
});

export default i18n;
