import { useQuery } from '@tanstack/react-query';
import { isAxiosError } from 'axios';
import { useEffect, useState } from 'react';
import { useAxios } from '../components/AxiosProvider/AxiosProvider';

const useUser = () => {
  const [hasAccount, setHasAccount] = useState<boolean | undefined>();

  const axios = useAxios();

  const userQuery = async () => {
    const { data } = await axios.get('/users');
    return data;
  };

  const { isPending, isError, error, data } = useQuery({
    queryKey: ['user'],
    queryFn: userQuery,
  });

  useEffect(() => {
    if (data) setHasAccount(true);

    if (isError && isAxiosError(error)) {
      if (error.status === 404) {
        setHasAccount(false);
      }
    }
  }, [data, isError, error]);

  return { hasAccount, isPending, isError, error };
};

export default useUser;
