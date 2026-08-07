import { IconSword, IconMap2 } from "@tabler/icons-react";

const THEME_ICONS = {
    fantasy: IconSword
};

export function getIconForTheme(theme) {
    return THEME_ICONS[theme] ?? IconMap2;
}