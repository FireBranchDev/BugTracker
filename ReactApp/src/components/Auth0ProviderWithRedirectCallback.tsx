import {
  Auth0Provider,
  type AppState,
  type Auth0ProviderOptions,
} from '@auth0/auth0-react';
import type { FC, PropsWithChildren } from 'react';
import { useNavigate } from 'react-router-dom';

const Auth0ProviderWithRedirectCallback: FC<
  PropsWithChildren & Auth0ProviderOptions
> = ({ children, ...props }) => {
  const navigate = useNavigate();

  const onRedirectCallback = (appState?: AppState) => {
    navigate((appState && appState.returnTo) || window.location.pathname);
  };

  return (
    <Auth0Provider onRedirectCallback={onRedirectCallback} {...props}>
      {children}
    </Auth0Provider>
  );
};

export default Auth0ProviderWithRedirectCallback;
