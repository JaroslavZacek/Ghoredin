import { useState, useEffect } from "react";
import { IconChevronDown } from "@tabler/icons-react";

import { getHandouts, createHandout, updateHandout, shareHandout, unshareHandout, deleteHandout } from "../api/handoutsApi";

import "./HandoutPanel.css";

const SHARE_MODE_LABELS = {
    Live: "sdíleno · živá",
    Snapshot: "sdíleno · snímek"
};

export default function HandoutPanel({ campaignId, isGameMaster }) {
    const [handouts, setHandouts] = useState([]);
    
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    const [expandedId, setExpandedId] = useState(null);
    const [editingId, setEditingId] = useState(null);

    //formulář
    const [title, setTitle] = useState("");
    const [content, setContent] = useState("");
    const [contentType, setContentType] = useState("Text");
    const [shareMode, setShareMode] = useState("Live");

    const load = async () => {
        setError("");

        try {
            const data = await getHandouts(campaignId);
            setHandouts(data);
        }
        catch (error) {
            setError("Nepodařilo se načíst listiny: " + error.message);
        }
        finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        load();
    }, [campaignId]);

    const resetForm = () => {
        setTitle("");
        setContent("");
        setContentType("Text");
        setShareMode("Live");
    };

    const startCreate = () => {
        resetForm();
        setEditingId("new");
        setExpandedId("new");
    };

    const startEdit = (h, e) => {
        e.stopPropagation();
        setTitle(h.title);
        setContent(h.content);
        setEditingId(h.id);
        setExpandedId(h.id);
    };

    const cancelEdit = () => {
        setEditingId(null);

        if (expandedId === "new")
            setExpandedId(null);
    };

    const handleSave = async () => {
        setError("");

        if (!title.trim()) {
            setError("Zadej název listiny.")
            return;
        }

        try {
            if (editingId === "new") {
                const created = await createHandout({ campaignId, title, content, contentType, shareMode });
                setExpandedId(created.id);
            }
            else {
                await updateHandout(editingId, { title, content });
            }
            setEditingId(null);
            await load();
        }
        catch (error) {
            setError("Nepodařilo se uložit: " + error.message);
        }
    };

    const handleToggleShare = async (h) => {
        try {
            if (h.isShared)
                await unshareHandout(h.id);
            else
                await shareHandout(h.id);

            await load();
        }
        catch (error) {
            setError("Nepodařilo se změnit sdílení: " + error.message);
        }
    };

    const handleDelete = async (h, e) => {
        e.stopPropagation();

        if (!window.confirm("Opravdu smazat tuto listinu?"))
            return;

        try {
            await deleteHandout(h.id);
            await load();
        }
        catch (error) {
            setError("Nepodařilo se smazat: " + error.message);
        }
    };

    if (loading)
        return <p>Načítání listin...</p>
        
    return (
        <div className="handout-panel">
            {error && <p className="handout-panel__error">{error}</p>}

            {
                isGameMaster && editingId !== "new" && (
                    <button className="handout-panel__add" onClick={startCreate}>
                        Nová listina
                    </button>
                )
            }

            <div className="handout-panel__list">
                {
                    editingId === "new" && (
                        <div className="handout-item">
                            <div className="handout-item__body">
                                ------ Zde bude formulář ------
                            </div>
                        </div>
                    )
                }

                {
                    handouts.map((h) => {
                        const isOpen = expandedId === h.id;
                        const isEditing = editingId === h.id;

                        return (
                            <div key={h.id} className="handout-item">
                                <div
                                    className="handout-item__header"
                                    onClick={() => setExpandedId(isOpen ? null : h.id)}
                                >
                                    <span className="handout-item__title">{h.title}</span>
                                    <span className="handout-item__header-right">
                                        {
                                            h.isShared && (
                                                <span className="handout-item__badge">
                                                    {SHARE_MODE_LABELS[h.shareMode]}
                                                </span>
                                            )
                                        }

                                        {
                                            isGameMaster && (
                                                <>
                                                    <button onClick={(e) => startEdit(h, e)}>Upravit</button>
                                                    <button onClick={(e) => handleDelete(h, e)}>Smazat</button>
                                                </>
                                            )
                                        }

                                        <IconChevronDown 
                                            size={16}
                                            className={`handout-item__chevron ${isOpen ? "handout-item__chevron--open" : ""}`}
                                        />
                                    </span>
                                </div>

                                {
                                    isOpen && (
                                        <div>
                                            {
                                                isEditing ? (
                                                    <p>
                                                        ------ Zde bude formulář ------
                                                    </p>
                                                ) : (
                                                    <>
                                                        {
                                                            h.contentType === "ImageUrl" ? (
                                                                <img className="handout-item__image" src={h.content} alt={h.title}/>
                                                            ) : (
                                                                <p className="handout-item__content">{h.content}</p>
                                                            )
                                                        }

                                                        {
                                                            isGameMaster && (
                                                                <button
                                                                    className="handout-item__share-toggle"
                                                                    onClick={() => handleToggleShare(h)}
                                                                >
                                                                    {
                                                                        h.isShared ? "Přestat sdílet" : "Sdílet s hráči"
                                                                    }
                                                                </button>
                                                            )
                                                        }
                                                    </>
                                                )
                                            }
                                        </div>
                                    )
                                }
                            </div>
                        );
                    })
                }

                {
                    handouts.length === 0 && editingId !== "new" && (
                        <p className="handout-panel__empty">Zatím žádné listiny.</p>
                    )
                }
            </div>
        </div>
    );
}