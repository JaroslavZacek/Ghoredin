import { apiGet, apiPost, apiPut } from "../../../shared/api/apiClient";

export const getCampaignNotes = (campaignId) =>
    apiGet(`notes/campaign/${campaignId}`);

export const createNote = (data) =>
    apiPost("notes", data);

export const updateNote = (id, data) =>
    apiPut(`notes/${id}`, data);

export const revealScene = (data) =>
    apiPost("notes/reveal", data);

export const getMyCurrentScene = (campaignId) =>
    apiGet(`notes/campaign/${campaignId}/my-current-scene`);

export const moveNote = (noteId, newParentNoteId) =>
    apiPost(`notes/${noteId}/move`, { newParentNoteId });

export const reorderSiblings = (campaignId, parentNoteId, orderedNoteIds) =>
    apiPost("notes/reorder", { campaignId, parentNoteId, orderedNoteIds });