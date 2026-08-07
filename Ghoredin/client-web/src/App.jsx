import { Routes, Route, Link, Navigate } from "react-router-dom";

import { useAuth } from "./features/auth/AuthContext";
import { useTheme } from "./shared/theme/ThemeContext";

import AuthPage from "./features/auth/components/AuthPage";
import CharacterList from "./features/characters/components/CharacterList";
import CampaignList from "./features/campaigns/components/CampaignList";
import AvailableCampaigns from "./features/campaigns/components/AvailableCampaigns";
import CampaignDetail from "./features/campaigns/components/CampaignDetail";
import CharacterCreationPage from "./features/characters/components/CharacterCreationPage";
import CharacterSheetPage from "./features/characters/components/CharacterSheetPage";
import CampaignCreationPage from "./features/campaigns/components/CampaignCreationPage";

import "./App.css"

function App() {
  const { user, loading, logout} = useAuth();
  const { activeTheme } = useTheme();

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
    <div className="app" data-theme={activeTheme}>
      <header className="app__header">
        <div className="app__header-col app__header-col--start">
          <h1 className="app__title">Kronika Ghoredinu</h1>
        </div>

        <nav className="app__header-col app__header-col--center">
          <Link className="app__nav-link" to="/characters">Postavy</Link>
          <Link className="app__nav-link" to="/campaigns">Dobrodružství</Link>
          <Link className="app__nav-link" to="/campaigns/available">Najít dobrodružství</Link>
        </nav>

        <div className="app__header-col app__header-col--end">
          <div className="app__user">
            <span className="app__user-email">{user.email}</span>
            <button className="app__user-logout-btn" onClick={logout}>Odhlásit se</button>
          </div>
        </div>

      </header>

      <div className="app__inner">
        <main className="app__main">
          <Routes>
            <Route path="/characters" element={<CharacterList />} />
            <Route path="/campaigns" element={<CampaignList />} />
            <Route path="/campaigns/create" element={<CampaignCreationPage />} />
            <Route path="/campaigns/available" element={<AvailableCampaigns />} />
            <Route path="/campaigns/:id" element={<CampaignDetail />} />
            <Route path="/campaigns/:id/create-character" element={<CharacterCreationPage />} />
            <Route path="/campaigns/:id/characters/:characterId" element={<CharacterSheetPage />} />
            {/* Výchozí adresa -> přesměrování na postavy*/}
            <Route path="*" element={<Navigate to="/characters" replace />} />
          </Routes>
        </main>
      </div>
    </div>
  );
}

export default App;