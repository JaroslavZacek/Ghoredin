import { useState, useEffect } from "react";

import { getMyCharacters } from "../api/charactersApi";

import "./CharacterList.css";

function CharacterList () {
    
    const [characters, setCharacters] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

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
                    <p className="character-list__empty">Zatím nemáš žádné postavy. Postavu si vytvoříš uvnitř dobrodružství.</p>
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

        </div>
    );
}

export default CharacterList;