import { useState, useEffect } from "react";

import { getCampaignNotes } from "../api/notesApi";

import NoteEditor from "./NoteEditor";
import RevealSceneControl from "./RevealSceneControl";

import "./NoteList.css";

function NoteList({ campaignId, isGameMaster, players }) {
    const [notes, setNotes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");
    const [editingNote, setEditingNote] = useState(null);

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

    const handleSaved = () => {
        setEditingNote(null);
        loadNotes();
    };

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
                isGameMaster && (
                    <NoteEditor 
                        campaignId={campaignId}
                        editingNote={editingNote}
                        onSaved={handleSaved}
                        onCancel={() => setEditingNote(null)}
                    />
                )
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
                                            <div className="note-card__meta">

                                                <span className="note-card__visibility">
                                                    {n.visibility === "GmOnly" ? "Jen PJ" : "Sdíleno"}
                                                </span>

                                                {
                                                    isGameMaster &&
                                                    <button
                                                        className="note-card__edit"
                                                        onClick={() => setEditingNote(n)}
                                                    >
                                                        Upravit
                                                    </button>
                                                }

                                            </div>
                                        </div>

                                        {n.content && (
                                            <p className="note-card__content">{n.content}</p>
                                        )}

                                        {n.playerFacingContent && (
                                            <p className="note-card__player-facing">
                                                {n.playerFacingContent}
                                            </p>
                                        )}
                                        
                                        {
                                            isGameMaster && n.visibility === "SharedWithPlayers" &&
                                            (
                                                <RevealSceneControl
                                                    campaignId={campaignId}
                                                    noteId={n.id}
                                                    players={players}
                                                    onRevealed={() => {}}
                                                />
                                            )
                                        }

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