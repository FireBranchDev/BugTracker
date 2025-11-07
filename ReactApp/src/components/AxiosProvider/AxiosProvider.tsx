import { useAuth0 } from '@auth0/auth0-react';
import axios, { type AxiosInstance } from 'axios';
import { createContext, useContext, type JSX } from 'react';

type Props = {
  children: JSX.Element;
};

export type AxiosContextType = AxiosInstance;

export const AxiosContext = createContext<AxiosContextType>(
  axios.create({
    baseURL: `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api`,
  }),
);

export const useAxios = () => useContext(AxiosContext);

const AxiosProvider = ({ children }: Props) => {
  let token: string | null = null;
  const { getAccessTokenSilently } = useAuth0();
  const axios = useAxios();
  axios.interceptors.request.use(async (config) => {
    token = await getAccessTokenSilently();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  });

  return (
    <AxiosContext.Provider value={axios}>{children}</AxiosContext.Provider>
  );
};

export default AxiosProvider;
