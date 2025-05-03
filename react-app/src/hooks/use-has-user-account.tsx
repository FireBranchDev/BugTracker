import { useAuth0 } from '@auth0/auth0-react';
import { useState, useEffect, useContext } from 'react';
import { AppContext } from '../contexts';

type UserJsonResponse = {
  displayName: string;
};

const useHasUserAccount = () => {
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);
  const [hasUserAccount, setHasUserAccount] = useState<boolean | null>(null);
  const { getAccessTokenSilently } = useAuth0();

  const appContext = useContext(AppContext);

  useEffect(() => {
    (async () => {
      const accessToken = await getAccessTokenSilently();
      try {
        const response = await fetch(
          `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/users`,
          {
            headers: {
              Authorization: `Bearer ${accessToken}`,
            },
          }
        );

        if (response.ok) {
          const json = (await response.json()) as UserJsonResponse;
          appContext.displayName = json.displayName;
          setHasUserAccount(true);
        }

        if (response.status === 404) {
          setHasUserAccount(false);
        }

        if (response.status === 500) {
          setError(new Error('Backend API encountered a server error'));
        }
      } catch (err: unknown) {
        if ((err as Error).name === 'TypeError') {
          if ((err as Error).message === 'Failed to fetch') {
            setError(new Error('Cannot establish a connection'));
          }
        }
        setError(err as Error);
      } finally {
        setIsLoading(false);
      }
    })();
  }, [appContext, getAccessTokenSilently]);

  return { isLoading, error, hasUserAccount };
};

export default useHasUserAccount;
