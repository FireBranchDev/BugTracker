import { Typography } from '@mui/material';
import type { FC } from 'react';
import { useParams } from 'react-router-dom';
import AddNewCollaboratorsToProject from '../../AddNewCollaboratorsToProject';
import ListCollaborators from '../../ListCollaborators';

const Collaborators: FC = () => {
  const params = useParams();

  return (
    <>
      <Typography variant="h4">Manage Collaborators</Typography>
      {params.projectId !== undefined && Number(params.projectId) && (
        <>
          <ListCollaborators projectId={Number(params.projectId)} />
          <AddNewCollaboratorsToProject projectId={Number(params.projectId)} />
        </>
      )}
    </>
  );
};

export default Collaborators;
