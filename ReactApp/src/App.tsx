import { createTheme, responsiveFontSizes, ThemeProvider } from '@mui/material';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Outlet } from 'react-router-dom';
import ButtonAppBar from './components/app-bars/ButtonAppBar.tsx';
import Auth0ProviderWithRedirectCallback from './components/Auth0ProviderWithRedirectCallback.tsx';
import AxiosProvider from './components/AxiosProvider/AxiosProvider.tsx';

let theme = createTheme();
theme = responsiveFontSizes(theme);

const queryClient = new QueryClient({
  defaultOptions: { queries: { staleTime: 5000 } },
});

function App() {
  return (
    <Auth0ProviderWithRedirectCallback
      domain={import.meta.env.VITE_AUTH0_DOMAIN}
      clientId={import.meta.env.VITE_AUTH0_CLIENT_ID}
      authorizationParams={{
        redirect_uri: window.location.origin,
        audience: import.meta.env.VITE_BUGTRACKER_BACKEND_API_AUDIENCE,
      }}
    >
      <QueryClientProvider client={queryClient}>
        <ThemeProvider theme={theme}>
          <AxiosProvider>
            <>
              <ButtonAppBar />
              <Outlet />
            </>
          </AxiosProvider>
        </ThemeProvider>
      </QueryClientProvider>
    </Auth0ProviderWithRedirectCallback>
  );
}

export default App;
