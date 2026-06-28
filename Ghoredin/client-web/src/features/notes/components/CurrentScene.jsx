import { useState, useEffect } from "react";

import { getMyCurrentScene } from "../api/notesApi";

import "./CurrentScene.css";

function CurrentScene({ campaignId, refreshKey }) {
    const [scene, setScene] = useState(null);
    const [loading, setLoading] = useState(true);

    const loadScene = async () => {
        try {
            const data = await getMyCurrentScene(campaignId);

            setScene(data);
        }
        catch {
            setScene(null);
        }
        finally {
            setLoading(false);
        }
    }
    
    useEffect(() => {
        loadScene();
    }, [campaignId, refreshKey]);

    if (loading) {
        return null;
    }

    if (!scene) {
        return (
            <div className="current-scene current-scene__empty">
                <span>Zatím se nic neodehrává.</span>
            </div>
        );
    }

    return (
        <div className="current-scene">
            <div className="current-scene__label">Právě se odehrává</div>
            <h3 className="current-scene__title">{scene.title}</h3>

            {
                scene.content &&
                    <p className="current-scene__gm-text">{scene.content}</p>
            }

            {
                scene.playerFacingContent &&
                    <p className="current-scene__text">{scene.playerFacingContent}</p>
            }
        </div>
    );
}

export default CurrentScene;