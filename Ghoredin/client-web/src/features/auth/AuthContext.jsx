import { createContext, useContext, useState, useEffect } from "react";

import { login as apiLogin, logout as apiLogout, getMe} from "./api/authApi";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        const checkAuth = async () => {
            try {
                const me = await getMe();

                setUser(me);
            }
            catch {
                setUser(null);
            }
            finally {
                setLoading(false);
            }
        };

        checkAuth();
    }, []);

    const login = async (email, password) => {
        await apiLogin(email, password);

        const me = await getMe();

        setUser(me);
    };

    const logout = async () => {
        try {
            await apiLogin();
        }
        finally {
            setUser(null);
        }
    };

    const value = { user, loading, login, logout };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
    const context = useContext(AuthContext);

    if (context === null) {
        throw new Error("useAuth musí být uvnitř AuthProvider");
    }

    return context;
}