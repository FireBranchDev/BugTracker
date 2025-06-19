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
import { useMutation, useQuery } from '@tanstack/react-query';
import { useEffect, useState, type FC } from 'react';
import type { User } from '../types';

type AddCollaboratorsProps = {
  projectId: number;
};

type SearchByDisplayNameQueryParameters = {
  projectId: number;
  displayName: string;
};

type SearchByDisplayNameQueryFnParameters = {
  queryKey: Array<string | SearchByDisplayNameQueryParameters>;
};

const searchUsersByDisplayNameAsync = async (
  accessToken: string,
  displayName: string,
  projectId: number,
): Promise<Array<User>> => {
  if (!displayName) return [];

  const res = await fetch(
    `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/users/search?displayName=${displayName}&excludeFromProjectId=${projectId}`,
    {
      headers: {
        authorization: `Bearer ${accessToken}`,
        'content-type': 'application/json',
      },
    },
  );
  return res.json();
};

const AddCollaborators: FC<AddCollaboratorsProps> = ({ projectId }) => {
  const [displayName, setDisplayName] = useState('');
  const [userIdsToAdd, setUserIdsToAdd] = useState<Array<number>>([]);
  const [users, setUsers] = useState<Array<User>>([]);

  const { getAccessTokenSilently } = useAuth0();

  const searchByDisplayNameQueryKey = [
    'searchByDisplayName',
    { displayName, projectId },
  ];
  const searchByDisplayNameQueryFn = async ({
    queryKey,
  }: SearchByDisplayNameQueryFnParameters) => {
    const { displayName, projectId } =
      queryKey[1] as SearchByDisplayNameQueryParameters;

    const accessToken = await getAccessTokenSilently();
    return await searchUsersByDisplayNameAsync(
      accessToken,
      displayName,
      projectId,
    );
  };

  const { data, isFetching } = useQuery({
    enabled: displayName !== '',
    queryKey: searchByDisplayNameQueryKey,
    queryFn: searchByDisplayNameQueryFn,
  });

  const removeUserIdToAdd = (id: number) => {
    const copyOfUserIdsToAdd = [...userIdsToAdd];
    const index = copyOfUserIdsToAdd.indexOf(id);
    copyOfUserIdsToAdd.splice(index, 1);
    setUserIdsToAdd(copyOfUserIdsToAdd);
  };

  const removeUser = (id: number) => {
    const user = users.find((u) => {
      return u.id === id;
    });
    if (user === undefined) return;
    const copyOfUsers = [...users];
    const index = copyOfUsers.indexOf(user);
    copyOfUsers.splice(index, 1);
    setUsers(copyOfUsers);
  };

  const addMutation = useMutation({
    mutationFn: async (userId: number) => {
      const accessToken = await getAccessTokenSilently();
      return await fetch(
        `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/projects/${projectId}/add-collaborator/${userId}`,
        {
          method: 'POST',
          headers: {
            authorization: `Bearer ${accessToken}`,
            'content-type': 'application/json',
          },
        },
      );
    },
    onSuccess: async (_data, userId, _context) => {
      removeUserIdToAdd(userId);
      removeUser(userId);
    },
  });

  useEffect(() => {
    if (data !== undefined) {
      setUsers(data);
    }
  }, [data]);

  const handleToggle = (value: number) => () => {
    const copyOfUserIdsToAdd = [...userIdsToAdd];
    const index = copyOfUserIdsToAdd.indexOf(value);
    if (index === -1) {
      copyOfUserIdsToAdd.push(value);
    } else {
      copyOfUserIdsToAdd.splice(index, 1);
    }

    setUserIdsToAdd(copyOfUserIdsToAdd);
  };

  const onClickAddButton = async () => {
    if (userIdsToAdd.length === 0) return;
    userIdsToAdd.map(async (value) => {
      await addMutation.mutateAsync(value);
    });
  };

  return (
    <Box component="section">
      <Typography variant="h6">Add a Collaborator</Typography>
      <TextField
        label="display name"
        fullWidth
        onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
          setDisplayName(e.target.value)
        }
      />

      {isFetching && <p>Loading...</p>}

      {users && users.length > 0 && displayName !== '' && (
        <List dense>
          {users.map((user) => {
            const labelId = `collaborator-list-label-${user.id}`;
            return (
              <ListItem key={user.id}>
                <ListItemButton onClick={handleToggle(user.id)}>
                  <ListItemIcon>
                    <Checkbox
                      edge="start"
                      checked={userIdsToAdd.includes(user.id)}
                      tabIndex={-1}
                      aria-labelledby={labelId}
                    />
                  </ListItemIcon>
                  <ListItemText id={labelId} primary={user.displayName} />
                </ListItemButton>
              </ListItem>
            );
          })}
        </List>
      )}

      {!isFetching && data && data.length === 0 && <p>No users found.</p>}

      <Button
        variant="contained"
        fullWidth
        sx={{ mt: 1 }}
        disabled={userIdsToAdd.length === 0}
        onClick={onClickAddButton}
      >
        Add
      </Button>
    </Box>
  );
};

export default AddCollaborators;
