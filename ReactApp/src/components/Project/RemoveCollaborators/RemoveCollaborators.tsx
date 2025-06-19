import { useAuth0 } from '@auth0/auth0-react';
import {
  Box,
  Button,
  Checkbox,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  TextField,
  Typography,
} from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useState, type FC } from 'react';
import type { Collaborator, User } from '../../../types';

type RemoveCollaboratorProps = {
  projectId: number;
};

type QueryKeyParameters = {
  projectId: number;
};

const RemoveCollaborators: FC<RemoveCollaboratorProps> = ({ projectId }) => {
  const { getAccessTokenSilently } = useAuth0();

  const { data, isPending, isError, error } = useQuery({
    queryKey: ['retrieveProjectCollaborators', { projectId }],
    queryFn: async ({ queryKey }): Promise<Collaborator[]> => {
      const [_key, parameters] = queryKey;
      if (parameters === undefined || typeof parameters !== 'object') return [];

      const { projectId } = parameters as QueryKeyParameters;

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
      return (await res.json()) as Collaborator[];
    },
  });

  const [_displayName, setDisplayName] = useState('');

  const [selectedUsers, setSelectedUsers] = useState<Array<User>>([]);
  const onUserSelected = (user: User) => () => {
    const currentIndex = selectedUsers.indexOf(user);
    const newSelectedUsers = [...selectedUsers];
    if (currentIndex === -1) {
      newSelectedUsers.push(user);
    } else {
      newSelectedUsers.splice(currentIndex, 1);
    }

    setSelectedUsers(newSelectedUsers);
  };

  return (
    <Box component="section" sx={{ m: 0.5 }}>
      <Typography variant="h4">Remove Collaborators</Typography>

      <TextField
        id="remove-collaborators-search-by-display-name"
        label="Display Name"
        variant="filled"
        onChange={(e) => setDisplayName(e.target.value)}
        fullWidth
      />

      {isPending && (
        <Typography variant="body1">Loading collaborators...</Typography>
      )}

      {isError && <Typography variant="body1">{error.message}</Typography>}

      {data && (
        <>
          <List>
            {data.map((user) => {
              return (
                <ListItem key={user.id} disablePadding>
                  <ListItemButton
                    role={undefined}
                    onClick={onUserSelected(user)}
                    dense
                  >
                    <ListItemIcon>
                      <Checkbox
                        edge="start"
                        tabIndex={-1}
                        checked={selectedUsers.includes(user)}
                        disableRipple
                      />
                    </ListItemIcon>
                    <ListItemText primary={user.displayName} />
                  </ListItemButton>
                </ListItem>
              );
            })}
          </List>
        </>
      )}

      <Typography variant="h6">Selected</Typography>
      {selectedUsers.length > 0 && (
        <>
          <List>
            {selectedUsers.map((user) => {
              return (
                <ListItem key={user.id}>
                  <ListItemText primary={user.displayName} />
                </ListItem>
              );
            })}
          </List>

          <Button variant="contained" fullWidth>
            Remove
          </Button>
        </>
      )}
    </Box>
  );
};

export default RemoveCollaborators;
