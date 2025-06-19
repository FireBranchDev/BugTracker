import { useAuth0, withAuthenticationRequired } from '@auth0/auth0-react';
import { Button, Container, Stack, TextField, Typography } from '@mui/material';
import { useMutation } from '@tanstack/react-query';
import { useEffect, useState, type ChangeEvent, type FC } from 'react';
import { useNavigate } from 'react-router-dom';
import useUser from '../../hooks/use-user';
import type { ValidationResult } from '../../types/validation-result';
import Error from '../Error';
import Loading from '../Loading';
import ErrorPage from './ErrorPage';

const CreateUserAccountPage: FC = () => {
  const DISPLAY_NAME_MAXIMUM_LENGTH = 100;
  const DISPLAY_NAME_EXCEEDS_MAXIMUM_LENGTH = `Your display name must be less or equal to ${DISPLAY_NAME_MAXIMUM_LENGTH} characters.`;

  const DISPLAY_NAME_MINIMUM_LENGTH = 1;
  const DISPLAY_NAME_LESS_THAN_MINIMUM_LENGTH = `Your display name needs to have at least one character.`;

  const [displayName, setDisplayName] = useState('');

  const [displayNameHasError, setDisplayNameHasError] = useState(false);
  const [displayNameErrorMessage, setDisplayNameErrorMessage] = useState<
    string | null
  >(null);

  const navigate = useNavigate();
  const { getAccessTokenSilently } = useAuth0();

  const createUserAccountMutation = useMutation({
    mutationFn: async (displayName: string) => {
      return await fetch(
        `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/users`,
        {
          method: 'POST',
          headers: {
            Authorization: `Bearer ${await getAccessTokenSilently()}`,
            'Content-Type': 'application/json; charset=utf-8',
          },
          body: JSON.stringify({ displayName }),
        },
      );
    },
  });

  const { hasAccount, isPending, isError } = useUser();
  useEffect(() => {
    if (hasAccount && !isError && !isPending) {
      navigate('/projects');
    }
  }, [hasAccount, isError, isPending]);

  const validateDisplayName = (displayName: string): ValidationResult => {
    const validationResult: ValidationResult = {
      isValid: false,
      isError: false,
      errorMessage: null,
    };

    if (displayName.length > DISPLAY_NAME_MAXIMUM_LENGTH) {
      validationResult.isError = true;
      validationResult.errorMessage = DISPLAY_NAME_EXCEEDS_MAXIMUM_LENGTH;
      return validationResult;
    }

    if (displayName.length < DISPLAY_NAME_MINIMUM_LENGTH) {
      validationResult.isError = true;
      validationResult.errorMessage = DISPLAY_NAME_LESS_THAN_MINIMUM_LENGTH;
      return validationResult;
    }

    validationResult.isValid = true;
    return validationResult;
  };

  const onChangeDisplayNameTextField = (e: ChangeEvent<HTMLInputElement>) => {
    const displayNameValidationResult = validateDisplayName(e.target.value);
    setDisplayNameHasError(displayNameValidationResult.isError);
    setDisplayNameErrorMessage(displayNameValidationResult.errorMessage);
    setDisplayName(e.target.value);
  };

  const onCreateAccountButton = async () => {
    if (!validateDisplayName(displayName)) return;

    const { ok, status } =
      await createUserAccountMutation.mutateAsync(displayName);

    if (ok) {
      navigate('/projects');
      return;
    }

    if (status === 409) {
      navigate('/projects');
      return;
    }

    return <Error />;
  };

  if (isPending) return <Loading />;

  if (isError) return <ErrorPage />;

  return (
    <>
      <Typography variant="h1" textAlign="center">
        Create User Account
      </Typography>
      <Container maxWidth="xs">
        <Stack spacing={2}>
          <TextField
            id="display-name"
            label="Display Name"
            variant="filled"
            onChange={onChangeDisplayNameTextField}
            value={displayName}
            error={displayNameHasError}
            helperText={displayNameErrorMessage}
          />
          <Button variant="contained" onClick={onCreateAccountButton}>
            Create
          </Button>
        </Stack>
      </Container>
    </>
  );
};

export default withAuthenticationRequired(CreateUserAccountPage);
