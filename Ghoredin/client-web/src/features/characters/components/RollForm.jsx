import { useState } from "react";

import { startRolledCharacter, rollAbility, completeRolledCharacter } from "../api/charactersApi";

import "./AbilityForm.css"

const ABILITIES = [
    { key: "strength", label: "Síla" },
    { key: "dexterity", label: "Obratnost" },
    { key: "constitution", label: "Odolnost" },
    { key: "intelligence", label: "Inteligence" },
    { key: "wisdom", label: "Moudrost" },
    { key: "charisma", label: "Charisma" }
];

export default function RollForm({ campaignId, existingCharacter, onCreated }) {
    const [character, setCharacter] = useState(existingCharacter);
    const [name, setName] = useState("");

    const [error, setError] = useState("");

    const [rollingKey, setRollingKey] = useState(null);
    const [starting, setStarting] = useState(false);
    const [completing, setCompleting] = useState(false);

    // Postava ještě neexistuje. Založení postavy jen s jménem
    const handleStart = async () => {
        setError("");
        if (!name.trim()) {
            setError("Zadej jméno postavy.");
            return;
        }

        setStarting(true);
        try {
            const created = await startRolledCharacter(campaignId, name);
            setCharacter(created);
        }
        catch (error) {
            setError("Nepodařilo se začít tvorbu: " + error.message);
        }
        finally {
            setStarting(false);
        }
    };

    // Hod na konkrétní atribut
    const handleRoll = async (key) => {
        setError("");
        setRollingKey(key);

        try {
            const updated = await rollAbility(character.id, key);
            setCharacter(updated);
        }
        catch (error) {
            setError("Nepodařilo se hodit: " + error.message);
        }
        finally {
            setRollingKey(null);
        }
    };

    // Dokončení, když už jsou všechny atributy hozené
    const handleComplete = async () => {
        setError("");
        setCompleting(true);

        try {
            await completeRolledCharacter(character.id);
            if (onCreated)
                onCreated();
        }
        catch (error) {
            setError("Nepodařilo se dokončit postavu: " + error.message);
            setCompleting(false);
        }
    };

    if (!character) {
        return (
            <div className="ability-form">
                <input 
                    className="ability-form__name"
                    type="text" 
                    placeholder="Jméno postavy"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />

                {
                    error &&
                        <p className="ability-form__error">{error}</p>
                }

                <button
                    className="ability-form__submit"
                    onClick={handleStart}
                    disabled={starting}
                >
                    {starting ? "Zakládám..." : "Začít házet"}
                </button>

            </div>
        );
    }

    const abilities = character.sheetData.abilities;
    const allRolled = ABILITIES.every((a) => abilities[a.key] > 0);

    return (
        <div className="ability-form">
            <p className="ability-form__name-display">{character.name}</p>

            {
                error && 
                    <p className="ability-form__error">{error}</p>
            }

            <div className="ability-form__list">
                {
                    ABILITIES.map((a) => {
                        const value = abilities[a.key];
                        const rolled = value > 0;

                        return (
                            <div key={a.key} className="ability-row">
                                <span className="ability-row__label">{a.label}</span>

                                {
                                    rolled ? (
                                        <span className="ability-row__value">{value}</span>
                                    ) : (
                                        <button
                                            className="ability-row__roll-btn"
                                            onClick={() => handleRoll(a.key)}
                                            disabled={rollingKey === a.key}
                                        >
                                            {rollingKey === a.key ? "Házím..." : "Hodit"}
                                        </button>
                                    )
                                }
                            </div>
                        );
                    })
                }
            </div>

            <button
                className="ability-form__submit"
                onClick={handleComplete}
                disabled={!allRolled || completing}
            >
                {completing ? "Dokončuji..." : "Dokončit postavu"}
            </button>
        </div>
    );
}