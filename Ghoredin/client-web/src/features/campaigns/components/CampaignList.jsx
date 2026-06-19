import { useState, useEffect } from "react";

import { getMyCampaigns, createCampaign } from "../api/campaignsApi";

import "./CampaignList.css"

function CampaignList() {
    const [campaigns, setCampaigns] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    // Formulář
    const [name, setName] = useState("");
    const [gameSystemId, setGameSystemId] = useState("dnd5e");
    const [maxPlayers, setMaxPlayers] = useState("");

    const loadCampaigns = async () => {
        setError("");

        try {
            const data = await getMyCampaigns();

            setCampaigns(data);
        }
        catch (error) {
            setError("Nepodařilo se načíst dobrodružství: " + error.message);
        }
        finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadCampaigns();
    }, []);

    const handleCreate = async () => {
        setError("");

        if (!name.trim()) {
            setError("Zadej název dobrodružství.");
        }

        try {
            await createCampaign({
                name: name,
                gameSystemId: gameSystemId,
                maxPlayers: maxPlayers === "" ? null : Number(maxPlayers)
            });

            setName("");
            setMaxPlayers("");
            await loadCampaigns();
        }
        catch (error) {
            setError("Nepodařilo se vytvořit dobrodružství: " + error.message);
        }
    };

    if (loading) {
        return <p>Načítání kampaní...</p>;
    }

    return (
        <div className="campaign-list">
            <h2>Moje dobrodružství</h2>

            {
                error &&
                <p className="campaing-list__error">{error}</p>
            }

            {
                campaigns.length === 0
                ? (
                    <p className="campaign-list__empty">Zatím nejsi v žádné kampani.</p>
                )
                : (
                    <ul className="campaign-list__items">
                        {
                            campaigns.map((c) => (
                                <li key={c.id} className="campaign-card">
                                    <div className="campaign-card__main">
                                        <span className="campaign-card__name">{c.name}</span>
                                        <span className="campaign-card__system">{c.gameSystemId}</span>
                                    </div>

                                    <span className="campaign-card__players">
                                        {c.playerCount}
                                        {c.maxPlayers != null ? ` / ${c.maxPlayers}` : ""} hráčů
                                    </span>
                                </li>
                            ))
                        }
                    </ul>
                )
            }

            <div className="campaign-form">
                <h3 className="campaign-form__title">Nové dobrodružství</h3>

                <input 
                    className="campaign-form__input"
                    type="text"
                    placeholder="Název dobrodružství"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />

                <select
                    className="campaign-form__input"
                    value={gameSystemId}
                    onChange={(e) => setGameSystemId(e.target.value)}
                >
                    <option value={dnd5e}>D&D 5e</option>
                    <option value={hrdinove-fantasy}>Hrdinové Fantasy</option>
                </select>

                <input 
                    className="campaign-form__input"
                    type="number"
                    min="1"
                    placeholder="Max. hráčů (prázdné = bez limitu)"
                    value={maxPlayers}
                    onChange={(e) => setMaxPlayers(e.target.value)}
                />

                <button className="campaign-form__button" onClick={handleCreate}>
                    Vytvoř dobrodružství
                </button>
            </div>
        </div>
    );
}

export default CampaignList;