import { apiGet, apiPut } from "../../../shared/api/apiClient";

export const getMyJournal = (campaignId) =>
    apiGet(`journal/campaign/${campaignId}`);

export const saveMyJournal = (campaignId, content) =>
    apiPut(`journal/campaign/${campaignId}`, { content });