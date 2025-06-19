import { Grid, Typography } from '@mui/material';
import type { FC } from 'react';
import ProjectSettingsAdminstrative from '../../ProjectSettingsAdminstrative';

const ProjectSettings: FC = () => {
  return (
    <>
      <Typography variant="h1">Project Settings</Typography>
      <Grid container spacing={2}>
        <Grid size={{ xs: 6, md: 8 }}>
          <Typography variant="h3">General</Typography>
        </Grid>
        <Grid size={{ xs: 6, md: 4 }}>
          <ProjectSettingsAdminstrative />
        </Grid>
      </Grid>
    </>
  );
};

export default ProjectSettings;
