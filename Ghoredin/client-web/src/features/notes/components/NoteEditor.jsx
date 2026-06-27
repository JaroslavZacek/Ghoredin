import { useState, useEffect } from "react";

import { createNote, updateNote } from "../api/notesApi";

import "./NoteEditor.css";

function NoteEditor({ campaignId, editingNote, onSaved, onCancel }) {

    const [title, setTitle] = useState("");
    const [content, setContent] = useState("");
    const [playerFacingContent, setPlayerFacingContent] = useState("");
    const [visibility, setVisibility] = useState("GmOnly");
    const [error, setError] = useState("");

    useEffect(() => {

        if (editingNote) {
            setTitle(editingNote.title);
            setContent(editingNote.content ?? "");
            setPlayerFacingContent(editingNote.playerFacingContent ?? "");
            setVisibility(editingNote.visibility);
        }
        else {
            setTitle("");
            setContent("");
            setPlayerFacingContent("");
            setVisibility("GmOnly");
        }
        setError("");

    }, [editingNote]);

    const handleSave = async () => {
        setError("");

        if (!title.trim()) {
            setError("Zadej titulek poznámky.");
            return;
        }

        const data = {
            title,
            content,
            playerFacingContent: playerFacingContent.trim() === "" ? null : playerFacingContent,
            visibility
        };

        try {
            if (editingNote) {
                await updateNote(editingNote.id, data);
            }
            else {
                await createNote({ campaignId, ...data });
            }
            onSaved();
        }
        catch (error) {
            setError("Nepodařilo se uložit: " + error.message);
        }
    };

    return (
        <div className="note-editor">
            <h4 className="note-editor__title">
                {editingNote ? "Upravit poznámku" : "Nová poznámka"}
            </h4>

            {
                error &&
                    <p className="note-editor__error">{error}</p>
            }

            <input 
                className="note-editor__input"
                type="text"
                placeholder="Titulek"
                value={title}
                onChange={(e) => setTitle(e.target.value)}            
            />

            <label className="note-editor__label">Obsah (tajná příprava)</label>

            <textarea 
                className="note-editor__textarea"
                placeholder="Tvoje poznámky ke scéně..."
                value={content}
                onChange={(e) => setContent(e.target.value)}
                rows={3}
            />

            <label className="note-editor__label">Nástin pro hráče (volitelné)</label>

            <textarea 
                className="note-editor__textarea"
                placeholder="Co uvidí hráči, když jim scénu sdílíš..."
                value={playerFacingContent}
                onChange={(e) => setPlayerFacingContent(e.target.value)}
                rows={2}
            />

            <label className="note-editor__label">Viditelnost</label>

            <select
                className="note-editor__input"
                value={visibility}
                onChange={(e) => setVisibility(e.target.value)}
            >
                <option value="GmOnly">Jen PJ</option>
                <option value="SharedWithPlayers">Sdíleno s hráři</option>
            </select>

            <div className="note-editor__actions">
                <button className="note-editor__save" onClick={handleSave}>
                    {editingNote ? "Uložit změny" : "Vytvořit"}
                </button>

                {
                    editingNote &&
                        <button className="note-editor__cancel" onClick={onCancel}>
                            Zrušit
                        </button>
                }
            </div>
        </div>
    );
}

export default NoteEditor;