import {
  AppState,
  Auth0Provider,
  Auth0ProviderOptions,
} from '@auth0/auth0-react';
import { ReactNode } from 'react';
import { useNavigate } from 'react-router';

interface IAuth0ProviderWithRedirectCallback extends Auth0ProviderOptions {
  children?: ReactNode;
}

const Auth0ProviderWithRedirectCallback = ({
  children,
  clientId,
  domain,
  authorizationParams,
}: IAuth0ProviderWithRedirectCallback) => {
  const navigate = useNavigate();
  const onRedirectCallback = async (appState?: AppState) => {
    navigate((appState && appState.returnTo) || window.location.pathname);
  };

  return (
    <Auth0Provider
      clientId={clientId}
      domain={domain}
      onRedirectCallback={onRedirectCallback}
      authorizationParams={authorizationParams}
    >
      {children}
    </Auth0Provider>
  );
};

export default Auth0ProviderWithRedirectCallback;
