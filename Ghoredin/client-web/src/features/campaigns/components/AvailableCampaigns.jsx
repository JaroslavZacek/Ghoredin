import { useState, useEffect } from "react";

import { getAvailableCampaigns, joinCampaign } from "../api/campaignsApi";
import { creationMethodLabel } from "../utils/campaignHelpers";

import CampaignCard from "./CampaignCard";

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
                                <li key={c.id}>
                                    <CampaignCard 
                                        campaign={c}
                                        action={
                                            <button className="campaign-card__join-btn" onClick={() => handleJoin(c.id)}>
                                                Připojit se
                                            </button>
                                        }
                                    />
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