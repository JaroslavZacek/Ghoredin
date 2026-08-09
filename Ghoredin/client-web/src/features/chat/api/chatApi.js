import { apiGet, apiPost } from "../../../shared/api/apiClient";

export const getChatHistory = (campaignId) =>
    apiGet(`chat/campaign/${campaignId}`);

export const sendMessage = (campaignId, content, whisperToUserId = null) =>
    apiPost("chat", { campaignId, content, whisperToUserId });