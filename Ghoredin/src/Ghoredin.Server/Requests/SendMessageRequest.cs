namespace Ghoredin.Server.Requests
{
    public record SendMessageRequest(
        Guid CampaignId,
        string Content,
        string? WhisperToUserId
    );
}
