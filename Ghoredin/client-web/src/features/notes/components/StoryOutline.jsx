import { useState, useEffect } from "react";
import { IconArrowUp, IconArrowDown, IconCornerDownRight } from "@tabler/icons-react";

import { getCampaignNotes, moveNote, reorderSiblings } from "../api/notesApi";
import NoteEditor from "./NoteEditor";
import RevealSceneControl from "./RevealSceneControl";

import "./StoryOutline.css";

export default function StoryOutline({ campaignId, isGameMaster, players }) {
    const [notes, setNotes] = useState([]);
    const [editingNote, setEditingNote] = useState(null);
    const [creatingUnderParent, setCreatingUnderParent] = useState(undefined);
    const [selectedId, setSelectedId] = useState(null);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const load = async () => {
        setError("");
        try {
            const data = await getCampaignNotes(campaignId);
            setNotes(data);
        }
        catch (error) {
            setError("Nepodařilo se načíst osnovu: " + error.message);
        }
        finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        load();
    }, [campaignId]);

    const roots = notes.filter((n) => !n.parentNoteId);
    const childrenOf = (parentId) =>
        notes.filter((n) => n.parentNoteId === parentId).sort((a, b) => a.sortOrder - b.sortOrder);

    const selectedNote = notes.find((n) => n.id === selectedId) ?? null;

    const handleMoveUpDown = async (note, direction) => {
        const siblings = childrenOf(note.parentNoteId ?? null);
        const index = siblings.findIndex((s) => s.id === note.id);
        const swapWith = direction === "up" ? index -1 : index + 1;

        if (swapWith < 0 || swapWith >= siblings.length)
            return;

        const reordered = [...siblings];
        [reordered[index], reordered[swapWith]] = [reordered[swapWith], reordered[index]];

        try {
            await reorderSiblings(campaignId, note.parentNoteId ?? null, reordered.map((s) => s.id));
            await load();
        }
        catch (error) {
            setError("Nepodařilo se přeskládat: " + error.message);
        }

    }

    const handleMoveTo = async (note, newParentId) => {
        try {
            await moveNote(note.id, newParentId);
            await load();
        }
        catch (error) {
            setError("Nepodařilo se přesunout: " + error.message);
        }
    };

    const handleSaved = () => {
        setEditingNote(null);
        setCreatingUnderParent(undefined);
        load();
    };

    const renderNote = (note, depth) => {
        const isSelected = note.id === selectedId;
        const children = childrenOf(note.id);

        return (
            <div key={note.id}>
                <div
                    className={`story-outline__row ${isSelected ? "story-outline__row--active" : ""}`}
                    style={{ paddingLeft: `${depth * 16}px` }}
                    onClick={() => setSelectedId(note.id)}
                >
                    <span className="story-outline__row-title">{note.title}</span>
                    {
                        isGameMaster && (
                            <span className="story-outline__row-actions">
                                <button onClick={(e) => { e.stopPropagation(); handleMoveUpDown(note, "up"); }}>
                                    <IconArrowUp size={13} />
                                </button>
                                <button onClick={(e) => { e.stopPropagation(); handleMoveUpDown(note, "down"); }}>
                                    <IconArrowDown size={13} />
                                </button>
                            </span>
                        )
                    }
                </div>
                {
                    children.map((child) => renderNote(child, depth + 1))
                }
            </div>
        );
    };

    if (loading)
        return <p>Načítání osnovy...</p>

    return (
        <div className="story-outline">
            {
                error &&
                    <p className="story-outline__error">{error}</p>
            }

            <div className="story-outline__layout">
                <div className="story-outline__tree">
                    {
                        roots.map((note) => renderNote(note, 0))
                    }

                    {
                        isGameMaster && (
                            <button
                                className="story-outline__add-root"
                                onClick={() => {setEditingNote(null); setCreatingUnderParent(null); }}
                            >
                                Nový akt
                            </button>
                        )
                    }
                </div>

                <div className="story-outline__detail">
                    {
                        creatingUnderParent !== undefined ? (
                            <NoteEditor 
                                campaignId={campaignId}
                                editingNote={editingNote}
                                parentNoteId={creatingUnderParent}
                                onSaved={handleSaved}
                                onCancel={() => { setEditingNote(null); setCreatingUnderParent(undefined); }}
                            />
                        ) : selectedNote ? (
                                <div>
                                    <div className="story-outline__detail-header">
                                        <h4>{selectedNote.title}</h4>
                                        {
                                            isGameMaster && (
                                                <div className="story-outline__detail-actions">
                                                    <button onClick={() => { setEditingNote(null); setCreatingUnderParent(selectedNote.id); }}>
                                                        <IconCornerDownRight size={14} /> Nová scéna zde
                                                    </button>
                                                    <select
                                                        value={selectedNote.parentNoteId ?? ""}
                                                        onChange={(e) => handleMoveTo(selectedNote, e.target.value || null)}
                                                    >
                                                        <option value="">Přesunout pod: (kořen)</option>
                                                        {
                                                            notes.filter((n) => n.id !== selectedNote.id)
                                                                .map((n) => (
                                                                    <option key={n.id} value={n.id}>
                                                                        Přesunout pod: {n.title}
                                                                    </option>
                                                                ))
                                                        }
                                                    </select>
                                                </div>

                                            )

                                        }
                                    </div>

                                {
                                    selectedNote.content &&
                                        <p className="story-outline__content">{selectedNote.content}</p>
                                }

                                {
                                    selectedNote.playerFacingContent && 
                                        <p className="story-outline__player-facing">{selectedNote.playerFacingContent}</p>
                                }

                                {
                                    isGameMaster && selectedNote.visibility === "SharedWithPlayers" && (
                                        <RevealSceneControl 
                                            campaignId={campaignId}
                                            noteId={selectedNote.id}
                                            players={players}
                                            onRevealed={() => {}}
                                        />
                                    ) 
                                }
                            </div>
                        ) : (
                            <p className="story-outline__empty">Vyber scénu v osnově vlevo.</p>
                        )
                    }
                </div>
            </div>
        </div>
    );
}