import { Link } from "react-router-dom";

import { getThemeForGameSystem } from "../../../shared/theme/themeUtils";
import { getIconForTheme } from "../../../shared/theme/themeIcons";
import { creationMethodLabel } from "../utils/campaignHelpers";

import "./CampaignCard.css";

export default function CampaignCard({ campaign, action }) {
    const theme = getThemeForGameSystem(campaign.gameSystemId);
    const Icon = getIconForTheme(theme);

    const content = (
        <>
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
            {
                action &&
                    <span className="campaign-card__action">{action}</span>
            }
        </>
    );

    if (action) {
        return (
            <div className="campaign-card" data-theme={theme}>
                {content}
            </div>
        );
    }

    return (
        <Link
            to={`/campaigns/${campaign.id}`}
            className="campaign-card"
            data-theme={theme}
        >
            {content}
        </Link>
    );
}
