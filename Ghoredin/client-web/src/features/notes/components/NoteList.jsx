import { useState, useEffect } from "react";

import { getCampaignNotes } from "../api/notesApi";

import "./NoteList.css";

function NoteList({ campaignId }) {
    const [notes, setNotes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const loadNotes = async () => {
        setError("");

        try {
            const data = await getCampaignNotes(campaignId);

            setNotes(data);
        }
        catch (error) {
            setError("Nepodařilo se načíst poznámky: " + error.message);
        }
        finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadNotes();
    }, [campaignId]);

    if (loading) {
        return <p>Načítání poznámek...</p>
    }

    return (
        <div className="note-list">
            
            {
                error &&
                    <p className="note-list__error">{error}</p>
            }

            {
                notes.length === 0
                    ? (
                        <p className="note-list__empty">Zatím žádné poznámky.</p>
                    )
                    : (
                        <ul className="note-list__items">
                            {
                                notes.map((n) => (
                                    <li key={n.id} className="note-card">
                                        <div className="note-card__header">
                                            <span className="note-card__title">{n.title}</span>
                                            <span className="note-card__visibility">
                                                {n.visibility === "GmOnly" ? "Jen PJ" : "Sdíleno"} 
                                            </span>
                                        </div>

                                        {n.content && (
                                            <p className="note-card__content">{n.content}</p>
                                        )}

                                        {n.playerFacingContent && (
                                            <p className="note-card__player-facing">
                                                {n.playerFacingContent}
                                            </p>
                                        )}
                                    </li>
                                ))
                            }
                        </ul>
                    )
            }

        </div>
    );
}

export default NoteList;