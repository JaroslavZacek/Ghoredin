import { apiGet, apiPost } from "../../../shared/api/apiClient";

export const getMyCharacters = () => 
    apiGet("characters");

export const getCharacter = (id) => 
    apiGet(`characters/${id}`);

export const getCampaignCharacters = (campaignId) =>
    apiGet(`characters/campaign/${campaignId}`);

export const createCharacterInCampaign = (campaignId, data) =>
    apiPost(`characters/campaign/${campaignId}`, data);

export const startRolledCharacter = (campaignId, name) =>
    apiPost(`characters/campaign/${campaignId}/start-rolled`, { name });

export const rollAbility = (characterId, abilityName) =>
    apiPost(`characters/${characterId}/roll-ability`, { abilityName });

export const completeRolledCharacter = (characterId) =>
    apiPost(`characters/${characterId}/complete-rolled`, {});