import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";

import { getCharacter } from "../api/charactersApi";

import "./CharacterSheetPage.css";

const ABILITY_LABELS = {
    strength: "Síla",
    dexterity: "Obratnost",
    constitution: "Odolnost",
    intelligence: "Inteligence",
    wisdom: "Moudrost",
    charisma: "Charisma"
};

function abilityModifier(score) {
    return Math.floor((score - 10) / 2);
}

function formatModifier(mod) {
    return mod >= 0 ? `+${mod}` : `${mod}`;
}

export default function CharacterSheetPage() {
    const navigate = useNavigate();
    
    const { characterId } = useParams();
    const [character, setCharacter] = useState(null);
    
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const load = async () => {
            setError("");

            try {
                const data = await getCharacter(characterId);
                setCharacter(data);
            }
            catch (error) {
                setError("Nepodařilo se zobrazit postavu: " + error.message);
            }
            finally {
                setLoading(false);
            }
        };

        load();
    }, [characterId]);

    if (loading)
        return <p>Načítání postavy...</p>

    if (error)
        return <p className="sheet-page__error">{error}</p>

    if (!character)
        return <p>Postava nenalezena</p>

    const abilities = character.sheetData.abilities || {};
    const hitPoints = character.sheetData.hitPoints || {};
    const complete = character.sheetData.creationComplete === true;

    return (
        <div className="sheet-page">
            <button className="sheet-page__back" onClick={() => navigate(-1)}>
                Zpět
            </button>

            <h2 className="sheet-page__name">{character.name}</h2>
            <p className="sheet-page__system">{character.gameSystemId}</p>

            {
                !complete && (
                    <p className="sheet-page__incomplete">Tvorba postavy ještě není dokončena.</p>
                )
            }

            <section className="sheet-page_section">
                <h3 className="sheet-page__section-title">Atributy</h3>

                <div className="sheet-page__abilities">
                    {
                        Object.entries(ABILITY_LABELS).map(([key, label]) => {
                            const score = abilities[key] ?? 0;
                            return (
                                <div key={key} className="ability-card">
                                    <span className="ability-card__label">{label}</span>
                                    <span className="ability-card__score">{score}</span>
                                    <span className="ability-card__mod">
                                        {score > 0 ? formatModifier(abilityModifier(score)): "—"}
                                    </span>
                                </div>
                            );
                        })
                    }
                </div>
            </section>

            <section className="sheet-page__section">
                <h3 className="sheet-page__section-title">Životy</h3>
                <p className="sheet-page__hp">
                    {hitPoints.current ?? "—"} / {hitPoints.max ?? "—"}
                </p>
            </section>
        </div>
    );
}