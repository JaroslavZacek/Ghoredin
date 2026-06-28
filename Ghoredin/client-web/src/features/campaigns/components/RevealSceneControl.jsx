import { useState } from "react";

import { revealScene } from "../../notes/api/notesApi";

import "./RevealSceneControl.css";

function RevealSceneControl({ campaignId, noteId, players, onRevealed }) {
    const [open, setOpen] = useState(false);
    const [selected, setSelected] = useState([]);
    const [error, setError] = useState("");

    const togglePlayer = (userId) => {
        setSelected((prev) =>
            prev.includes(userId)
                ? prev.filter((id) => id !== userId)
                : [...prev, userId]
        );
    };

    const selectAll = () => {
        setSelected(players.map((p) => p.userId));
    };

    const handleReveal = async () => {
        setError("");

        if (selected.length === 0) {
            setError("Vyber aspoň jednoho hráče.");
            return;
        }

        try {
            await revealScene({
                campaignId,
                noteId,
                targetUserIds: selected
            });

            setOpen(false);
            setSelected([]);

            if (onRevealed) {
                onRevealed();
            }   
        }

        catch (error) {
            setError("Nepodařilo se odhalit: " + error.message);
        }
    };

    if (!open) {
        return (
            <button className="reveal-control__toggle" onClick={() => setOpen(true)}>
                Zobrazit hráče
            </button>
        );
    }

    return (
        <div className="reveal-control">
            {
                error &&
                    <p className="reveal-control__error">{error}</p>
            }

            <div className="reveal-control__players">
                {
                    players.length === 0
                        ? (
                            <span className="reveal-control__empty">Žádní hráči v dobrodružství</span>
                        ) : (
                            players.map((p) => (
                                <label key={p.userId} className="reveal-control__player">
                                    <input 
                                        type="checkbox"
                                        checked={selected.includes(p.userId)}
                                        onChange={() => togglePlayer(p.userId)}
                                    />
                                    {p.characterName || "Hráč bez postavy"}
                                </label>
                            ))
                        )
                }
            </div>

            <div className="reveal-control__actions">
                <button className="reveal-control__all" onClick={selectAll}>
                    Vybrat všechny
                </button>
                <button className="reveal-control__confirm" onClick={handleReveal}>
                    Odhalit
                </button>
                <button className="reveal-control__cancel" onClick={() => setOpen(false)}>
                    Zrušit
                </button>
            </div>
        </div>
    );
}

export default RevealSceneControl;