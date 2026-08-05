import PointBuyForm  from "./PointBuyForm";

import "./CharacterCreation.css"

export default function CharacterCreation({ campaignId, creationMethod, onCreated}) {
    return (
        <div className="character-creation">
            {creationMethod === "PointBuy" && (
                <PointBuyForm campaignId={campaignId} onCreated={onCreated} />
            )}
            {creationMethod === "StandardArray" && (
                <p>Standard array formulář (doplníme).</p>
            )}
            {creationMethod === "Roll" && (
                <p>Roll formulář (doplníme)</p>
            )}
            {!["PointBuy", "StandardArray", "Roll"].includes(creationMethod) && (
                <p>Neznámá metoda tvorby: {creationMethod}</p>
            )}
        </div>
    );
}