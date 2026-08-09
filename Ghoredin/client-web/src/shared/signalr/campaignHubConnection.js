import * as signalR from "@microsoft/signalr";

let connection = null;
let startPromise = null;

function createConnection() {
    return new signalR.HubConnectionBuilder()
        .withUrl("/hubs/campaign", { withCredentials: true })
        .withAutomaticReconnect()
        .build();
}

export async function getHubConnection() {
    if (!connection) {
        connection = createConnection();
    }

    if (connection.state === signalR.HubConnectionState.Connected) {
        return connection;
    }

    if (!startPromise) {
        startPromise = connection.start().catch((error) => {
            startPromise = null;
            throw error;
        });
    }

    await startPromise;
    return connection;
}

export async function joinCampaignGroup(campaignId) {
    const conn = await getHubConnection();

    await conn.invoke("JoinCampaignGroup", campaignId);
}

export async function leaveCampaignGroup(campaignId) {
    const conn = await getHubConnection();

    await conn.invoke("LeaveCampaignGroup", campaignId);
}