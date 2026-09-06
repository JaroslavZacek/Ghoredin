import "./HandoutForm.css";

export default function HandoutForm({
    title, setTitle,
    content, setContent,
    contentType, setContentType,
    shareMode, setShareMode,
    isNew, onSave, onCancel
}) {
    return (
        <div className="handout-form">
            <input
                className="handout-form__input"
                placeholder="Název listiny"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                
            />
            <textarea
                className="handout-form__input"
                placeholder={contentType === "ImageUrl" ? "URL obrázku..." : "Obsah listiny..."}
                value={content}
                onChange={(e) => setContent(e.target.value)}
                rows={3}
            />

            {
                isNew && (
                    <>
                        <div className="handout-form__row">
                            <label>
                                <input
                                    type="radio"
                                    checked={contentType === "Text"}
                                    onChange={() => setContentType("Text")}
                                /> 
                                
                                Text
                            </label>

                            <label>
                                <input
                                    type="radio"
                                    checked={contentType === "ImageUrl"}
                                    onChange={() => setContentType("ImageUrl")}
                                />

                                Obrázek (URL)
                            </label>
                        </div>

                        <div className="handout-form__row">
                            <label>
                                <input
                                    type="radio"
                                    checked={shareMode === "Live"}
                                    onChange={() => setShareMode("Live")}
                                />

                                Živá (Hráči vidí aktuální verzi)
                            </label>

                            <label>
                                <input
                                    type="radio"
                                    checked={shareMode === "Snapshot"}
                                    onChange={() => setShareMode("Snapshot")}
                                />

                                Snímek (Zamrzne v okamžiku sdílení)
                            </label>
                        </div>
                    </>
                )
            }

            <div className="handout-form__actions">
                <button className="handout-form__save" onClick={onSave}>Uložit</button>
                <button className="handout-form__cancel" onClick={onCancel}>Zrušit</button>
            </div>
        </div>
    );
}