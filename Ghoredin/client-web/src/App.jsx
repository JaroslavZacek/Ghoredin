import { Routes, Route, Link, Navigate } from "react-router-dom";

import { useAuth } from "./features/auth/AuthContext";

import AuthPage from "./features/auth/components/AuthPage";
import CharacterList from "./features/characters/components/CharacterList";
import CampaignList from "./features/campaigns/components/CampaignList";

import "./App.css"

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
    <div className="app">
      <header className="app_header">
        <h1 className="app__title">Kronika Ghoredinu</h1>

        <nav className="app__nav">
          <Link className="app__nav-link">Postavy</Link>
          <Link className="app__nav-link">Dobrodružství</Link>

          <button className="auth-button">Odhlásit se</button>
        </nav>
      </header>

      <main className="app__main">
        <Routes>
          <Route path="/characters" element={<CharacterList />} />
          <Route path="/campaigns" element={<CampaignList />} />
          {/* Výchozí adresa -> přesměrování na postavy*/}
          <Route path="*" element={<Navigate to="/characters" replace />} /> 
        </Routes>
      </main>
    </div>
  );
}

export default App;