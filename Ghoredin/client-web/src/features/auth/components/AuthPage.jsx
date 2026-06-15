import { useState } from "react";

import { register, login, getMe } from "../api/authApi";

import "./AuthPage.css"

function AuthPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");
    const [currentUser, setCurrentUser] = useState(null);

    /*-------------------------------------------------------------*/
    /*---------------------Handle funkce---------------------------*/
    /*-------------------------------------------------------------*/

    const handleRegister = async () => {

        setMessage("");

        try {
            await register(email, password);

            setMessage("Registrace proběhla úspěšně. Ted se můžeš přihlásit.");
        }
        catch (error) {
            setMessage("Chyba při registraci: " + error.message);
        }
    };

    const handleLogin = async () => {
        
        setMessage("");

        try {
            await login(email, password);

            const me = await getMe();

            setCurrentUser(me);

            setMessage("Přihlášení úspěšné.");
        }
        catch (error) {
            setCurrentUser(null);

            setMessage("Chyba při přihlášení: " + error.message);
        }
    };

    const handleCheckMe = async () => {

        setMessage("");

        try {
            const me = await getMe();

            setCurrentUser(me);
        }
        catch (error) {
            setCurrentUser(null);

            setMessage("Nejsi přihlášený: " + error.message);
        }
    };

    return (
        <div className="auth-page">

            <h2 className="auth-page__title">Kronika Ghoredinu</h2>

            <div className="auth-form">

                <input
                    className="auth-form__input"
                    type="email"
                    placeholder="E-mail"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)} 
                />

                <input
                    className="auth-form__input" 
                    type="password"
                    placeholder="Heslo"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)} 
                />

                <div className="auth-form__row">

                    <button className="auth-button" onClick={handleRegister}>
                        Registrovat
                    </button>

                    <button className="auth-button" onClick={handleLogin}>
                        Přihlásit
                    </button>

                </div>

                <button
                    className="auth-button auth-button--secondary"
                    onClick={handleCheckMe}
                >
                    Zjistit, kdo jsem
                </button>

                {
                    message && 
                        
                        <p className="auth-message">{message}</p>
                }

                {
                    currentUser && (
                        <div className="auth-result">

                            <strong>Přihlášený uživatel: </strong>

                            <pre>{JSON.stringify(currentUser, null, 2)}</pre>

                        </div>
                    )
                }

            </div>
        </div>
    );
}

export default AuthPage;