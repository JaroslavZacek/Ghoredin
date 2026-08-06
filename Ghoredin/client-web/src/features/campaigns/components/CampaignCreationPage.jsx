import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { createCampaign } from "../api/campaignsApi";

import "./CampaignCreationPage.css";

export default function CampaignCreationPage() {
    const navigate = useNavigate();

    const [name, setName] = useState("");
    const [gameSystemId, setGameSystemId] = useState("dnd5e");
    const [maxPlayers, setMaxPlayers] = useState("");
    const [characterCreationMethod, setCharacterCreationMethod] = useState("PointBuy");
    const [charactersVisibleToAll, setCharactersVisibleToAll] = useState(false);

    const [error, setError] = useState("");
    const [saving, setSaving] = useState(false);

    const handleCreate = async () => {
        setError("");

        if (!name.trim()) {
            setError("Zadej název dobrodružství.");
            return;
        }

        setSaving(true);
        try {
            const campaign = await createCampaign({
                name,
                gameSystemId,
                maxPlayers: maxPlayers === "" ? null : Number(maxPlayers),
                characterCreationMethod,
                charactersVisibleToAll
            });

            navigate(`/campaigns/${campaign.id}`);
        }
        catch (error) {
            setError("Nepodařilo se vytvořit dobrodružství: " + error.message);
            setSaving(false);
        }
    };

    return (
        <div className="campaign-creation-page">
            <button
                className="campaign-creation-page__back"
                onClick={() => navigate("/campaigns")}
            >
                Zpět na výpis mích dobrodružství
            </button>

            <h2 className="campaign-creation-page__title">Nové dobrodružství</h2>

            {
                error &&
                    <p className="campaign-creation-page__error">{error}</p>
            }

            <div className="campaign-creation-page__form">
                <input 
                    className="campaign-creation-page__form-input"
                    type="text" 
                    placeholder="Název dobrodružství"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />

                <select
                    className="campaign-creation-page__form-input"
                    value={gameSystemId}
                    onChange={(e) => setGameSystemId(e.target.value)}
                >
                    <option value="dnd5e">D&D 5e</option>
                </select>

                <select
                    className="campaign-creation-page__form-input"
                    value={characterCreationMethod}
                    onChange={(e) => setCharacterCreationMethod(e.target.value)}
                >
                    <option value="PointBuy">Nákup bodů</option>
                    <option value="StandardArray">Pevná sada</option>
                    <option value="Roll">Házení kostkami</option>
                </select>

                <input 
                    className="campaign-creation-page__form-input"
                    type="number"
                    min="1"
                    placeholder="Max. hráčů (prázdné = bez limitu)"
                    value={maxPlayers}
                    onChange={(e) => setMaxPlayers(e.target.value)} 
                />

                <label className="campaign-creation-page__form-checkbox">
                    <input 
                        type="checkbox"
                        checked={charactersVisibleToAll}
                        onChange={(e) => setCharactersVisibleToAll(e.target.checked)} 
                    />
                    Postavi vidí všichni členové (jinak jen vlastník a PJ)
                </label>

                <button
                    className="campaign-creation-page__form-btn"
                    onClick={handleCreate}
                    disabled={saving}
                >
                    {saving ? "Vytvářím..." : "Vytvořit dobrodružství"}
                </button>
            </div>
        </div>
    );

}