const SYSTEM_THEMES = {
    dnd5e: "fantasy"
};

export default function getThemeForGameSystem(gameSystemId) {
    return SYSTEM_THEMES[gameSystemId] ?? "shell";
}