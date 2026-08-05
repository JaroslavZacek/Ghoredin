import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";

import { useAuth } from "../../auth/AuthContext";

import { getCampaign } from "../../campaigns/api/campaignsApi";
import { creationMethodLabel } from "../../campaigns/utils/campaignHelpers";

import { getCharacter } from "../api/charactersApi";
import CharacterCreation from "./CharacterCreation";

import "./CharacterCreationPage.css";

export default function CharacterCreationPage() {
    const { id } = useParams();
    const navigate = useNavigate();
    const { user } = useAuth();

    const [campaign, setCampaign] = useState(null);
    const [existingCharacter, setExistingCharacter] = useState(null);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const load = async () => {
            setError("");
            try {
                const campaignData = await getCampaign(id);
                setCampaign(campaignData);

                const myMembership = campaignData.members.find((m) => m.userId === user.userId);
                if (myMembership?.characterId) {
                    const character = await getCharacter(myMembership.characterId);
                    setExistingCharacter(character);
                }
            }
            catch (error) {
                setError("Nepodařilo se načíst dobrodružství: " + error.message);
            }
            finally {
                setLoading(false);
            }
        };
        load();
    }, [id, user.userId]);

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
                existingCharacter={existingCharacter}
                onCreated={handleDone}
            />
        </div>
    );
}