import PointBuyForm  from "./PointBuyForm";
import StandardArrayFrom from "./StandardArrayForm";
import RollForm from "./RollForm";

import "./CharacterCreation.css"

export default function CharacterCreation({ campaignId, creationMethod, existingCharacter, onCreated}) {
    return (
        <div className="character-creation">
            {creationMethod === "PointBuy" && (
                <PointBuyForm campaignId={campaignId} onCreated={onCreated} />
            )}
            {creationMethod === "StandardArray" && (
                <StandardArrayFrom campaignId={campaignId} onCreated={onCreated} />
            )}
            {creationMethod === "Roll" && (
                <RollForm campaignId={campaignId} existingCharacter={existingCharacter} onCreated={onCreated} />
            )}
            {!["PointBuy", "StandardArray", "Roll"].includes(creationMethod) && (
                <p>Neznámá metoda tvorby: {creationMethod}</p>
            )}
        </div>
    );
}