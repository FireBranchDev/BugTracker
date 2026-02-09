import { withAuthenticationRequired } from '@auth0/auth0-react';
import { useEffect, type FC } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import useUser from '../../hooks/use-user';
import Loading from '../Loading';
import ErrorPage from '../pages/ErrorPage';

const UserAccountRequiredProtectedRoute: FC = () => {
  const navigate = useNavigate();

  const { hasAccount, isPending, isError } = useUser();

  useEffect(() => {
    if (!isError && !isPending && !hasAccount) navigate('/signup');
  }, [hasAccount, isError, isPending]);

  if (hasAccount) return <Outlet />;

  if (isError) return <ErrorPage />;

  return <Loading />;
};

export default withAuthenticationRequired(UserAccountRequiredProtectedRoute);
