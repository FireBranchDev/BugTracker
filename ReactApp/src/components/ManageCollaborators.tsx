import { useAuth0 } from '@auth0/auth0-react';
import { Box, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useEffect, useState, type FC } from 'react';
import type { Collaborator } from '../types';
import AddCollaborators from './AddCollaborators';
import RemoveCollaborator from './RemoveCollaborator';

type ManageCollaboratorsProps = {
  projectId: number;
};

const ManageCollaborators: FC<ManageCollaboratorsProps> = ({ projectId }) => {
  const [collaborators, setCollaborators] = useState<Array<Collaborator>>([]);

  const { getAccessTokenSilently } = useAuth0();

  const queryCollaborators = async (
    projectId: number,
  ): Promise<Array<Collaborator>> => {
    const accessToken = await getAccessTokenSilently();
    const res = await fetch(
      `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/projects/${projectId}/collaborators`,
      {
        headers: {
          authorization: `Bearer ${accessToken}`,
          'content-type': 'application/json',
        },
      },
    );
    const jsonData: Array<Collaborator> = await res.json();
    return jsonData;
  };

  const { data } = useQuery({
    queryKey: ['collaborators', projectId],
    queryFn: async () => await queryCollaborators(projectId),
  });

  useEffect(() => {
    if (data !== undefined) {
      data.map((value) => {
        setCollaborators((prev) => [
          ...prev,
          {
            id: value.id,
            displayName: value.displayName,
            isOwner: value.isOwner,
            joined: value.joined,
          },
        ]);
      });
    }
  }, [data]);

  return (
    <Box component="section" sx={{ m: 2, mb: 4 }}>
      <Typography variant="h4">Manage Collaborators</Typography>

      <RemoveCollaborator
        collaborators={collaborators}
        setCollaborators={setCollaborators}
        projectId={projectId}
      />

      <AddCollaborators projectId={projectId} />
    </Box>
  );
};

export default ManageCollaborators;
