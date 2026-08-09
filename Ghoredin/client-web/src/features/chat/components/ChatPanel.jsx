import { useState, useEffect, useRef } from "react";

import { getChatHistory, sendMessage } from "../api/chatApi";
import { getHubConnection } from "../../../shared/signalr/campaignHubConnection";

import "./ChatPanel.css";

export default function ChatPanel({ campaignId, currentUserId, isGameMaster, players }) {
    const [messages, setMessages] = useState([]);
    const [text, setText] = useState("");
    const [whisperTo, setWhisperTo] = useState("");

    const [error, setError] = useState("");

    const bottomRef = useRef(null);

    useEffect(() => {
        const load = async () => {
            try {
                const data = await getChatHistory(campaignId);
                setMessages(data);
            }
            catch (error) {
                setError("Nepodařilo se načíst chat: " + error.message);
            }
        }
        load();
    }, [campaignId]);

    useEffect(() => {
        let conn;
        let handler;

        const subscribe = async () => {
            conn = await getHubConnection();

            handler = (message) => {
                if (message.campaignId !== campaignId)
                    return;

                setMessages((prev) => [...prev, message]);
            };
            
            conn.on("ReceiveMessage", handler);
        };

        subscribe();

        return () => {
            if (conn && handler)
                conn.off("ReceiveMessage", handler);
        };
    }, [campaignId]);

    useEffect(() => {
        bottomRef.current?.scrollIntoView({ behavior: "smooth" });
    }, [messages]);

    const handleSend = async () => {
        setError("");
        if (!text.trim())
            return;

        try {
            await sendMessage(campaignId, text, whisperTo || null);
            setText("");
        }
        catch (error) {
            setError("Nepodařilo se odeslat: " + error.message);
        }
    };

    return (
        <div className="chat-panel">
            {
                error && 
                    <p className="chat-panel__error">{error}</p>
            }

            <div className="chat-panel__messages">
                {
                    messages.map((m) => (
                        <div
                            key={m.id}
                             className={`chat-message ${m.authorUserId === currentUserId ? "chat-message--mine" : ""} ${m.isWhisper ? "chat-message--whisper" : ""}`}
                        >
                            {
                                m.isWhisper &&
                                    <span className="chat-message__whisper-tag"></span>
                            }
                            <span className="chat-message__content">{m.content}</span>
                        </div>
                    ))
                }
                <div ref={bottomRef} />
            </div>

            <div className="chat-panel__composer">
                {
                    isGameMaster && (
                        <select
                            className="chat-panel__whisper-select"
                            value={whisperTo}
                            onChange={(e) => setWhisperTo(e.target.value)}
                        >
                            <option value="">Všem</option>
                            {
                                players.map((p) => (
                                    <option key={p.userId} value={p.userId}>
                                        Šepot: {p.characterName || "Hráč bez postavy"}
                                    </option>
                                ))
                            }
                        </select>
                    )
                }

                <input
                    className="chat-panel__input" 
                    type="text" 
                    placeholder="Napiš zprávu..."
                    value={text}
                    onChange={(e) => setText(e.target.value)}
                    onKeyDown={(e) => e.key === "Enter" && handleSend()}
                />

                <button
                    className="chat-panel__send"
                    onClick={handleSend}
                >
                    Odeslat
                </button>
            </div>
        </div>
    );

}