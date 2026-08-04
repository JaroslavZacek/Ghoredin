import "./CharacterCreation.css"

function CharacterCreation({ campaignId, creationMethod, onCreated}) {
    return (
        <div className="character-creation">
            {creationMethod === "PointBuy" && (
                <p>Point buy formulář (doplníme)</p>
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