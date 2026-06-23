const API_URL = import.meta.env.VITE_API_URL;

const handleResponse = async (response) => {
    if(!response.ok) {
        throw new Error(`Odezva sítě nebyla v pořádku: ${response.status} ${response.statusText}`);
    }

    if(response.status === 204) {
        return null;
    }

    const text = await response.text();

    return text ? JSON.parse(text) : null;
};

export const apiGet = async (url, params = {}) => {
    const filteredParams = Object.fromEntries(
        Object.entries(params).filter(
            ([_, value]) => value !== undefined && value !== null && value !== ""
        )
    );

    const queryParams = new URLSearchParams(filteredParams);

    const apiUrl = queryParams.toString()
        ? `${API_URL}${url}?${queryParams}`
        : `${API_URL}${url}` ;

    const response = await fetch(apiUrl, {
        credentials: "include"
    });

    return handleResponse(response);
}

export const apiPost = async (url, body) => {
    const response = await fetch(`${API_URL}${url}`, {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
    });

    return handleResponse(response);
}

export const apiPut = async (url, body) => {
    const response = await fetch(`${API_URL}{url}`, {
        method: "PUT",
        credentials: "include",
        headers: { "Content-Type": "application/json"},
        body: JSON.stringify(body)
    });

    return handleResponse(response);
}

export const apiPatch = async (url, body) => {
    const response = await fetch(`${API_URL}${url}`, {
        method: "PATCH",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(body)
    });

    return handleResponse(response);
}

export const apiDelete = async (url, body) => {
    const response = await fetch(`${API_URL}${url}`, {
        method: "DELETE",
        credentials: "include",
        headers: { "Content-Type": "application/json" },
        ...(body !== undefined && { body: JSON.stringify(body) })
    });

    return handleResponse(response);
}