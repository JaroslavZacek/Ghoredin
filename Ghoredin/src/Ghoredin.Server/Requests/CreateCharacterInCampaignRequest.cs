namespace Ghoredin.Server.Requests
{
    public record CreateCharacterInCampaignRequest
    (
        string Name,
        Dictionary<string, object>? SheetData
    );
}
