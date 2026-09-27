import { useState } from "react";
import { useNavigate } from "react-router-dom";

import { useAuth } from "../../auth/AuthContext";

import "./ProfilePage.css";

export default function ProfilePage() {
    const { user, updateNickname } = useAuth();
    const navigate = useNavigate();

    const [nickname, setNickname] = useState(user.nickname ?? "");
    
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState("");
    const [saved, setSaved] = useState(false);

    const handleSave = async () => {
        setError("");
        setSaved(false);
        setSaving(true);
        try {
            await updateNickname(nickname.trim() === "" ? null : nickname);
            setSaved(true);
        } catch (error) {
            setError("Nepodařilo se uložit:" + error.mesage);
        } finally {
            setSaving(false);
        }
    };

    return (
        <div className="profile-page">
            <button className="profile-page__back" onClick={() => navigate(-1)}>
                Zpět
            </button>

            <h2 className="profile-page__title">Upravit profil</h2>

            <div className="profile-page__field">
                <label className="profile-page__label">E-mail</label>
                <p className="profile-page__email">{user.email}</p>
            </div>

            <div className="profile-page__field">
                <label className="profile-page__label">Přezdívka</label>
                <input
                    className="profile-page__input" 
                    type="text" 
                    placeholder="Jaké jméno mají ostatní vidět?"
                    value={nickname}
                    onChange={(e) => setNickname(e.target.value)}
                />
                <p className="profile-page__hint">
                    Necháš-li prázdné, ostatní tě uvidí podle e-mailu.
                </p>
            </div>

            {
                error &&
                    <p className="profile-page__error">{error}</p>
            }
            {
                saved && 
                    <p className="profile-page__saved">Uloženo</p>
            }

            <button className="profile-page__save" onClick={handleSave} disabled={saving}>
                {
                    saving ? "Ukládám..." : "Uložit"
                }
            </button>
        </div>
    );
}