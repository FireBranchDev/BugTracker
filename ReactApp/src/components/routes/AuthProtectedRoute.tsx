import { useAuth0 } from '@auth0/auth0-react';
import type { FC } from 'react';
import { Outlet } from 'react-router-dom';
import Loading from '../Loading';

const AuthProtectedRoute: FC = () => {
  const { isLoading } = useAuth0();

  if (isLoading) return <Loading />;

  return <Outlet />;
};

export default AuthProtectedRoute;
