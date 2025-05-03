import { FC } from 'react';

import { Typography } from '@mui/material';
import { Container } from '@mui/system';
import DefaultPageLayout from '../components/default-page-layout';

type Props = {
  message: string;
};

const ErrorPage: FC<Props> = ({ message }) => {
  return (
    <>
      <DefaultPageLayout title="Error">
        <Container maxWidth={false}>
          <Typography variant="h3" textAlign="center">
            {message}
          </Typography>
        </Container>
      </DefaultPageLayout>
    </>
  );
};

export default ErrorPage;
