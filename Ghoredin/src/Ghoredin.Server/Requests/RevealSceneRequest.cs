namespace Ghoredin.Server.Requests
{
    public record RevealSceneRequest
    (
        Guid CampaignId,
        Guid NoteId,
        List<string> TargetUserIds
    );
}
