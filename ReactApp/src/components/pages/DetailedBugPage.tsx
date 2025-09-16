import { useAuth0 } from '@auth0/auth0-react';
import { Box, Button, TextField, Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import type { FC } from 'react';
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
      <Box component="section">
        <Typography variant="h5">Assign collaborators?</Typography>
        <TextField id="collaborator" label="Collaborator" variant="filled" />
        <Button variant="contained">Assign</Button>
      </Box>
    </Box>
  );
};

export default DetailedBugPage;
