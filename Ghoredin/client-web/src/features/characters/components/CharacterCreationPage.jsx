import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";

import { getCampaign } from "../../campaigns/api/campaignsApi";
import { creationMethodLabel } from "../../campaigns/utils/campaignHelpers";
import CharacterCreation from "./CharacterCreation";

import "./CharacterCreationPage.css";

export default function CharacterCreationPage() {
    const { id } = useParams();
    const navigate = useNavigate();

    const [campaign, setCampaign] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const load = async () => {
            try {
                const data = await getCampaign(id);
                setCampaign(data);
            }
            catch (error) {
                setError("Nepodařilo se načíst dobrodružství: " + error.message);
            }
            finally {
                setLoading(false);
            }
        };
        load();
    }, [id]);

    const handleDone = () => {
        navigate(`/campaigns/${id}`);
    };

    if (loading)
        return <p>Načítání...</p>
    if (!campaign)
        return <p>{error || "Dobrodružství nenelezeno"}</p>
    
    return (
        <div className="creation-page">
            <button className="creation-page__back" onClick={() => navigate(`/campaigns/${id}`)}>
                Zpět na dobrodružství
            </button>

            <h2 className="creation-page__title">{campaign.name}</h2>
            <p className="creation-page__method">
                Metoda tvorby: {creationMethodLabel(campaign.characterCreation)}
            </p>

            <CharacterCreation 
                campaignId={id}
                creationMethod={campaign.characterCreation}
                onCreated={handleDone}
            />
        </div>
    );
}