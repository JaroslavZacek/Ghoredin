import { apiGet, apiPost } from "../../../shared/api/apiClient";

export const getMyCharacters = () => 
    apiGet("characters");

export const getCharacter = (id) => 
    apiGet(`characters/${id}`);

export const createCharacter = (data) => 
    apiPost("characters", data);

export const getCampaignCharacters = (campaignId) =>
    apiGet(`characters/campaign/${campaignId}`);

export const createCharacterInCampaign = (campaignId, data) =>
    apiPost(`characters/campaign/${campaignId}`, data);