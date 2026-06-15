import { apiGet, apiPost } from "../../../shared/api/apiClient";

export const register = (email, password) => 
    apiPost("register", { email, password });

export const login = (email, password) =>
    apiPost("login?useCookies=true", { email, password });

export const getMe = () => apiGet("me");