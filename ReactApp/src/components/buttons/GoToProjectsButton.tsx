import { Button } from '@mui/material';
import type { FC } from 'react';
import { useNavigate } from 'react-router-dom';

const GoToProjectsButton: FC = () => {
  const navigate = useNavigate();

  return (
    <Button
      variant="contained"
      sx={{ alignSelf: 'center' }}
      onClick={() => navigate('/projects')}
    >
      GO TO PROJECTS
    </Button>
  );
};

export default GoToProjectsButton;
