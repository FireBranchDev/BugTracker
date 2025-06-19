import { Box, Typography } from '@mui/material';
import type { FC } from 'react';
import Collaborators from './Collaborators';

const ProjectSettingsAdminstrative: FC = () => {
  return (
    <Box sx={{ p: 1 }}>
      <Typography variant="h3">Adminstrative</Typography>
      <Collaborators />
    </Box>
  );
};

export default ProjectSettingsAdminstrative;
