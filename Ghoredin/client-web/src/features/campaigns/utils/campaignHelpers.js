export function creationMethodLabel(method) {
    switch (method) {
        case "PointBuy": return "Nákup bodů";
        case "StandardArray": return "Pevná sada";
        case "Roll": return "Házení kostkami";
        default: return method;
    }
}