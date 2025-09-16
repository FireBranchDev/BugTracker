import { Button, CardActions, Typography } from '@mui/material';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import React from 'react';
import {
  Link as RouterLink,
  type LinkProps as RouterLinkProps,
} from 'react-router-dom';
import { BugLifecycle } from '../../../enums/bug-lifecycle';
import BugStatusChip from '../BugStatusChip';

type Props = {
  title: string;
  description?: string;
  detailedBugUrl: string;
};

const LinkBehavior = React.forwardRef<any, RouterLinkProps>((props, ref) => (
  <RouterLink ref={ref} {...props} role={undefined} />
));

function BugCard({ title, description, detailedBugUrl }: Props) {
  return (
    <>
      <Card>
        <CardContent>
          <BugStatusChip lifecycle={BugLifecycle.New} />
          <Typography variant="h5" gutterBottom>
            {title}
          </Typography>
          {description && (
            <Typography variant="body2">{description}</Typography>
          )}
        </CardContent>
        <CardActions>
          <Button
            size="small"
            component={LinkBehavior}
            to={detailedBugUrl}
            state={{ title: title, description: description }}
          >
            View Details
          </Button>
        </CardActions>
      </Card>
    </>
  );
}

export default BugCard;
