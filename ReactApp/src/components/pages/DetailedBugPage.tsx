import { useAuth0 } from '@auth0/auth0-react';
import { Typography } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import type { FC } from 'react';
import { useLocation, useParams } from 'react-router-dom';

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
    <>
      <Typography variant="h1" textAlign="center">
        {title}
      </Typography>
      {description && <Typography variant="body1">{description}</Typography>}
    </>
  );
};

export default DetailedBugPage;
