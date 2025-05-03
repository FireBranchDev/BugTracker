import { useAuth0 } from '@auth0/auth0-react';
import * as material from '@mui/material';

function LogoutButton() {
  const { isAuthenticated, logout } = useAuth0();

  return (
    isAuthenticated && (
      <material.Button
        variant="contained"
        onClick={() => {
          logout({
            logoutParams: {
              returnTo: window.location.origin,
            },
          });
        }}
        size="small"
      >
        Log out
      </material.Button>
    )
  );
}

export default LogoutButton;
