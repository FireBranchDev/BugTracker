import { createContext } from 'react';

type AppContext = {
  displayName: string | null;
};

export const AppContext = createContext<AppContext>({
  displayName: null,
});
