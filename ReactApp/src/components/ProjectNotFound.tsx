import { Box, Typography } from '@mui/material';
import type { FC } from 'react';
import GoToProjectsButton from './buttons/GoToProjectsButton';

const ProjectNotFound: FC = () => {
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        maxWidth: 'md',
        justifySelf: 'center',
      }}
    >
      <Typography variant="h1">Project not found</Typography>
      <GoToProjectsButton />
    </Box>
  );
};

export default ProjectNotFound;
