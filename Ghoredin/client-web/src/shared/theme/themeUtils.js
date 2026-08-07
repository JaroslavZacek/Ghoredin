const SYSTEM_THEMES = {
    dnd5e: "fantasy"
};

export function getThemeForGameSystem(gameSystemId) {
    return SYSTEM_THEMES[gameSystemId] ?? "shell";
}