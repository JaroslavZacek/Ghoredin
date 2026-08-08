import { useState, useEffect } from "react";
import { Link, Links, useNavigate } from "react-router-dom";

import { getMyCampaigns, createCampaign } from "../api/campaignsApi";
import { creationMethodLabel } from "../utils/campaignHelpers";

import CampaignCard from "./CampaignCard";

import "./CampaignList.css"


function CampaignList() {
    const navigate = useNavigate();

    const [campaigns, setCampaigns] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

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
            <div className="campaign-list__header">
                <h2 className="campaign-list__title">Moje dobrodružství</h2>

                <button
                    className="campaign-list__create-button"
                    onClick={() => navigate("/campaigns/create")}
                >
                    Nové dobrodružství
                </button>
            </div>

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
                                <li key={c.id}>
                                    <CampaignCard campaign={c} />
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