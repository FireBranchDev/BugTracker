import { Button, CardActions, CardHeader, Typography } from '@mui/material';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import type { FC } from 'react';
import { useNavigate } from 'react-router-dom';
import { truncateString } from '../helpers/string';

type ProjectCardProps = {
  id: number;
  name: string;
  description: string | null;
};

const ProjectCard: FC<ProjectCardProps> = ({ id, name, description }) => {
  const MAX_DESCRIPTION_LENGTH = 90;

  const navigate = useNavigate();

  return (
    <Card variant="outlined">
      <CardHeader component="header" title={name} />
      <CardContent>
        {description != null && (
          <Typography variant="body1">
            {truncateString(description, MAX_DESCRIPTION_LENGTH)}
          </Typography>
        )}
      </CardContent>
      <CardActions>
        <Button
          onClick={() =>
            navigate(`/projects/${id}`, {
              state: { projectName: name },
            })
          }
        >
          View
        </Button>
      </CardActions>
    </Card>
  );
};

export default ProjectCard;
