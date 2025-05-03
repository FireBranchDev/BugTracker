import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter, Route, Routes } from 'react-router';
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import './reset.css';
import './index.css';
import {
  ThemeProvider,
  createTheme,
  responsiveFontSizes,
} from '@mui/material/styles';
import * as Pages from './pages';
import Wrapper from './components/wrapper';
import Auth0ProviderWithRedirectCallback from './auth0/auth0-provider-with-redirect-callback';
import ProtectedRoute from './auth0/protected-route';
import UserAccountRequired from './components/user-account-required';

let theme = createTheme();
theme = responsiveFontSizes(theme);

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <BrowserRouter>
      <Auth0ProviderWithRedirectCallback
        domain={import.meta.env.VITE_AUTH0_DOMAIN}
        clientId={import.meta.env.VITE_AUTH0_CLIENT_ID}
        authorizationParams={{
          redirect_uri: window.location.origin,
          audience: import.meta.env.VITE_BUGTRACKER_BACKEND_API_AUDIENCE,
        }}
      >
        <ThemeProvider theme={theme}>
          <Wrapper>
            <Routes>
              <Route path="/" element={<Pages.DashboardPage />} />
              <Route path="/home" element={<Pages.HomePage />} />
              <Route
                path="/projects"
                element={
                  <ProtectedRoute
                    component={() => (
                      <>
                        <UserAccountRequired>
                          <Pages.ProjectsPage />
                        </UserAccountRequired>
                      </>
                    )}
                  />
                }
              />
              <Route
                path="/signup"
                element={<ProtectedRoute component={Pages.SignupPage} />}
              />
              <Route
                path="/profile"
                element={
                  <ProtectedRoute
                    component={() => (
                      <>
                        <UserAccountRequired>
                          <Pages.ProfilePage />
                        </UserAccountRequired>
                      </>
                    )}
                  />
                }
              />
            </Routes>
          </Wrapper>
        </ThemeProvider>
      </Auth0ProviderWithRedirectCallback>
    </BrowserRouter>
  </StrictMode>
);
