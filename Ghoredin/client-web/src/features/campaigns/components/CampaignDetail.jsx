import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "../../auth/AuthContext";

import { getCampaign } from "../api/campaignsApi";
import { getCampaignCharacters, createCharacterInCampaign } from "../../characters/api/charactersApi";

import NoteList from "../../notes/components/NoteList";

import "./CampaignDetail.css"

function CampaignDetail() {
    const { id } = useParams();
    const { user } = useAuth();

    const [campaign, setCampaign] = useState(null);
    const [characters, setCharacters] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [characterName, setCharacterName] = useState("");

    const load = async () => {
        setError("");
        try {
            const campaignData = await getCampaign(id);
            const charactersData = await getCampaignCharacters(id);

            setCampaign(campaignData);
            setCharacters(charactersData);
        }
        catch (error) {
            setError("Nepodařilo se načíst dobrodružství: " + error.message);
        }
        finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        load();
    }, [id]);

    

    const handleCreateCharacter = async () => {
        setError("");

        if (!characterName.trim()) {
            setError("Zadej jméno postavy.");
            return;
        }

        try {
            await createCharacterInCampaign(id, {
                name: characterName,
                sheetData: {}
            });

            setCharacterName("");
            await load();
        }
        catch (error) {
            setError("Nepodařilo se vytvořit postavu: " + error.message);
        }
    };

    if (loading) {
        return <p>Načítání dobrodružství...</p>
    }

    if (!campaign) {
        return <p>{error || "Dobrodružství nenalezeno."}</p>
    }

    // Najdi moje členství v dobrodružství
    const myMembership = campaign?.members?.find((m) => m.userId === user.userId);
    const iAmGameMaster = myMembership?.role === "GameMaster";
    const iHaveCharacter = myMembership?.characterId != null;

    return (
        <div className="campaign-detail">
            <div className="campaign-detail__header">
                <h2 className="campaign-detail__title">{campaign.name}</h2>

                <span className="campaign-detail__system">{campaign.gameSystemId}</span>
            </div>

            {
                error &&
                    <p className="campaign-detail__error">{error}</p>
            }

            <section className="campaign-detail__section">
                <h3 className="campaign-detail__section-title">
                    Členové ({campaign.playerCount}
                    {campaign.maxPlayers != null ? ` / ${campaign.maxPlayers}` : ""} hráčů)
                </h3>

                <ul className="member-list">
                    {
                        campaign.members.map((m) => {
                            const character = characters.find((c) => c.id === m.characterId);

                            return (
                                <li key={m.id} className="member-row">
                                    <span className="member-row__role">
                                        {m.role === "GameMaster" ? "PJ" : "Hráč"}
                                    </span>
                                    <span className="member-row__character">
                                        {character ? character.name : <em>bez postavy</em>}
                                    </span>
                                </li>
                            );
                        })
                    }
                </ul>
            </section>

            <section className="campaign-detail__section">
                <h3 className="campaign-detail__section-title">Poznámky a příběh</h3>
                <NoteList campaignId={id} isGameMaster={iAmGameMaster}/>
            </section>

            {
                !iAmGameMaster && !iHaveCharacter && 
                (
                    <section className="campaign-detail__section">
                        <h3 className="campaign-detail__section-title">Vytvoř svou postavu</h3>

                        <div className="character-create">
                            <input 
                                className="character-create__input"
                                type="text"
                                placeholder="Jméno postavy"
                                value={characterName}
                                onChange={(e) => setCharacterName(e.target.value)}
                            />

                            <button className="character-create__button" onClick={handleCreateCharacter}>
                                Vytvoř postavu
                            </button>
                        </div>
                    </section>
                )
            }
        </div>
    );
}

export default CampaignDetail;