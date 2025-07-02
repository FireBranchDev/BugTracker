import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { Grid, IconButton, Typography } from '@mui/material';
import { type FC } from 'react';
import { Link } from 'react-router-dom';
import ProjectSettingsAdminstrative from '../../ProjectSettingsAdminstrative';

const ProjectSettings: FC = () => {
  return (
    <>
      <IconButton
        aria-label="back"
        color="primary"
        sx={{
          p: {
            xs: 1,
            md: 2,
            lg: 3,
          },
        }}
        component={Link}
        to=".."
        relative="path"
      >
        <ArrowBackIcon />
      </IconButton>
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
