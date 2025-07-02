import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { Grid, IconButton, Typography } from '@mui/material';
import { type FC } from 'react';
import { Link } from 'react-router-dom';
import ProjectSettingsAdminstrative from '../../ProjectSettingsAdminstrative';

const ProjectSettings: FC = () => {
  return (
    <>
      <Grid container sx={{ m: 2 }}>
        <Grid size={{ xs: 12, md: 'auto' }}>
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
        </Grid>
        <Grid size={{ xs: 12, md: 'grow' }}>
          <Typography variant="h1" textAlign="center">
            Settings
          </Typography>
        </Grid>
      </Grid>
      <Grid container spacing={2} sx={{ m: 2 }}>
        <Grid size={{ xs: 12, md: 6, lg: 'grow' }}>
          <Typography variant="h3">General</Typography>
        </Grid>
        <Grid size={{ xs: 12, md: 6, lg: 4 }}>
          <ProjectSettingsAdminstrative />
        </Grid>
      </Grid>
    </>
  );
};

export default ProjectSettings;
