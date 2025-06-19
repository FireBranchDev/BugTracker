import { useAuth0 } from '@auth0/auth0-react';
import { useQuery } from '@tanstack/react-query';
import { useEffect, useState } from 'react';

const useUser = () => {
  const [hasAccount, setHasAccount] = useState<boolean | undefined>();

  const { getAccessTokenSilently } = useAuth0();

  const { isPending, isError, data, error } = useQuery({
    queryKey: ['user-account'],
    queryFn: async () => {
      const accessToken = await getAccessTokenSilently();

      const response = await fetch(
        `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/users`,
        {
          headers: {
            authorization: `Bearer ${accessToken}`,
            'content-type': 'application/json',
          },
        },
      );

      if (response.status === 404) return false;

      if (response.ok) return true;
    },
  });

  useEffect(() => {
    setHasAccount(data);
  }, [data]);

  return { hasAccount, isPending, isError, error };
};

export default useUser;
