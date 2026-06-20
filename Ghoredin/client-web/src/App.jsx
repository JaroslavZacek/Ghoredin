import { Routes, Route, Link, Navigate } from "react-router-dom";

import { useAuth } from "./features/auth/AuthContext";

import AuthPage from "./features/auth/components/AuthPage";
import CharacterList from "./features/characters/components/CharacterList";
import CampaignList from "./features/campaigns/components/CampaignList";
import AvailableCampaigns from "./features/campaigns/components/AvailableCampaigns";

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
          <Link className="app__nav-link" to="/characters">Postavy</Link>
          <Link className="app__nav-link" to="/campaigns">Dobrodružství</Link>
          <Link className="app__nav-link" to="/campaigns/available">Najít dobrodružství</Link>

          <button className="auth-button" onClick={logout}>Odhlásit se</button>
        </nav>
      </header>

      <main className="app__main">
        <Routes>
          <Route path="/characters" element={<CharacterList />} />
          <Route path="/campaigns" element={<CampaignList />} />
          <Route path="/campaigns/available" element={<AvailableCampaigns />} />
          {/* Výchozí adresa -> přesměrování na postavy*/}
          <Route path="*" element={<Navigate to="/characters" replace />} /> 
        </Routes>
      </main>
    </div>
  );
}

export default App;