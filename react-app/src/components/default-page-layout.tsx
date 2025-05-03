import { FC, PropsWithChildren } from 'react';
import DisplayAppBar from '../components/display-app-bar';
import { Container } from '@mui/system';
import { Typography } from '@mui/material';

type Props = {
  title: string;
};

const DefaultPageLayout: FC<PropsWithChildren<Props>> = ({
  title,
  children,
}) => {
  return (
    <>
      <DisplayAppBar />
      <Container maxWidth={false}>
        <Typography variant="h1" textAlign="center">
          {title}
        </Typography>
        {children}
      </Container>
    </>
  );
};

export default DefaultPageLayout;
