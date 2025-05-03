import { FC, PropsWithChildren } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import LoadingPage from '../pages/loading-page';

const Wrapper: FC<PropsWithChildren> = ({ children }) => {
  const { isLoading, error } = useAuth0();

  if (isLoading) {
    return <LoadingPage />;
  }

  if (error) return <p>That's not meant to happen. {error.message}</p>;

  return children;
};

export default Wrapper;
