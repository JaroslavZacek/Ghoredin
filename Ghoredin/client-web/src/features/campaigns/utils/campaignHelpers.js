export function creationMethodLabel(method) {
    switch (method) {
        case "PointBuy": return "Nákup bodů";
        case "StandardArray": return "Pevná sada";
        case "Roll": return "Házení kostkami";
        default: return method;
    }
}

export function isCharacterComplete(character) {
    if (!character || !character.sheetData)
        return false;

    return character.sheetData.creationComplete === true;
}