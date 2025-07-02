import { useAuth0 } from '@auth0/auth0-react';
import SettingsIcon from '@mui/icons-material/Settings';
import { Box, Grid, IconButton, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useEffect, useState } from 'react';
import { Link, useLocation, useNavigate, useParams } from 'react-router-dom';
import ProjectNotFoundException from '../../exceptions/project-not-found-exception';
import ViewingProjectForbiddenException from '../../exceptions/viewing-project-forbidden-exception';
import BugCard from '../BugCard';
import Error from '../Error';
import Loading from '../Loading';

const ProjectPage = () => {
  const { state } = useLocation();

  const { projectId } = useParams();

  const { getAccessTokenSilently } = useAuth0();

  const [projectName, setProjectName] = useState('');

  const navigate = useNavigate();

  const MAXIMUM_QUERY_RETRY_COUNT = 3;
  const { error, isLoading, data } = useQuery({
    queryKey: [`project_${projectId}`],
    queryFn: async () => {
      const accessToken = await getAccessTokenSilently();

      const response = await fetch(
        `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/projects/${projectId}`,
        {
          headers: {
            Authorization: `Bearer ${accessToken}`,
            'Content-Type': 'application/json',
          },
        },
      );

      if (response.status === 403) {
        throw new ViewingProjectForbiddenException();
      }

      if (response.status === 404) {
        throw new ProjectNotFoundException();
      }

      return response.json();
    },
    retry: (failureCount: number, error: Error) => {
      if (
        error instanceof ProjectNotFoundException ||
        error instanceof ViewingProjectForbiddenException
      ) {
        return false;
      }

      return failureCount < MAXIMUM_QUERY_RETRY_COUNT;
    },
  });

  useEffect(() => {
    if (state !== null && state.projectName !== null) {
      setProjectName(state.projectName);
    } else if (data !== undefined && data.name !== undefined) {
      setProjectName(data.name);
    }

    if (error instanceof ViewingProjectForbiddenException) {
      navigate('/forbidden');
    }

    if (error instanceof ProjectNotFoundException) {
      navigate('/not-found', {
        state: {
          projectNotFound: true,
        },
      });
    }
  }, [state, data, error]);

  if (isLoading) {
    return <Loading />;
  }

  if (error) return <Error />;

  return (
    <>
      <Grid container>
        <Grid offset="auto" sx={{ p: 1 }}>
          <IconButton
            aria-label="settings"
            color="primary"
            sx={{
              p: {
                xs: 1,
                md: 2,
                lg: 3,
              },
            }}
            component={Link}
            to="settings"
          >
            <SettingsIcon />
          </IconButton>
        </Grid>
        <Grid size={{ xs: 12 }}>
          <Typography variant="h1" textAlign="center">
            {projectName}
          </Typography>
        </Grid>
      </Grid>

      <Grid container spacing={3} sx={{ m: 4, mt: 2 }}>
        <Grid size={{ xs: 12, sm: 7, lg: 9 }}>
          <Box
            component="section"
            sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}
          >
            <Typography variant="h4">Bugs</Typography>
            <Grid container spacing={1} sx={{ mx: 2 }}>
              <Grid
                size={{
                  xs: 12,
                  sm: 6,
                  md: 4,
                  lg: 3,
                }}
              >
                <BugCard title="Bug One" />
              </Grid>
              <Grid
                size={{
                  xs: 12,
                  sm: 6,
                  md: 4,
                  lg: 3,
                }}
              >
                <BugCard title="Bug Two" />
              </Grid>
              <Grid
                size={{
                  xs: 12,
                  sm: 6,
                  md: 4,
                  lg: 3,
                }}
              >
                <BugCard title="Bug Three" />
              </Grid>
              <Grid
                size={{
                  xs: 12,
                  sm: 6,
                  md: 4,
                  lg: 3,
                }}
              >
                <BugCard title="Bug Four" />
              </Grid>
            </Grid>
          </Box>
        </Grid>
        <Grid size={{ xs: 12, sm: 5, lg: 3 }}>
          <Box component="section">
            <Typography variant="h4">Description</Typography>
            <Typography variant="body1">
              Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean
              commodo ligula eget dolor. Aenean massa. Cum sociis natoque
              penatibus et magnis dis parturient montes, nascetur ridiculus mus.
              Donec quam felis, ultricies nec, pellentesque eu, pretium quis,
              sem. Nulla consequat massa quis enim. Donec pede justo, fringilla
              vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut,
              imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede
              mollis pretium. Integer tincidunt. Cras dapibus. Vivamus e
            </Typography>
          </Box>
        </Grid>
      </Grid>
    </>
  );
};

export default ProjectPage;
