import { FC } from 'react';
import { useAuth0 } from '@auth0/auth0-react';
import * as material from '@mui/material';

const LoginButton: FC = () => {
  const { isAuthenticated, loginWithRedirect } = useAuth0();

  return (
    !isAuthenticated && (
      <material.Button
        onClick={() => loginWithRedirect()}
        style={{ color: 'white' }}
      >
        Log in
      </material.Button>
    )
  );
};

export default LoginButton;
