import { useAuth0 } from '@auth0/auth0-react';
import {
  Box,
  Button,
  FormControl,
  Input,
  InputLabel,
  MenuItem,
  OutlinedInput,
  Select,
  Stack,
  TextField,
  Typography,
  type SelectChangeEvent,
} from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useEffect, useState, type FC } from 'react';
import { useLocation, useParams } from 'react-router-dom';
import { BugLifecycle } from '../../enums/bug-lifecycle';
import { BugStatusChip } from '../Bug';

const DetailedBugPage: FC = () => {
  const location = useLocation();
  const params = useParams();

  const { bugId } = params;

  let { title, description } = location.state || [undefined, undefined];

  const { getAccessTokenSilently } = useAuth0();

  const { data } = useQuery({
    queryKey: ['findBug', bugId],
    queryFn: async () => {
      const bug = await fetch(
        `${import.meta.env.VITE_BUGTRACKER_BACKEND_API_ORIGIN}/api/bugs/${bugId}/find`,
        {
          headers: {
            Authorization: `Bearer ${await getAccessTokenSilently()}`,
          },
        },
      );
      return await bug.json();
    },
    enabled: () => {
      return title === undefined || description === undefined;
    },
  });

  if (data) {
    title = data.title;
    description = data.description;
  }

  const [collaborators, setCollaborators] = useState([
    'Jane Doe',
    'John Doe',
    'Mark Doe',
    'Jack',
  ]);

  const [collaboratorsSelectedToAssign, setCollaboratorsSelectedToAssign] =
    useState<Array<string>>([]);

  const onCollaboratorsSelectedToAssignHandleChange = (
    event: SelectChangeEvent<typeof collaborators>,
  ) => {
    const {
      target: { value },
    } = event;
    setCollaboratorsSelectedToAssign(
      typeof value === 'string' ? value.split(',') : value,
    );
  };

  useEffect(() => {
    console.log(collaboratorsSelectedToAssign);
  }, [collaboratorsSelectedToAssign]);

  const [searchForCollaboratorInput, setSearchForCollaboratorInput] =
    useState('');

  const [filteredCollaborators, setFilteredCollaborators] = useState<
    Array<string>
  >([]);

  useEffect(() => {
    console.log(searchForCollaboratorInput);
  }, [searchForCollaboratorInput]);

  return (
    <Box
      sx={{
        m: 1,
      }}
    >
      <Typography variant="h1" textAlign="center">
        {title}
      </Typography>
      <Typography variant="h6">Description</Typography>
      {description && <Typography variant="body1">{description}</Typography>}
      <Typography variant="h6">Status</Typography>
      <BugStatusChip lifecycle={BugLifecycle.New} />
      <Typography variant="h6">Created</Typography>
      <Typography variant="body1">
        16th September 2025, 1:43 PM UTC+8
      </Typography>
      <Typography variant="h6">Updated</Typography>
      <Typography variant="body1">
        16th September 2025, 1:46 PM UTC+8
      </Typography>
      <Box component="section" width="50%">
        <Typography variant="h5">Assign Collaborators</Typography>
        <Stack direction="column" spacing={1.5} mt={1}>
          <TextField
            id="collaborators"
            label="Search for collaborators"
            variant="outlined"
            onChange={(event: React.ChangeEvent<HTMLInputElement>) => {
              let result = collaborators.filter((element, index) => {
                console.log('filtering through collaborators', element);
                return element.includes(event.target.value.trim());
              });
              console.log('Result after filter', result);
              setSearchForCollaboratorInput(event.target.value);
              setFilteredCollaborators(result);
            }}
          />
          {filteredCollaborators.length >= 1 && (
            <FormControl>
              <InputLabel id="collaborators-assign-select-label">
                Select Collaborators
              </InputLabel>
              <Select
                labelId="collaborators-assign-select-label"
                id="collaborators-assign-select"
                multiple
                label="Select Collaborators"
                value={collaboratorsSelectedToAssign}
                onChange={onCollaboratorsSelectedToAssignHandleChange}
              >
                {filteredCollaborators.map((collaborator, index) => {
                  return (
                    <MenuItem key={index} value={`${collaborator}`}>
                      {collaborator}
                    </MenuItem>
                  );
                })}
              </Select>
            </FormControl>
          )}

          <Button variant="contained">Assign</Button>
          <Button
            variant="contained"
            color="secondary"
            onClick={() => {
              setCollaboratorsSelectedToAssign([]);
            }}
          >
            Clear Selected
          </Button>
        </Stack>
      </Box>
    </Box>
  );
};

export default DetailedBugPage;
