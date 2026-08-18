namespace Ghoredin.Server.Requests
{
    public record ReorderSiblingsRequest(
        Guid CampaignId,
        Guid? ParentNoteId,
        List<Guid> OrderedNoteIds);
}
