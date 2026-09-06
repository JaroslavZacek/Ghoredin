import { apiGet, apiPost, apiPut, apiDelete} from "../../../shared/api/apiClient";

export const getHandouts = (campaignId) =>
    apiGet(`handouts/campaign/${campaignId}`);

export const createHandout = (data) =>
    apiPost("handouts", data);

export const updateHandout = (id, data) =>
    apiPut(`handouts/${id}`, data);

export const shareHandout = (id) =>
    apiPost(`handouts/${id}/share`, {});

export const unshareHandout = (id) =>
    apiPost(`handouts/${id}/unshare`, {});

export const deleteHandout = (id) =>
    apiDelete(`handouts/${id}`);