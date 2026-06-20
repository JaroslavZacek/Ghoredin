import { useState, useEffect } from "react";

import { getAvailableCampaigns, joinCampaign } from "../api/campaignsApi";

import "./CampaignList.css"

function AvailableCampaigns() {
    const [campaigns, setCampaigns] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const loadAvailable = async () => {
        setError("");

        try {
            const data = await getAvailableCampaigns();

            setCampaigns(data);
        }
        catch (error) {
            setError("Nepodařilo se načíst dostupné dobrodružství: " + error.message);
        }
        finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadAvailable();
    }, []);
    
    const handleJoin = async (campaignId) => {
        setError("");

        try {
            await joinCampaign(campaignId);
            
            await loadAvailable();
        }
        catch (error) {
            setError("Nepodařilo se připojit: " + error.message);
        }
    };

    if (loading) {
        return <p>Načítání dostupných kampaní...</p>;
    }

    return (
        <div className="campaign-list">
            <h2 className="campaign-list__title">Dostupné dobrodružství</h2>

            {
                error &&
                    <p className="campaign-list__error">{error}</p>
            }

            {
                campaigns.length === 0
                ? (
                    <p className="campaign-list__empty">Žádné dostupné dobrodružství k připojení</p>
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

                                    <div className="campaign-card__join">
                                        <span className="campaign-card__players">
                                            {c.playerCount}
                                            {c.maxPlayers != null ? ` / ${c.maxPlayers}` : ""} hráčů
                                        </span>

                                        <button
                                            className="campaign-form__button"
                                            onClick={() => handleJoin(c.id)}
                                        >
                                            Připojit se
                                        </button>
                                    </div>
                                </li>
                            ))
                        }
                    </ul>
                )
            }
        </div>
    );

}

export default AvailableCampaigns;