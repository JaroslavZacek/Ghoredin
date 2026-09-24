import { getHubConnection } from "../../shared/signalr/campaignHubConnection";

const ICE_SERVERS = [{ urls: "stun:stun.l.google.com:19302"}];

let localStream = null;
let peers = {};
let campaignId = null;
let conn = null;

let isSelfMuted = false;
let isForceMuted = false;

let handlers = {
    onParticipantsChanged: () => {},
    onRemoteStream: () => {},
    onSpeakingChanged: () => {},
    onMuteChanged: () => {},
    onForceMuteChanged: () => {},
};

function createPeerConnection(targetUserId) {
    const pc = new RTCPeerConnection({ iceServers: ICE_SERVERS });

    const clonedTrack = localStream.getAudioTracks()[0].clone();
    pc.addTrack(clonedTrack, localStream);

    pc.onicecandidate = (e) => {
        if (e.candidate) {
            conn.invoke("SendVoiceSignal", campaignId, targetUserId, "ice-candidate", JSON.stringify(e.candidate));  
        }
    };

    pc.ontrack = (e) => {
        handlers.onRemoteStream(targetUserId, e.streams[0]);
    };

    peers[targetUserId] = { connection: pc, clonedTrack };

    return pc;
}

async function createAndSendOffer(targetUserId) {
    const pc = createPeerConnection(targetUserId);
    const offer = await pc.createOffer();

    await pc.setLocalDescription(offer);
    await conn.invoke("SendVoiceSignal", campaignId, targetUserId, "offer", JSON.stringify(offer));
}

async function handleIncomingSignal({ fromUserId, signalType, payload }) {
    const data = JSON.parse(payload);

    if (signalType === "offer") {
        const pc = createPeerConnection(fromUserId);

        await pc.setRemoteDescription(new RTCSessionDescription(data));

        const answer = await pc.createAnswer();

        await pc.setLocalDescription(answer);
        await conn.invoke("SendVoiceSignal", campaignId, fromUserId, "answer", JSON.stringify(answer));
    } else if (signalType === "answer") {
        const pc = peers[fromUserId]?.connection;
        if (pc)
            await pc.setRemoteDescription(new RTCSessionDescription(data));
    } else if (signalType === "ice-candidate") {
        const pc = peers[fromUserId]?.connection;
        if (pc)
            await pc.addIceCandidate(new RTCIceCandidate(data));
    }
}

function startSpeakingDetection() {
    const audioCtx = new AudioContext();
    const source = audioCtx.createMediaStreamSource(localStream);
    const analyser = audioCtx.createAnalyser();

    analyser.fftSize = 512;
    source.connect(analyser);

    const data = new Uint8Array(analyser.frequencyBinCount);
    let wasSpeaking = false;
    let lastSent = 0;

    const check = () => {
        if (!conn)
            return;

        analyser.getByteFrequencyData(data);

        const volume = data.reduce((sum, v) => sum + v, 0) / data.length;
        
        const isSpeaking = !isSelfMuted && !isForceMuted && volume > 10;

        const now = Date.now();
        if (isSpeaking !== wasSpeaking && now - lastSent > 300) {
            wasSpeaking = isSpeaking;
            lastSent = now;
            conn.invoke("SetSpeaking", campaignId, isSpeaking).catch((error) => {console.error("SetSpeaking selhalo: ", error);});
        }
        requestAnimationFrame(check);
    };
    check();
}

export async function joinVoice(newCampaignId, myUserId,newHandlers) {
    campaignId = newCampaignId;
    handlers = { ...handlers, ...newHandlers };

    localStream = await navigator.mediaDevices.getUserMedia({ audio: true });
    conn = await getHubConnection();

    conn.on("ReceiveVoiceSignal", handleIncomingSignal);
    conn.on("VoiceParticipantJoined", ({ userId }) => {
        handlers.onParticipantsChanged();
    });
    conn.on("VoiceParticipantLeft", ({ userId }) => {
        if (peers[userId]) {
            peers[userId].connection.close();
            delete peers[userId];
        }
        handlers.onParticipantsChanged();
    });
    conn.on("VoiceSpeakingChanged", ({ userId, isSpeaking }) => {
        handlers.onSpeakingChanged(userId, isSpeaking);
    });
    conn.on("VoiceMuteChanged", ({ userId, isSelfMuted: muted }) => {
        handlers.onMuteChanged(userId, muted);
    });
    conn.on("VoiceForceMuteChanged", ({ userId, isForceMuted: muted }) => {
        if (userId === myUserIdPlaceholder) {
            isForceMuted = muted;
        }

        handlers.onForceMuteChanged(userId, muted);
    });

    const existingParticipants = await new Promise((resolve) => {
        conn.on("VoiceParticipantsSnapshot", resolve);
        conn.invoke("JoinVoice", campaignId);
    });

    for (const participant of existingParticipants) {
        await createAndSendOffer(participant.userId);
    }

    startSpeakingDetection();
    handlers.onParticipantsChanged();

    return existingParticipants;
}

export async function leaveVoice() {
    if (conn && campaignId) {
        await conn.invoke("LeaveVoice", campaignId).catch(() => {});
        conn.off("ReceiveVoiceSignal", handleIncomingSignal);
    }

    Object.values(peers).forEach((p) => p.connection.close());
    peers = {};
    if (localStream) {
        localStream.getTracks().forEach((t) => t.stop());
        localStream = null;
    }
    campaignId = null;
    isSelfMuted = false;
    isForceMuted = false;
}

export function setSelfMute(muted) {
    isSelfMuted = muted;

    if (localStream) 
        localStream.getAudioTracks().forEach((t) => (t.enabled = !muted));

    conn?.invoke("SetSelfMute", campaignId, muted).catch(() => {});
}

export function setWhisperTargets(targetUserIds) {
    Object.entries(peers).forEach(([userId, peer]) => {
        const shouldHear = !targetUserIds || targetUserIds.includes(userId);
        peer.clonedTrack.enabled = shouldHear;
    });
}

export function forceMute(campaignIdParam, targetUserId, muted) {
    return conn?.invoke("SetForceMute", campaignIdParam, targetUserId, muted);
}