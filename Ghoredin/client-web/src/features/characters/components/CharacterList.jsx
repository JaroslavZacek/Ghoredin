import { useState, useEffect, useSyncExternalStore } from "react";

import { getMyCharacters, createCharacter } from "../api/charactersApi";

import "./CharacterList.css";

function CharacterList () {
    
    const [characters, setCharacters] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    // Formulář
    const [name, setName] = useState("");
    const [gameSystemId, setGameSystemId] = useState("dnd5e");

    // Načtení postav při zobrazení komponenty
    const loadCharacters = async () => {
        setError("");

        try {
            const data = await getMyCharacters();

            setCharacters(data);
        }
        catch (error) {
            setError("Nepodařilo se načíst postavy: " + error.message);
        }
        finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadCharacters();
    }, []);

    /*-----------------------------------------------------------------------------*/
    /*------------------------------Handle funkce----------------------------------*/
    /*-----------------------------------------------------------------------------*/

    const handleCreate = async () => {
        setError("");
        if (!name.trim()) {
            setError("Zadej jméno postavy.");
            return;
        }

        try {
            await createCharacter({
                name: name,
                gameSystemId: gameSystemId,
                sheetData: {}
            });
            
            setName("");
            await loadCharacters();
        }
        catch (error) {
            setError("Nepodařilo se vytvořit postavu: " + error.message);
        }
    };

    if (loading) {
        return <p>Načítání postav...</p>;
    }

    return (
        <div className="character-list">
            <h2 className="character-list__title">Moje postavy</h2>

            {
                error &&
                    <p className="character-list__error">{error}</p>
            }

            {
                characters.length === 0
                ? (
                    <p className="character-list__empty">Zatím nemáš žádné postavy.</p>
                )
                : (
                    <ul className="character-list__items">
                        {
                            characters.map((c) =>(
                                <li key={c.id} className="character-card">
                                    <span className="character-card__name">{c.name}</span>
                                    <span className="chatacter-card__system">{c.gameSystemId}</span>
                                </li>
                            ))
                        }
                    </ul>
                )
            }

            <div className="character-form">
                <h3 className="character-form__title">Nová postava</h3>

                <input 
                    className="character-form__input"
                    type="text"
                    placeholder="Jméno postavy"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />

                <select
                    className="character-form__input"
                    value={gameSystemId}
                    onChange={(e) => setGameSystemId(e.target.value)}
                >
                    <option value="dnd5e">D&D 5E</option>
                    <option value="hrdinove-fantasy">Hrdinové Fantasy</option>
                </select>

                <button className="character-form__button" onClick={handleCreate}>
                    Vytvoř postavu
                </button>
            </div>
        </div>
    );
}

export default CharacterList;