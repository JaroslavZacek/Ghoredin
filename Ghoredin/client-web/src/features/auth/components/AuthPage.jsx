import { useState } from "react";

import { register, login, getMe } from "../api/authApi";

function AuthPage() {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");
    const [currentUser, setCurrentUser] = useState(null);

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

            setMessage("Nejsi přihlášený: " + error.message);
        }
    };

    return (
        <>
        </>
    );
}

export default AuthPage;