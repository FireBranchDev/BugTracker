import { useAuth0, withAuthenticationRequired } from '@auth0/auth0-react';
import { Grid, Typography } from '@mui/material';
import { useMutation, useQuery } from '@tanstack/react-query';
import { useEffect, useState, type FC } from 'react';
import type { NewProject } from '../../types/new-project';
import type { Project } from '../../types/project';
import CreateProjectForm from '../forms/CreateProjectForm';
import Loading from '../Loading';
import ProjectsCards from '../ProjectsCards';

const ProjectsPage: FC = () => {
  const [projects, setProjects] = useState<Array<Project>>([]);

  const { data, isSuccess, isLoading } = useQuery({
    queryKey: ['projects'],
    queryFn: async () => {
      const accessToken = await getAccessTokenSilently();
      const projects = await fetch(
        `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/projects`,
        {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        },
      );
      return await projects.json();
    },
  });

  useEffect(() => {
    if (isSuccess) {
      Object.values<Project>(data).forEach((project) => {
        setProjects((prev) => [...prev, project]);
      });
    }
  }, [data, isSuccess]);

  const { getAccessTokenSilently } = useAuth0();

  const [isErrorCreatingProject, setIsErrorCreatingProject] = useState(false);

  const createProjectMutation = useMutation({
    mutationFn: async (project: NewProject) => {
      const accessToken = await getAccessTokenSilently();
      return await fetch(
        `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/projects`,
        {
          method: 'POST',
          headers: {
            Authorization: `Bearer ${accessToken}`,
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            name: project.name,
            description: project.description,
          }),
        },
      );
    },
    onSuccess: (data, variables) => {
      const location = data.headers.get('Location');
      if (location) {
        const url = new URL(location);
        const splitPathname = url.pathname.split('/');
        splitPathname.forEach((value) => {
          if (parseInt(value)) {
            setProjects((prev) => [
              ...prev,
              {
                id: parseInt(value),
                name: variables.name,
                description: variables.description,
              },
            ]);
          }
        });
      }
    },
  });

  const createProjectAsync = async (
    name: string,
    description: string | null,
  ) => {
    const newProject: NewProject = {
      name,
      description,
    };

    try {
      await createProjectMutation.mutateAsync(newProject);
      setIsErrorCreatingProject(false);
    } catch {
      setIsErrorCreatingProject(true);
    }
  };

  return (
    <>
      <Typography variant="h1" textAlign="center">
        Projects
      </Typography>
      <Grid container spacing={2} sx={{ m: 2 }}>
        <Grid
          size={{ xs: 12, md: 8, lg: 9 }}
          sx={{
            order: {
              xs: 2,
              md: 1,
            },
          }}
        >
          {isLoading ? <Loading /> : <ProjectsCards projects={projects} />}
        </Grid>
        <Grid
          size={{ xs: 12, sm: 8, md: 4, lg: 3 }}
          offset={{ sm: 2, md: 0 }}
          sx={{
            order: {
              xs: 1,
              md: 2,
            },
          }}
        >
          <CreateProjectForm
            createProjectAsync={createProjectAsync}
            isErrorCreatingProject={isErrorCreatingProject}
          />
        </Grid>
      </Grid>
    </>
  );
};

export default withAuthenticationRequired(ProjectsPage);
