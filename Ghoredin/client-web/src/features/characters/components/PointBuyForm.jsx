import { useState } from "react";

import { createCharacterInCampaign } from "../api/charactersApi"

import "./AbilityForm.css"

const ABILITIES = [
    { key: "strength", label: "Síla" },
    { key: "dexterity", label: "Obratnost" },
    { key: "constitution", label: "Odolnost" },
    { key: "intelligence", label: "Inteligence" },
    { key: "wisdom", label: "Moudrost" },
    { key: "charisma", label: "Charisma" }
];

const POINT_COST = { 8: 0, 9: 1, 10: 2, 11: 3, 12: 4, 13: 5, 14: 7, 15: 9 };
const BUDGET = 27;
const MIN = 8;
const MAX = 15;

export default function PointBuyForm({ campaignId, onCreated }) {
    const [name, setName] = useState("");

    const [scores, setScores] = useState({
        strength: 8, dexterity: 8, constitution: 8,
        intelligence: 8, wisdom: 8, charisma:8
    });

    const [error, setError] = useState("");
    const [saving, setSaving] = useState(false);

    const spent = Object.values(scores).reduce((sum, v) => sum + POINT_COST[v], 0);
    const remaining = BUDGET - spent;

    const changeScore = (key, delta) => {
        setScores((prev) => {
            const newValue = prev[key] + delta;

            if (newValue < MIN || newValue > MAX)
                return prev;

            if (delta > 0) {
                const newSpent = spent - POINT_COST[prev[key]] + POINT_COST[newValue];

                if (newSpent > BUDGET)
                    return prev;
            }

            return { ...prev, [key]: newValue};
        });
    };

    const handleCreate = async () => {
        setError("");

        if (!name.trim()) {
            setError("Zadej jméno postavy.");
            return;
        }

        setSaving(true);

        try {
            await createCharacterInCampaign(campaignId, {
                name,
                sheetData: { abilities: scores}
            });

            if (onCreated)
                onCreated();
        }
        catch (error) {
            setError("Nepodařilo se vytvořit postavu: " + error.message);
            setSaving(false);
        }
    };

    return (
        <div className="ability-form">
            <input 
                className="ability-form__name"
                type="text" 
                placeholder="Jméno postavy"
                value={name}
                onChange={(e) => setName(e.target.value)}
            />

            <div className={`ability-form__budget ${remaining < 0 ? "ability-form__budget--over" : ""}`}>
                Zbývající body: <strong>{remaining}</strong> / {BUDGET}
            </div>

            {
                error &&
                    <p className="ability-form__error">{error}</p>
            }

            <div className="ability-form__list">
                {
                    ABILITIES.map((a) => (
                        <div key={a.key} className="ability-row">
                            <span className="ability-row__label">{a.label}</span>
                            <div className="ability-row__controls">
                                <button
                                    className="ability-row__btn"
                                    onClick={() => changeScore(a.key, -1)}
                                    disabled={scores[a.key] <= MIN}
                                >
                                    -
                                </button>
                                <span className="ability-row__value">{scores[a.key]}</span>
                                <button
                                    className="ability-row__btn"
                                    onClick={() => changeScore(a.key, +1)}
                                    disabled={scores[a.key] >= MAX}
                                >
                                    +
                                </button>
                            </div>
                        </div>
                    ))
                }
            </div>

            <button
                className="ability-form__submit"
                onClick={handleCreate}
                disabled={saving || remaining < 0}
            >
                {saving ? "Vytvářím..." : "Vytvořit postavu"}
            </button>
        </div>
    );
}