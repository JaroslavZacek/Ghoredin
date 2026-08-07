import { useState, useEffect } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";

import { useAuth } from "../../auth/AuthContext";
import { useTheme } from "../../../shared/theme/ThemeContext";

import { getCampaign, deleteCampaign, leaveCampaign } from "../api/campaignsApi";
import { getCampaignCharacters, createCharacterInCampaign, deleteCharacter } from "../../characters/api/charactersApi";
import { isCharacterComplete } from "../utils/campaignHelpers";

import NoteList from "../../notes/components/NoteList";
import CurrentScene from "../../notes/components/CurrentScene";

import { getThemeForGameSystem } from "../../../shared/theme/themeUtils";
import "./CampaignDetail.css"

function CampaignDetail() {
    const { id } = useParams();
    const { user } = useAuth();
    const { setActiveTheme } = useTheme();
    const navigate = useNavigate();

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

    useEffect(() => {
        if (campaign) {
            setActiveTheme(getThemeForGameSystem(campaign.gameSystemId));
        }
        return () => setActiveTheme("shell");
    }, [campaign]);

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

    const handleDeleteCharacter = async (characterId) => {
        if(!window.confirm("Opravdu chceš smazat tuto postavu?")) {
            return;
        }

        try {
            await deleteCharacter(characterId);
            await load ();
        }
        catch (error) {
            setError("Nepodařilo se smazat postavu: " + error.message);
        }
    };

    const handleLeaveCampaign = async () => {
        if (!window.confirm("Opravdu chceš odejít z tohoto dobrodružství? Tvá postava zůstane zachována, kdyby ses chtěl vrátit.")) {
            return;
        }
        
        try {
            await leaveCampaign(id);
            navigate("campaigns");
        }
        catch (error) {
            setError("Nepodařilo se odejít z dobrodružství: " + error.message);
        }
    };

    const handleDeleteCampaign = async () => {
        if(!window.confirm("Opravdu chceš smazat tohle dobrodružství? Tato akce je nevratná a smaže i všechny postavy a poznámky.")) {
            return;
        }

        try {
            await deleteCampaign(id);
            navigate("/campaigns");
        }
        catch (error) {
            setError("Nepodařilo se smazat dobrodružství: " + error.message);
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

    const myCharacter = characters.find((c) => c.id === myMembership?.characterId);
    const myCharacterComplete = isCharacterComplete(myCharacter);

    const players = campaign.members
                        .filter((m) => m.role ==="Player")
                        .map((m) => {
                            const character = characters.find((c) => c.id === m.characterId);

                            return {
                                userId: m.userId,
                                characterName: character ? character.name : null
                            };
                        });

    return (
        <div className="campaign-detail">
            <div className="campaign-detail__header">
                
                <div className="campaign-detail__heading">
                    <h2 className="campaign-detail__title">{campaign.name}</h2>

                    <span className="campaign-detail__system">{campaign.gameSystemId}</span>
                </div>

                {
                    iAmGameMaster && (
                        <button
                            className="campaign-detail__delete"
                            onClick={handleDeleteCampaign}
                        >
                            Smazat dobrodružství
                        </button>
                    )
                }

                {
                    !iAmGameMaster && myMembership && (
                        <button
                            className="campaign-detail__leave"
                            onClick={handleLeaveCampaign}
                        >
                            Odejdi z dobrodružství
                        </button>
                    )
                }
            </div>

            {
                error &&
                    <p className="campaign-detail__error">{error}</p>
            }

            <CurrentScene campaignId={id} />
            <section className="campaign-detail__section">
    
                <div className="campaign-detail__section-header">
                    <h3 className="campaign-detail__section-title">
                        Členové ({campaign.playerCount}
                        {campaign.maxPlayers != null ? ` / ${campaign.maxPlayers}` : ""} hráčů)
                    </h3>

                    {
                        !iAmGameMaster && !iHaveCharacter &&
                        (
                            <button
                                className="campaign-detail__action"
                                onClick={() => navigate(`/campaigns/${id}/create-character`)}
                            >
                                Vytvoř postavu
                            </button>
                        )
                    }

                    {
                        !iAmGameMaster && iHaveCharacter && !myCharacterComplete &&
                        (
                            <button
                                className="campaign-detail__action"
                                onClick={() => navigate(`/campaigns/${id}/create-character`)}
                            >
                                Dokonči postavu
                            </button>
                        )
                    }

                </div>

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
                                        {
                                            character ? (
                                                <>
                                                    <Link
                                                        to={`/campaigns/${id}/characters/${character.id}`}
                                                        className="member-row__character-link"
                                                    >
                                                        {character.name}
                                                    </Link>
                                                    {
                                                        iAmGameMaster && (
                                                            <button
                                                                className="member-row__delete-character"
                                                                onClick={() => handleDeleteCharacter(character.id)}
                                                            >
                                                                Smazat
                                                            </button>
                                                        )
                                                    }
                                                </>
                                                
                                            ) : (
                                                <em>bez postavy</em>
                                            )
                                        }
                                    </span>
                                </li>
                            );
                        })
                    }
                </ul>
            </section>

            <section className="campaign-detail__section">
                <h3 className="campaign-detail__section-title">Poznámky a příběh</h3>
                <NoteList campaignId={campaign.id} isGameMaster={iAmGameMaster} players={players}/>
            </section>

            
        </div>
    );
}

export default CampaignDetail;