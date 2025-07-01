import { Typography } from '@mui/material';
import { type FC } from 'react';
import { useParams } from 'react-router-dom';
import AddNewCollaboratorsToProject from '../../AddNewCollaboratorsToProject';
import ListCollaborators from '../../ListCollaborators';

const Collaborators: FC = () => {
  const params = useParams();

  const projectId = Number(params.projectId) || 0;

  const newCollaboratorsEvent = new Event('newCollaborators');

  return (
    <>
      <Typography variant="h4">Manage Collaborators</Typography>
      <ListCollaborators
        projectId={projectId}
        newCollaboratorsEvent={newCollaboratorsEvent}
      />
      <AddNewCollaboratorsToProject
        projectId={projectId}
        newCollaboratorsEvent={newCollaboratorsEvent}
      />
    </>
  );
};

export default Collaborators;
