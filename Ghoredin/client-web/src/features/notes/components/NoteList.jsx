import { useState, useEffect } from "react";
import { IconChevronDown } from "@tabler/icons-react";

import { getCampaignNotes } from "../api/notesApi";

import NoteEditor from "./NoteEditor";
import RevealSceneControl from "./RevealSceneControl";

import "./NoteList.css";

function NoteList({ campaignId, isGameMaster, players }) {
    const [notes, setNotes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [expandedId, setExpandedId] = useState(null);
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
        setExpandedId(null);
        loadNotes();
    };

    const handleCancel = () => {
        setEditingNote(null);
        setExpandedId(null);
    }

    const toggleNew = () => {
        if (expandedId === "new") {
            setExpandedId(null);
        }
        else {
            setEditingNote(null);
            setExpandedId("new");
        }
    };

    const toggleNote = (note) => {
        if (expandedId === note.id) {
            setExpandedId(null);
        }
        else {
            setEditingNote(null);
            setExpandedId(note.id);
        }
    };

    const startEdit = (note, e) => {
        e.stopPropagation();
        setEditingNote(note);
        setExpandedId(note.id);
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
                    <div className="note-item">
                        <div className="note-item__header" onClick={toggleNew}>
                            <span className="note-item__title">Nová poznámka</span>
                            <IconChevronDown 
                                size={16}
                                className={`note-item__chevron ${expandedId === "new" ? "note-item__chevron--open" : ""}`}
                            />
                        </div>
                        {
                            expandedId === "new" && 
                            (
                                <div className="note-item__body">
                                    <NoteEditor 
                                        campaignId={campaignId}
                                        editingNote={null}
                                        onSaved={handleSaved}
                                        onCancel={handleCancel}
                                    />
                                </div>
                            )
                        }
                    </div>
                )
            }

            {
                isGameMaster &&
                    <hr className="note-list__divider"/>
            }

            {
                notes.length === 0
                    ? (
                        <p className="note-list__empty">Zatím žádné poznámky.</p>
                    )
                    : (
                        <ul className="note-list__items">
                            {
                                notes.map((n) => {
                                    const isOpen = expandedId === n.id;
                                    const isEditingThis = editingNote?.id === n.id;

                                    return (
                                        <li key={n.id} className="note-item">
                                            <div className="note-item__header" onClick={() => toggleNote(n)}>
                                                <span className="note-item__title">{n.title}</span>
                                                <span className="note-item__header-right">
                                                    <span className="note-card__visibility">
                                                        {n.visibility === "GmOnly" ? "Jen PJ" : "Sdíleno"}
                                                    </span>

                                                    {
                                                        isGameMaster && (
                                                            <button
                                                                className="note-card__edit"
                                                                onClick={(e) => startEdit(n, e)}
                                                            >
                                                                Upravit
                                                            </button>
                                                        )
                                                    }
                                                    <IconChevronDown 
                                                        size={16}
                                                        className={`note-item__chevron ${isOpen ? "note-item__chevron--open" : ""}`}
                                                    />
                                                </span>
                                            </div>

                                            {
                                                isOpen && (
                                                    <div className="note-item__body">
                                                        {
                                                            isEditingThis ? (
                                                                <NoteEditor
                                                                    campaignId={campaignId}
                                                                    editingNote={n}
                                                                    onSaved={handleSaved}
                                                                    onCancel={handleCancel} 
                                                                />
                                                            ) : (
                                                                <>
                                                                    {
                                                                        n.content &&
                                                                            <p className="note-card__content">{n.content}</p>
                                                                    }

                                                                    {
                                                                        n.playerFacingContent &&
                                                                            <p className="note-card__player-facing">{n.playerFacingContent}</p>
                                                                    }

                                                                    {
                                                                        isGameMaster && n.visibility === "SharedWithPlayers" && (
                                                                            <RevealSceneControl 
                                                                                campaignId={campaignId}
                                                                                noteId={n.id}
                                                                                players={players}
                                                                                onRevealed={() => {}}
                                                                            />
                                                                        )
                                                                    }
                                                                </>
                                                            )
                                                        }
                                                    </div>
                                                )
                                            }
                                        </li>
                                    );
                                })
                            }
                        </ul>
                    )
            }

        </div>
    );
}

export default NoteList;