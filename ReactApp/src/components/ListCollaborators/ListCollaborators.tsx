import { useAuth0 } from '@auth0/auth0-react';
import { Grid, Typography } from '@mui/material';
import { useMutation, useQuery } from '@tanstack/react-query';
import { useEffect, useState, type FC } from 'react';
import type { Collaborator } from '../../types';
import CollaboratorCard from '../CollaboratorCard/CollaboratorCard';

type Props = {
  projectId: number;
  newCollaboratorsEvent: Event;
};

const ListCollaborators: FC<Props> = ({ projectId, newCollaboratorsEvent }) => {
  useEffect(() => {
    const eventListenerCallback = () => {
      refetch();
    };

    document.addEventListener(
      newCollaboratorsEvent.type,
      eventListenerCallback,
    );

    return () => {
      document.removeEventListener(
        newCollaboratorsEvent.type,
        eventListenerCallback,
      );
    };
  });

  const { getAccessTokenSilently } = useAuth0();

  const getCollaborators = async (): Promise<Array<Collaborator>> => {
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
    return await res.json();
  };

  const [collaborators, setCollaborators] = useState<Array<Collaborator>>();

  const { isPending, data, refetch } = useQuery({
    queryKey: ['collaborators', projectId],
    queryFn: getCollaborators,
  });

  useEffect(() => {
    if (data !== undefined) {
      setCollaborators(data);
    }
  }, [data]);

  const removeCollaboratorMutation = useMutation({
    mutationFn: async ({
      projectId,
      userId,
    }: {
      projectId: number;
      userId: number;
    }) => {
      const accessToken = await getAccessTokenSilently();
      return await fetch(
        `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/Projects/${projectId}/collaborators/remove/${userId}`,
        {
          method: 'DELETE',
          headers: {
            authorization: `Bearer ${accessToken}`,
          },
        },
      );
    },
  });

  const removeCollaborator = async (userId: number) => {
    if (collaborators === undefined) return;
    await removeCollaboratorMutation.mutateAsync({ projectId, userId });

    if (removeCollaboratorMutation.isSuccess) {
      const updatedCollaborators = [...collaborators];
      const indexOfCollaborator = collaborators.findIndex(
        (c) => c.id === userId,
      );
      updatedCollaborators.splice(indexOfCollaborator, 1);
      setCollaborators(updatedCollaborators);
    }
  };

  return (
    <>
      {isPending && <Typography variant="body1">Pending...</Typography>}
      {collaborators && (
        <Grid container spacing={2}>
          {collaborators.map((collaborator) => {
            return (
              <Grid key={collaborator.id}>
                <CollaboratorCard
                  onClickRemoveButton={() =>
                    removeCollaborator(collaborator.id)
                  }
                  displayName={collaborator.displayName}
                />
              </Grid>
            );
          })}
        </Grid>
      )}
    </>
  );
};

export default ListCollaborators;
