import {
  Button,
  Container,
  styled,
  TextField,
  Typography,
} from '@mui/material';
import Box from '@mui/material/Box';
import { useMutation, useQuery } from '@tanstack/react-query';
import { isAxiosError } from 'axios';
import { useEffect } from 'react';
import { useForm, type SubmitHandler } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import image from '../assets/images/zetong-li-rXXSIr8-f9w-unsplash.jpg';
import { useAxios } from '../components/AxiosProvider/AxiosProvider';

type FormInputs = {
  displayName: string;
};

const lightGrayishBlue = '#E0E3E7';

const CssTextField = styled(TextField)({
  '& label.Mui-focused': {
    color: lightGrayishBlue,
  },
  '& .MuiInput-underline:after': {
    borderBottomColor: lightGrayishBlue,
  },
  '& .MuiOutlinedInput-root': {
    '& fieldset': {
      borderColor: lightGrayishBlue,
    },
    '&:hover fieldset': {
      borderColor: lightGrayishBlue,
    },
    '&.Mui-focused fieldset': {
      borderColor: lightGrayishBlue,
    },
  },
  '.MuiInputBase-input': {
    color: lightGrayishBlue,
  },
  '.MuiInputLabel-root': {
    color: lightGrayishBlue,
  },
});

const CssButton = styled(Button)({
  '&.MuiButton-loading': {
    backgroundColor: lightGrayishBlue,
  },
  '.MuiButton-loadingIndicator': {
    color: 'black',
  },
});

type NewUser = {
  displayName: string;
};

type Error = {
  message: string;
};

const Error = ({ message }: Error) => {
  return (
    <Typography variant="body1" sx={{ color: 'red' }}>
      {message}
    </Typography>
  );
};

const SignupPage = () => {
  const axios = useAxios();
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<FormInputs>();

  const mutation = useMutation({
    mutationFn: (newUser: NewUser) => {
      return axios.post('/users', newUser);
    },
  });

  const query = useQuery({
    queryKey: ['user'],
    queryFn: () => {
      return axios.get('/users');
    },
  });

  const navigate = useNavigate();

  useEffect(() => {
    if (query.isSuccess) navigate('/projects');
  }, [query.isSuccess]);

  const onSubmit: SubmitHandler<FormInputs> = async (data) => {
    try {
      await mutation.mutateAsync({ displayName: data.displayName });
      navigate('/projects');
    } catch (error) {
      if (isAxiosError(error)) {
        if (error.status && error.status === 409) {
          navigate('/projects');
        }
      }
    }
  };

  const getError = () => {
    if (mutation.isError) return <Error message={mutation.error.message} />;

    if (query.isError) return <Error message={query.error.message} />;

    return null;
  };

  return (
    <Box
      sx={{
        flexGrow: 1,
        backgroundImage: `url(${image})`,
        backgroundRepeat: 'no-repeat',
        backgroundPosition: 'center',
        backgroundSize: 'cover',
      }}
    >
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          height: '100%',
        }}
      >
        <Container fixed sx={{ mb: 10 }}>
          <Box sx={{ px: 45 }}>
            <Box
              sx={{
                bgcolor: '#292929',
                color: 'white',
                textAlign: 'center',
                padding: 4,
                borderRadius: 5,
              }}
            >
              {getError()}

              <Typography variant="h5" component="h1">
                Create your BugTracker account
              </Typography>
              <Box
                component="form"
                noValidate
                autoComplete="off"
                onSubmit={handleSubmit(onSubmit)}
              >
                <CssTextField
                  {...register('displayName', {
                    required: 'This field is required',
                    maxLength: {
                      value: 50,
                      message: 'This field must not exceed 50 characters',
                    },
                  })}
                  id="display-name-input"
                  label="Display name"
                  fullWidth
                  sx={{ mt: 2 }}
                  error={errors.displayName !== undefined}
                  helperText={
                    errors.displayName?.message !== undefined &&
                    errors.displayName.message
                  }
                />
                <CssButton
                  variant="contained"
                  sx={{ mt: 2 }}
                  size="large"
                  fullWidth
                  loading={mutation.isPending}
                  type="submit"
                >
                  Submit
                </CssButton>
              </Box>
            </Box>
          </Box>
        </Container>
      </Box>
    </Box>
  );
};

export default SignupPage;
