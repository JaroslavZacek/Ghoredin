using Ghoredin.Domain.Handouts;

namespace Ghoredin.Server.Requests
{
    public record CreateHandoutRequest(
        Guid CampaignId,
        string Title,
        string Content,
        HandoutContentType ContentType,
        HandoutShareMode ShareMode);
}
