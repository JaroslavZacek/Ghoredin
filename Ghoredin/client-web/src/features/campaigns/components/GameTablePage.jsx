import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";

import { useAuth } from "../../auth/AuthContext";
import { useTheme } from "../../../shared/theme/ThemeContext";

import { getThemeForGameSystem} from "../../../shared/theme/themeUtils";
import { getCampaign } from "../api/campaignsApi";
import { getCampaignCharacters} from "../../characters/api/charactersApi";

import CurrentScene from "../../notes/components/CurrentScene";
import ChatPanel from "../../chat/components/ChatPanel";

import "./GameTablePage.css";

export default function GameTablePage() {
    const { id } = useParams();
    const { user } = useAuth();
    const { setActiveTheme } = useTheme();
    const navigate = useNavigate();

    const [campaign, setCampaign] = useState(null);
    const [characters, setCharacters] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        const load = async () => {
            try {
                const campaignData = await getCampaign(id);
                const charactersData = await getCampaignCharacters(id);

                setCampaign(campaignData);
                setCharacters(charactersData);
            }
            catch (error) {
                setError("Nepodařilo se načíst herní stůl: " + error.message);
            }
            finally {
                setLoading(false);
            }
        };
        load();    
    }, [id]);

    useEffect(() => {
        if (campaign) {
            setActiveTheme(getThemeForGameSystem(campaign.gameSystemId));
        }

        return () => setActiveTheme("shell");
    }, [campaign]);

    if (loading)
        return <p>Načítání herního stolu...</p>

    if (error)
        return <p className="game-table__error">{error}</p>

    if (!campaign)
        return <p>Dobrodružství nenalezeno.</p>

    const myMembership = campaign.member?.find((m) => m.userId === user.userId);
    const iAmGameMaster = myMembership?.role === "GameMaster";
    const players = campaign.members
        .filter((m) => m.role === "Player")
        .map((m) => {
            const character = characters.find((c) => c.id === m.characterId);
            return { userId: m.userId, characterName: character ? character.name : null};
        });

    return (
        <div className="game-table">
            <div className="game-table__header">
                <button className="game-table__back" onClick={() => navigate(`/campaigns/${id}`)}>
                    Zpět na dobrodružství
                </button>
                <h2 className="game-table__title">{campaign.name} - Herní stůl</h2>
            </div>

            <CurrentScene campaignId={id} />

            <section className="game-table__section">
                <h3 className="game-table__section-title">Chat</h3>

                <ChatPanel 
                    campaignId={id}
                    currentUserId={user.userId}
                    isGameMaster={iAmGameMaster}
                    players={players}
                />
            </section>
        </div>
    );
}