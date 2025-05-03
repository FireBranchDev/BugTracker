import { useAuth0 } from '@auth0/auth0-react';
import { FC } from 'react';
import AppBar from './app-bar';
import AuthAppBar from './auth-app-bar';

const DisplayAppBar: FC = () => {
  const { isLoading, isAuthenticated } = useAuth0();

  if (isLoading) {
    return <AppBar />;
  }

  if (isAuthenticated) {
    return <AuthAppBar />;
  }

  return <AppBar />;
};

export default DisplayAppBar;
