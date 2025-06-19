import { withAuthenticationRequired } from '@auth0/auth0-react';
import { useEffect, type FC } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import { CREATE_USER_ACCOUNT_PAGE_PATH } from '../../constants';
import useUser from '../../hooks/use-user';
import Loading from '../Loading';
import ErrorPage from '../pages/ErrorPage';

const UserAccountRequiredProtectedRoute: FC = () => {
  const navigate = useNavigate();

  const { hasAccount, isPending, isError } = useUser();

  useEffect(() => {
    if (!isError && !isPending && hasAccount === false)
      navigate('/' + CREATE_USER_ACCOUNT_PAGE_PATH);
  }, [hasAccount, isError, isPending]);

  if (isPending) return <Loading />;

  if (isError) return <ErrorPage />;

  return <Outlet />;
};

export default withAuthenticationRequired(UserAccountRequiredProtectedRoute);
