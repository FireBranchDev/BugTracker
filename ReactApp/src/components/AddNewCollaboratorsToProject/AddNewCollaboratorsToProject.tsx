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
import { useQuery, type QueryFunction } from '@tanstack/react-query';
import { useState, type ChangeEventHandler, type FC } from 'react';
import type { User } from '../../types';

type AddNewCollaboratorsToProjectProps = {
  projectId: number;
};

const AddNewCollaboratorsToProject: FC<AddNewCollaboratorsToProjectProps> = ({
  projectId,
}) => {
  const SEARCH_RESULT_USERS_LIMIT = 10;

  const [selectedUsers, setSelectedUsers] = useState<Array<User>>([]);

  const [lastRetrievedId, setLastRetrievedId] = useState(0);
  const [lastRetrievedIdHistory, setLastRetrievedIdHistory] = useState<
    Array<number>
  >([]);

  const [displayName, setDisplayName] = useState('');

  const { getAccessTokenSilently } = useAuth0();

  const abortController = new AbortController();
  const signal = abortController.signal;

  const searchUsersAsync: QueryFunction<
    User[] | undefined,
    [
      string,
      {
        projectId: number;
        displayName: string;
        lastRetrievedId: number;
        signal: AbortSignal;
      },
    ]
  > = async ({ queryKey }): Promise<User[]> => {
    const [_key, { projectId, displayName, lastRetrievedId }] = queryKey;
    const accessToken = await getAccessTokenSilently();

    const endpoint = `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/users/v2/search?excludeFromProjectId=${projectId}&displayName=${displayName}&limit=${SEARCH_RESULT_USERS_LIMIT}&lastRetrievedId=${lastRetrievedId}`;

    const res = await fetch(endpoint, {
      headers: {
        authorization: `Bearer ${accessToken}`,
        'content-type': 'application/json',
      },
      signal,
    });
    return await res.json();
  };

  const { isFetching, data } = useQuery({
    enabled: displayName !== '',
    queryKey: [
      'search-users',
      { projectId, displayName, lastRetrievedId, signal },
    ],
    queryFn: searchUsersAsync,
  });

  const handleClickBackButton = () => {
    abortController.abort();

    if (lastRetrievedIdHistory.length <= 1) {
      setLastRetrievedId(0);
      return;
    }

    let lastRetrievedIdHistoryCopy = [...lastRetrievedIdHistory];
    const id = lastRetrievedIdHistoryCopy[1];

    setLastRetrievedId(id);

    lastRetrievedIdHistoryCopy.splice(0, 1);

    setLastRetrievedIdHistory(lastRetrievedIdHistoryCopy);
  };

  const handleClickNextButton = () => {
    abortController.abort();

    if (!data || data.length < SEARCH_RESULT_USERS_LIMIT) return;
    const dataCopy = [...data];

    const lastUserResult = dataCopy.splice(-1)[0];

    setLastRetrievedId(lastUserResult.id);

    if (lastRetrievedIdHistory.includes(lastUserResult.id)) return;

    const lastRetrievedIdHistoryCopy = [
      lastUserResult.id,
      ...lastRetrievedIdHistory,
    ];
    setLastRetrievedIdHistory(lastRetrievedIdHistoryCopy);
  };

  const handleChangeDisplayNameTextField: ChangeEventHandler<
    HTMLInputElement | HTMLTextAreaElement
  > = (event) => {
    abortController.abort();
    setLastRetrievedId(0);
    setLastRetrievedIdHistory([]);
    setDisplayName(event.target.value);
  };

  const onSelectUser = (user: User) => () => {
    const updatedSelectedUsers = [...selectedUsers];
    const index = selectedUsers.findIndex((u) => u.id === user.id);

    if (index === -1) {
      updatedSelectedUsers.push(user);
    } else {
      updatedSelectedUsers.splice(index, 1);
    }

    setSelectedUsers(updatedSelectedUsers);
  };

  return (
    <Box sx={{ mt: 2 }}>
      <Typography variant="body1">Add New Collaborators</Typography>
      <TextField
        fullWidth
        sx={{ mt: 0.5 }}
        onChange={handleChangeDisplayNameTextField}
      />
      {isFetching && <Typography variant="body1">Loading</Typography>}

      <List>
        {data?.map((user) => {
          const labelId = 'add-new-collaborator-user-display-name-label';
          return (
            <ListItem key={user.id} disablePadding>
              <ListItemButton
                role={undefined}
                onClick={onSelectUser(user)}
                dense
              >
                <ListItemIcon>
                  <Checkbox
                    edge="start"
                    tabIndex={-1}
                    checked={selectedUsers.indexOf(user) !== -1}
                    disableRipple
                    aria-labelledby={labelId}
                  />
                </ListItemIcon>
                <ListItemText id={labelId} primary={user.displayName} />
              </ListItemButton>
            </ListItem>
          );
        })}
      </List>

      <Button
        disabled={!data || lastRetrievedId === 0}
        onClick={handleClickBackButton}
      >
        Back
      </Button>
      <Button
        disabled={!data || data.length < SEARCH_RESULT_USERS_LIMIT}
        onClick={handleClickNextButton}
      >
        Next
      </Button>
      <Button variant="contained" disabled={selectedUsers.length === 0}>
        Add Users
      </Button>
    </Box>
  );
};

export default AddNewCollaboratorsToProject;
