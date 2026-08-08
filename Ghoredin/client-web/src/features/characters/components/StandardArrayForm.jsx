import { useState } from "react";
import { IconChevronDown } from "@tabler/icons-react";

import { createCharacterInCampaign } from "../api/charactersApi";

import "./AbilityForm.css";

const ABILITIES = [
    { key: "strength", label: "Síla" },
    { key: "dexterity", label: "Obratnost" },
    { key: "constitution", label: "Odolnost" },
    { key: "intelligence", label: "Inteligence" },
    { key: "wisdom", label: "Moudrost" },
    { key: "charisma", label: "Charisma" }
];

const STANDARD_ARRAY = [15, 14, 13, 12, 10, 8];

export default function StandardArrayFrom({ campaignId, onCreated }) {
    const [name, setName] = useState("");

    const [assignments, setAssignments] = useState({
        strength: null, dexterity: null, constitution: null,
        intelligence: null, wisdom: null, charisma: null
    });

    const [error, setError] = useState("");
    const [saving, setSaving] = useState(false);

    const usedValues = Object.values(assignments).filter((v) => v !== null);
    const allAssigned = usedValues.length === ABILITIES.length;

    const availableFor = (currentValue) => {
        const options = STANDARD_ARRAY.filter((v) => !usedValues.includes(v));

        if (currentValue !== null)
            options.push(currentValue);

        return options.sort((a, b) => b - a);
    };

    const handleChange = (key, value) => {
        const numValue = value === "" ? null : Number(value);
        setAssignments((prev) => ({ ...prev, [key]: numValue}));
    };

    const handleCreate = async () => {
        setError("");

        if (!name.trim()) {
            setError("Zadej jméno postavy");
            return;
        }

        if (!allAssigned) {
            setError("Přiřaď všech šest hodnot.");
            return;
        }

        setSaving(true);

        try {
            await createCharacterInCampaign(campaignId, {
                name,
                sheetData: { abilities: assignments }
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

            <div className="ability-form_budget">
                Přiřazeno: <strong>{usedValues.length}</strong> / {ABILITIES.length}
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
                            <div className="themed-select-wrap ability-row__select-wrap">
                                <select
                                    className="themed-select"
                                    value={assignments[a.key] ?? ""}
                                    onChange={(e) => handleChange(a.key, e.target.value)}
                                >
                                    <option value="">- vyber -</option>
                                    {availableFor(assignments[a.key]).map((v) => (
                                        <option key={v} value={v}>{v}</option>
                                    ))}
                                </select>
                                <IconChevronDown size={16} />
                            </div> 
                        </div>
                    ))
                }
            </div>

            <button
                className="ability-form__submit"
                onClick={handleCreate}
                disabled={saving || !allAssigned}
            >
                {saving ? "Vytvářím..." : "Vytvoř postavu"}
            </button>
        </div>
    );
}