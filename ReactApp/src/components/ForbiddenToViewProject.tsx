import { Box, Typography } from '@mui/material';
import type { FC } from 'react';
import GoToProjectsButton from './buttons/GoToProjectsButton';

const ForbiddenToViewProject: FC = () => {
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        maxWidth: 'md',
        justifySelf: 'center',
      }}
    >
      <Typography variant="h3" textAlign="center" sx={{ mt: 2 }}>
        You are forbidden to view this project
      </Typography>
      <GoToProjectsButton />
    </Box>
  );
};

export default ForbiddenToViewProject;
