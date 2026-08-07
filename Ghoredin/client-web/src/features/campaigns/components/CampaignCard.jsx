import { Link } from "react-router-dom";

import { getThemeForGameSystem } from "../../../shared/theme/themeUtils";
import { getIconForTheme } from "../../../shared/theme/themeIcons";
import { creationMethodLabel } from "../utils/campaignHelpers";

import ".CampaignCard.css";

export default function CampaignCard({ campaign }) {
    const theme = getThemeForGameSystem(campaign.gameSystemId);
    const Icon = getIconForTheme(theme);

    return (
        <Link
            to={`/campaigns/${campaign.id}`}
            className="campaign-card"
            data-theme={theme}
        >
            <span className="campaign-card__icon">
                <Icon size={20} />
            </span>

            <span className="campaign-card__info">
                <span className="campaign-card__name">{campaign.name}</span>
                <span className="campaign-card__meta">
                    {campaign.gameSystemId} · {creationMethodLabel(campaign.characterCreation)} ·{" "}
                    {campaign.playerCount}
                    {campaign.maxPlayers != null ? ` / ${campaign.maxPlayers}` : ""} hráčů
                </span>
            </span>
        </Link>
    );
}
