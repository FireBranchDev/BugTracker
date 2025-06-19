import { Grid } from '@mui/material';
import type { FC } from 'react';
import type { Project } from '../types/project';
import ProjectCard from './ProjectCard';

type ProjectCardsProps = {
  projects: Array<Project>;
};

const ProjectsCards: FC<ProjectCardsProps> = ({ projects }) => {
  return (
    <Grid container spacing={2}>
      {projects.map((project) => {
        return (
          <Grid key={project.id} size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
            <ProjectCard
              id={project.id}
              name={project.name}
              description={project.description}
            />
          </Grid>
        );
      })}
    </Grid>
  );
};

export default ProjectsCards;
