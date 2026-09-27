import { apiGet, apiPost, apiPut } from "../../../shared/api/apiClient";

export const register = (email, password) => 
    apiPost("register", { email, password });

export const login = (email, password) =>
    apiPost("login?useCookies=true", { email, password });

export const logout = () =>
    apiPost("auth/logout", {});

export const getMe = () => apiGet("me");

export const setNickname = (nickname) =>
    apiPut("me/nickname", { nickname });