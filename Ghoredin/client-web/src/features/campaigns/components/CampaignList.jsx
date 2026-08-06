import { useState, useEffect } from "react";
import { Link, Links, useNavigate } from "react-router-dom";

import { getMyCampaigns, createCampaign } from "../api/campaignsApi";
import { creationMethodLabel } from "../utils/campaignHelpers";

import "./CampaignList.css"


function CampaignList() {
    const navigate = useNavigate();

    const [campaigns, setCampaigns] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    // Formulář
    const [name, setName] = useState("");
    const [gameSystemId, setGameSystemId] = useState("dnd5e");
    const [maxPlayers, setMaxPlayers] = useState("");
    const [characterCreationMethod, setCharacterCreationMethod] = useState("PointBuy");
    const [charactersVisibleToAll, setCharactersVisibleToAll] = useState(false);

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

    if (loading) {
        return <p>Načítání kampaní...</p>;
    }

    return (
        <div className="campaign-list">
            <button className="campaign-list__create-button" onClick={() => navigate("/campaigns/create")}>
                Nové dobrodružství
            </button>

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
                                    <Link to={`/campaigns/${c.id}`} className="campaign-card campaign-card--link">
                                        <div className="campaign-card__main">
                                            <span className="campaign-card__name">{c.name}</span>
                                            <span className="campaign-card__system">{c.gameSystemId}</span>
                                        </div>

                                        <span className="campaign-card__players">
                                            {c.playerCount}
                                            {c.maxPlayers != null ? ` / ${c.maxPlayers}` : ""} hráčů
                                        </span>
                                        <span className="campaign-card__creation ">
                                            Tvorba postavy: {creationMethodLabel(c.characterCreation)}
                                        </span>
                                    </Link>
                                </li>
                            ))
                        }
                    </ul>
                )
            }
        </div>
    );
}

export default CampaignList;