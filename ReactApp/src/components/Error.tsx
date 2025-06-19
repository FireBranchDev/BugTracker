import { Button, Container, Typography } from '@mui/material';
import type { FC } from 'react';
import { useNavigate } from 'react-router';

const Error: FC = () => {
  const navigate = useNavigate();

  return (
    <Container sx={{ mt: 8, textAlign: 'center' }}>
      <Typography variant="body1">
        Oh no, Something unexpected has happened.
      </Typography>
      <Button onClick={() => navigate(0)} variant="contained" sx={{ mt: 0.5 }}>
        Retry?
      </Button>
    </Container>
  );
};

export default Error;
