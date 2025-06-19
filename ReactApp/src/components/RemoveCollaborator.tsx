import { useAuth0 } from '@auth0/auth0-react';
import DeleteIcon from '@mui/icons-material/Delete';
import PersonIcon from '@mui/icons-material/Person';
import {
  Icon,
  IconButton,
  List,
  ListItem,
  ListItemText,
  Typography,
} from '@mui/material';
import { useMutation } from '@tanstack/react-query';
import { useState, type FC } from 'react';
import type { Collaborator, SetCollaboratorsCallback } from '../types';
import RemoveCollaboratorConfirmationDialog from './RemoveCollaboratorConfirmationDialog';

type RemoveCollaboratorProps = {
  collaborators: Array<Collaborator>;
  setCollaborators: (callback: SetCollaboratorsCallback) => void;
  projectId: number;
};

type RemoveCollaboratorMutationProps = {
  projectId: number;
  collaboratorId: number;
};

const RemoveCollaborator: FC<RemoveCollaboratorProps> = ({
  collaborators,
  setCollaborators,
  projectId,
}) => {
  const [openRemoveCollaboratorDialog, setOpenRemoveCollaboratorDialog] =
    useState(false);

  const [collaboratorIdToRemove, setCollaboratorIdToRemove] = useState(-1);

  const { getAccessTokenSilently } = useAuth0();

  const removeCollaboratorMutation = useMutation({
    mutationFn: async (
      removeCollaborator: RemoveCollaboratorMutationProps,
    ): Promise<Response> => {
      const accessToken = await getAccessTokenSilently();
      return await fetch(
        `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/projects/${removeCollaborator.projectId}/collaborators/remove/${removeCollaborator.collaboratorId}`,
        {
          method: 'DELETE',
          headers: {
            authorization: `Bearer ${accessToken}`,
          },
        },
      );
    },
    onSuccess: () => {
      removeCollaborator(collaboratorIdToRemove);
      setCollaboratorIdToRemove(-1);
      setOpenRemoveCollaboratorDialog(false);
    },
    onError: (error) => {
      console.log(error);
    },
  });

  const onCloseRemoveCollaboratorDialog = () => {
    setOpenRemoveCollaboratorDialog(false);
  };

  const removeCollaborator = (id: number) => {
    setCollaborators((prev) => prev.filter((value) => value.id !== id));
  };

  const onConfirmRemoveCollaboratorDialog = async () => {
    await removeCollaboratorMutation.mutateAsync({
      projectId,
      collaboratorId: collaboratorIdToRemove,
    });
  };

  const displayRemoveCollaboratorConfirmationDialog = (
    idOfCollaboratorToRemove: number,
  ) => {
    setCollaboratorIdToRemove(idOfCollaboratorToRemove);
    setOpenRemoveCollaboratorDialog(true);
  };

  return (
    <>
      <Typography variant="h6">Collaborators</Typography>
      <List dense={true}>
        {collaborators.map((collaborator) => {
          return (
            <ListItem
              key={collaborator.id}
              secondaryAction={
                collaborator.isOwner ? (
                  <Icon>
                    <PersonIcon />
                  </Icon>
                ) : (
                  <IconButton
                    edge="end"
                    aria-label="delete"
                    onClick={() =>
                      displayRemoveCollaboratorConfirmationDialog(
                        collaborator.id,
                      )
                    }
                  >
                    <DeleteIcon />
                  </IconButton>
                )
              }
            >
              <ListItemText
                primary={`${collaborator.displayName}`}
                secondary={`${collaborator.joined}`}
              />
            </ListItem>
          );
        })}
      </List>

      <RemoveCollaboratorConfirmationDialog
        open={openRemoveCollaboratorDialog}
        onClose={onCloseRemoveCollaboratorDialog}
        onConfirm={onConfirmRemoveCollaboratorDialog}
      />
    </>
  );
};

export default RemoveCollaborator;
