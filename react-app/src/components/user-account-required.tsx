import { FC, PropsWithChildren, ReactNode, useEffect } from 'react';
import useHasUserAccount from '../hooks/use-has-user-account';
import { useNavigate } from 'react-router';
import ErrorPage from '../pages/error-page';
import LoadingPage from '../pages/loading-page';
import { useLocation } from 'react-router';

const UserAccountRequired: FC<PropsWithChildren> = ({
  children,
}): ReactNode => {
  const { isLoading, error, hasUserAccount } = useHasUserAccount();
  const navigate = useNavigate();
  const location = useLocation();

  useEffect(() => {
    if (!hasUserAccount && !isLoading) {
      navigate(`/signup?redirect_uri=${location.pathname}`, {
        replace: true,
      });
    }
  }, [navigate, location, isLoading, hasUserAccount]);

  if (isLoading) {
    return <LoadingPage />;
  }

  if (error) {
    return <ErrorPage message={error.message} />;
  }

  return children;
};

export default UserAccountRequired;
