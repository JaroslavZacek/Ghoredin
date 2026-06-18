import { useAuth } from "./features/auth/AuthContext";

import AuthPage from "./features/auth/components/AuthPage";
import CharacterList from "./features/characters/components/CharacterList";

function App() {
  const { user, loading, logout} = useAuth();

  if (loading) {
    return (
      <p style={{ textAlign: "center", marginTop: 48}}>
        Načítání...
      </p>
    );
  }

  if (!user) {
    return <AuthPage />
  }

  return (
    <div style={{ maxWidth: 600, margin: "40px auto", fontFamily: "var(--font-sans)"}}>
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: 24}}>
        <h1 style={{ fontSize: 22, margin: 0 }}>Kronika Ghoredinu</h1>

        <button className="auth-button" onClick={logout}>Odhlásit se</button>
      </div>

      <CharacterList />
    </div>
  );
}

export default App;