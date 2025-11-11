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
import type { AxiosResponse } from 'axios';
import { useEffect, useState, type FC } from 'react';
import { useLocation, useParams } from 'react-router-dom';
import { BugLifecycle } from '../../enums/bug-lifecycle';
import type { Bug } from '../../types/bug';
import { useAxios } from '../AxiosProvider/AxiosProvider';
import { BugStatusChip } from '../Bug';
import Loading from '../Loading';

type LocationState = {
  title: string | null;
  description: string | null;
};

type FindBugResult = Bug & {
  description: string | null;
};

const DetailedBugPage: FC = () => {
  const location = useLocation();
  const { bugId } = useParams() as { bugId: string };

  let { title, description } =
    (location.state as LocationState) ||
    ({
      title: null,
      description: null,
    } as LocationState);

  const axios = useAxios();
  const fetchBug = async (
    bugId: string,
  ): Promise<AxiosResponse<FindBugResult, any>> => {
    const result = await axios.get(`/bugs/${bugId}/find`);
    return result;
  };

  const { data, isLoading } = useQuery({
    queryKey: ['bug', bugId],
    queryFn: async () => {
      const { data } = await fetchBug(bugId);
      console.log(data);
      return data;
    },
  });

  useEffect(() => {
    console.log(data);
    if (data) {
      title = data.title;
      description = data.description;
    }
  }, [data]);

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

  const capitaliseText = (text: string) => {
    return text
      .split(' ')
      .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
      .join(' ');
  };

  return (
    <Box
      sx={{
        m: 1,
      }}
    >
      {isLoading ? (
        <Loading />
      ) : (
        <>
          {title && (
            <Typography variant="h1" textAlign="center">
              {capitaliseText(title)}
            </Typography>
          )}

          <Typography variant="h6">Description</Typography>
          {description ? (
            <Typography variant="body1">{description}</Typography>
          ) : (
            <Typography variant="body1">No description</Typography>
          )}
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
        </>
      )}
    </Box>
  );
};

export default DetailedBugPage;
