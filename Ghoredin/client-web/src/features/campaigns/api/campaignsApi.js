import { apiGet, apiPost } from "../../../shared/api/apiClient";

export const getMyCampaigns = () =>
    apiGet("campaigns");

export const getAvailableCampaigns = () =>
    apiGet("campaigns/available");

export const createCampaign = (data) =>
    apiPost("campaigns", data);

export const joinCampaign = (campaignId, characterId = null) =>
    apiPost(`campaigns/${campaignId}/join`, { characterId });

export const getCampaign = (id) =>
    apiGet(`campaigns/${id}`);