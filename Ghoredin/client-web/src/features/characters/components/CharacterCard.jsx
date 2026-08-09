import { Link } from "react-router-dom";

import { getThemeForGameSystem } from "../../../shared/theme/themeUtils";
import { getIconForTheme } from "../../../shared/theme/themeIcons";

import "./CharacterCard.css";

export default function CharacterCard({ character }) {
    const theme = getThemeForGameSystem(character.gameSystemId);
    const Icon = getIconForTheme(theme);

    const content = (
        <>
            <span className="character-card__icon">
                <Icon size={20} />
            </span>

            <span className="character-card__name">{character.name}</span>

            <span className="character-card__meta-wrap">
                {
                    character.campaignName && (
                        <span className="character-card__meta">
                            {character.campaignName} ({character.gameSystemId})
                        </span>
                    )
                }

                {
                    character.campaignName && !character.campaignActive && (
                        <span className="character-card__left-badge">
                            Opustil jsi toto dobrodružství
                        </span>
                    )
                }
            </span>
        </>
    );

    return (
        <Link
            to={`/campaigns/${character.campaignId}/characters/${character.id}`}
            className="character-card"
            data-theme={theme}
        >
            {content}
        </Link>
    );
}