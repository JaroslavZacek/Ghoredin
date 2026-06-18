import { apiGet, apiPost } from "../../../shared/api/apiClient";

export const getMyCharacters = () => 
    apiGet("characters");

export const getCharacter = (id) => 
    apiGet(`characters/${id}`);

export const createCharacter = (data) => 
    apiPost("characters", data);