import { useState, useEffect, useRef } from "react";

import { getMyJournal, saveMyJournal } from "../api/journalApi";

import "./JournalPanel.css";

export default function JournalPanel({ campaignId }) {
    const [content, setContent] = useState("");

    const [loading, setLoading] = useState(true);
    const [status, setStatus] = useState("");
    const [error, setError] = useState("");

    const saveTimeout = useRef(null);

    useEffect(() => {
        const load = async () => {
            setError("");

            try {
                const entry = await getMyJournal(campaignId);
                setContent(entry?.content ?? "");
            }
            catch (error) {
                setError("Nepodařilo se načíst deník: " + error.message);
            }
            finally {
                setLoading(false);
            }
        };
        load();
    }, [campaignId]);

    const handleChange = (value) => {
        setContent(value);
        setStatus("");

        if (saveTimeout.current)
            clearTimeout(saveTimeout.current);

        saveTimeout.current = setTimeout(async () => {
            setStatus("saving")

            try {
                await saveMyJournal(campaignId, value);
                setStatus("saved");
            }
            catch (error) {
                setError("Nepodařilo se uložit: " + error.message);
                setStatus("");
            }
        }, 800);
    };

    useEffect(() => {
        return () => {
            if (saveTimeout.current)
                clearTimeout(saveTimeout.current);
        };
    }, []);

    if (loading)
        return <p>Načítání deníku...</p>

    return (
        <div className="journal-panel">
            <div className="journal-panel__header">
                <span className="journal-panel__badge">jen pro tebe</span>
                {
                    status === "saving" &&
                        <span className="journal-panel__status">Ukládám...</span>
                }

                {
                    status === "saved" &&
                        <span className="journal-panel__status">Uloženo</span>
                }
            </div>

            {
                error &&
                    <p className="journal-panel__error">{error}</p>
            }

            <textarea 
                className="journal-panel__textarea"
                placeholder="Připiš poznámku..."
                value={content}
                onChange={(e) => handleChange(e.target.value)}
                rows={5}
            />

        </div>
    );
}