import * as signalR from "@microsoft/signalr";

let connection = null;

export async function getHubConnection() {
    if (connection && connection.state === signalR.HubConnectionState.Connected) {
        return connection;
    }

    if (!connection) {
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/hubs/campaign", { withCredentials: true })
            .withAutomaticReconnect()
            .build();
    }

    if (connection.state === signalR.HubConnectionState.Disconnected) {
        await connection.start();
    }

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