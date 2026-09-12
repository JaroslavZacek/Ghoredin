import { useState } from "react";
import { IconChevronDown } from "@tabler/icons-react";

export default function CollapsibleSection({ title, defaultOpen = true, onDarkBg = false, children }) {
    const [open, setOpen] = useState(defaultOpen);

    return (
        <section className={`collapsible-section ${onDarkBg ? "collapsible-section--on-dark-bg" : ""}`}>
            <div
                className="collapsible-section__header"
                onClick={() => setOpen((prev) => !prev)}
            >
                <h3 className="collapsible-section__title">{title}</h3>
                <IconChevronDown 
                    size={18}
                    className={`collapsible-section__chevron ${open ? "collapsible-section__chevron--open" : ""}`}
                />
            </div>

            {
                open &&
                    <div className="collapsible-section__body">{children}</div>
            }
        </section>
    );
}