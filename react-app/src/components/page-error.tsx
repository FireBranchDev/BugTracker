import { Typography } from '@mui/material';
import { Container } from '@mui/system';

type PageErrorProps = {
  message: string | undefined;
};

const PageError = ({ message }: PageErrorProps) => {
  if (message === undefined || message === '')
    message = 'Oh no, an error has occurred. Please try again later.';
  return (
    <Container maxWidth="sm">
      <Typography variant="h5" color="red">
        Error
      </Typography>
      <Typography variant="h5">{message}</Typography>
    </Container>
  );
};

export default PageError;
