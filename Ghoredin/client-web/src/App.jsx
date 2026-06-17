import { useAuth } from "./features/auth/AuthContext";
import AuthPage from "./features/auth/components/AuthPage";

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
      <h1>Kronika Ghoredinu</h1>

      <p>Přihlášen jako: {user.userId}</p>

      <button className="auth-button" onClick={logout}>Odhlásit se</button>
    </div>
  );
}

export default App;