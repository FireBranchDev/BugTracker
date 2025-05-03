import { useAuth0 } from '@auth0/auth0-react';
import { Button, TextField } from '@mui/material';
import { Box } from '@mui/system';
import { FC, useContext, useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router';
import DefaultPageLayout from '../components/default-page-layout';
import PageError from '../components/page-error';
import { AppContext } from '../contexts';

type DisplayNameError = {
  message: string;
};

type PageError = {
  message: string;
};

const SignupPage: FC = () => {
  const [displayName, setDisplayName] = useState('');
  const [displayNameError, setDisplayNameError] =
    useState<DisplayNameError | null>(null);

  const [canSubmit, setCanSubmit] = useState(false);

  const [pageError, setPageError] = useState<PageError | null>(null);

  const navigate = useNavigate();
  const { loginWithRedirect } = useAuth0();

  const { getAccessTokenSilently } = useAuth0();

  const appContext = useContext(AppContext);

  useEffect(() => {
    (async () => {
      try {
        const accessToken = await getAccessTokenSilently();
        const response = await fetch(
          `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/users`,
          {
            headers: {
              Accept: 'application/json',
              'Content-Type': 'application/json',
              Authorization: `Bearer ${accessToken}`,
            },
          }
        );
        if (response.ok) navigate('/');

        if (response.status === 401) {
          await loginWithRedirect({
            openUrl() {
              window.location.replace('/signup');
            },
          });
        }
      } catch (err: unknown) {
        console.log((err as Error).message);
      }
    })();
  }, [getAccessTokenSilently, loginWithRedirect, navigate]);

  const handleDisplayNameChanged = (e: React.ChangeEvent<HTMLInputElement>) => {
    setCanSubmit(e.target.value.length > 0);
    if (e.target.value.length === 0) {
      setDisplayNameError({
        message: 'Display Name is required',
      });
    } else {
      setDisplayNameError(null);
    }

    setDisplayName(e.target.value);
  };

  const location = useLocation();
  const query = location.search.replace('?', '');
  let querySplit: Array<string> = [];
  if (query.includes('&')) {
    querySplit = query.split('&');
  } else {
    querySplit.push(query);
  }

  let redirectUriValue = '';
  querySplit.forEach((value) => {
    const split = value.split('=');
    if (split[0] === 'redirect_uri') {
      redirectUriValue = split[1];
      return;
    }
  });

  const onClickSubmitButton = async (
    e: React.MouseEvent<HTMLButtonElement>
  ) => {
    e.preventDefault();
    const accessToken = await getAccessTokenSilently();
    if (displayName.length === 0) return;

    try {
      const response = await fetch(
        `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/users`,
        {
          method: 'POST',
          headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',
            Authorization: `Bearer ${accessToken}`,
          },
          body: JSON.stringify({ displayName }),
        }
      );
      if (response.ok) {
        appContext.displayName = displayName;
        if (redirectUriValue === '') {
          navigate('/');
          return;
        }
        navigate(redirectUriValue);
      }
      if (response.status === 409) {
        if (redirectUriValue === '') {
          navigate('/');
          return;
        }
        navigate(redirectUriValue);
      }
      if (response.status === 401) {
        await loginWithRedirect({
          openUrl() {
            window.location.replace('/signup');
          },
        });
      }
    } catch (err: unknown) {
      if ((err as Error).message === 'Failed to fetch') {
        setPageError({
          message:
            'Oh no, could not establish a connection. Please try again later.',
        });
      }
    }
  };

  return (
    <>
      <DefaultPageLayout title="Signup">
        {pageError !== null && <PageError message={pageError?.message} />}
        <Box
          component="form"
          noValidate
          autoComplete="off"
          display="flex"
          flexDirection="column"
          alignItems="center"
          rowGap="1vh"
          marginTop="1vh"
        >
          <TextField
            id="display-name"
            label="Display Name"
            variant="standard"
            error={displayNameError !== null}
            helperText={displayNameError?.message}
            value={displayName}
            onChange={handleDisplayNameChanged}
          />
          <Button
            variant="contained"
            disabled={!canSubmit}
            onClick={onClickSubmitButton}
          >
            Submit
          </Button>
        </Box>
      </DefaultPageLayout>
    </>
  );
};

export default SignupPage;
