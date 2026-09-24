import { useState, useEffect, useRef } from "react";
import { IconMicrophone, IconMicrophoneOff, IconPhoneOff, IconVolume } from "@tabler/icons-react";

import { joinVoice, leaveVoice, setSelfMute, setWhisperTargets, forceMute } from "../voiceConnectionManager";

import "./VoicePanel.css";

export default function VoicePanel({ campaignId, isGameMaster, players, currentUserId }) {
    const [connected, setConnected] = useState(false);
    const [participants, setParticipants] = useState([]);
    const [myMuted, setMyMuted] = useState(false);
    const [whisperMode, setWhisperMode] = useState(false);
    const [whisperTargets, setWhisperTargetsLocal] = useState([]);

    const [error, setError] = useState("");

    const audioRefs = useRef({});

    const upsertParticipant = (userId, patch) => {
        setParticipants((prev) => {
            const exists = prev.find((p) => p.userId === userId);
            if (exists) {
                return prev.map((p) => (p.userId === userId ? {...p, ...patch } : p));
            }
            return [...prev, { userId, speaking: false, selfMuted: false, forceMuted: false, ...patch}];
        });
    };

    const handleJoin = async () => {
        setError("");
        try {
            const existing = await joinVoice(campaignId, {
                onParticipantsChanged: () => {},
                onRemoteStream: (userId, stream) => {
                    if (!audioRefs.current[userId]) {
                        const audio = new Audio();
                        audio.autoplay = true;
                        audioRefs.current[userId] = audio;
                    }
                    audioRefs.current[userId].srcObject = stream;
                    upsertParticipant(userId, {});
                },
                onSpeakingChanged: (userId, speaking) => {
                    upsertParticipant(userId, { speaking });
                },
                onMuteChanged: (userId, selfMuted) => upsertParticipant(userId, { selfMuted }),
                onForceMuteChanged: (userId, forceMuted) => upsertParticipant(userId, { forceMuted }),
            });
            existing.forEach((p) => upsertParticipant(p.userId, {
                selfMuted: p.isSelfMuted, forceMuted: p.isForceMuted, speaking: p.isSpeaking
            }));
            upsertParticipant(currentUserId, {});

            setConnected(true);
        } catch (error) {
            setError("Nepodařilo se připojit k hlasovému chatu: " + error.message);
        }
    };

    const handleLeave = async () => {
        await leaveVoice();
        setConnected(false);
        setParticipants([]);
        Object.values(audioRefs.current).forEach((a) => (a.srcObject = null));
        audioRefs.current = {};
    };

    const toggleMyMute = () => {
        const next = !myMuted;
        setMyMuted(next);
        setSelfMute(next);
    };

    const toggleWhisperMode = () => {
        const next = !whisperMode;
        setWhisperMode(next);
        setWhisperTargets(next ? whisperTargets : null);
    };

    const toggleWhisperTarget = (userId) => {
        const next = whisperTargets.includes(userId)
            ? whisperTargets.filter((id) => id !== userId)
            : [...whisperTargets, userId];
        
            setWhisperTargetsLocal(next);

            if (whisperMode)
                setWhisperTargets(next);
    };

    const nameFor = (userId) => players.find((p) => p.userId === userId)?.characterName || "Hráč";

    useEffect(() => {
        return () => {
            if (connected)
                leaveVoice();
        };
    }, [connected]);

    return (
        <div className="voice-panel">
            {error && <p className="voice-panel__error">{error}</p>}

            {
                !connected ? (
                    <button className="voice-panel__join" onClick={handleJoin}>
                        <IconVolume size={16} /> Připojit se k hlasovému chatu
                    </button>
                ) : (
                    <>
                        <div className="voice-panel__controls">
                            <button className="voice-panel__mute" onClick={toggleMyMute}>
                                {myMuted ? <IconMicrophoneOff size={16} /> : <IconMicrophone size={16} />}
                                {myMuted ? "Zapnout mikrofon" : "Ztišit se"}
                            </button>

                            {
                                isGameMaster && (
                                    <button className="voice-panel__whisper-toggle" onClick={toggleWhisperMode}>
                                        {whisperMode ? "Mluvit na všechny" : "Šeptat vybraným"}
                                    </button>
                                )
                            }

                            <button className="voice-panel__leave" onClick={handleLeave}>
                                <IconPhoneOff size={16} /> Odpojit
                            </button>
                        </div>

                        <ul className="voice-panel__list">
                            {
                                participants.map((p) => (
                                    <li
                                        key={p.userId}
                                        className={`voice-participant ${p.speaking ? "voice-participant--speaking" : ""}`}
                                    >
                                        <span className="voice-participant__name">{nameFor(p.userId)}</span>

                                        {
                                            isGameMaster && whisperMode && (
                                                <label className="voice-participant__whisper-check">
                                                    <input 
                                                        type="checkbox"
                                                        checked={whisperTargets.includes(ú.userId)}
                                                        onChange={() => toggleWhisperTarget(p.userId)}
                                                    />
                                                    slyší
                                                </label>
                                            )
                                        }

                                        {
                                            isGameMaster && (
                                                <button
                                                    className="voice-participant__force-mute"
                                                    onClick={() => forceMute(campaignId, p.userId, !p.forceMuted)}
                                                >
                                                    {p.forceMuted ? "Odmlčet" : "Umlčet"}
                                                </button>
                                            )
                                        }
                                    </li>
                                ))
                            }
                        </ul>
                    </>
                )
            }
        </div>
    );
}