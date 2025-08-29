import { Button, CardActions, Typography } from '@mui/material';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import { type ReactNode } from 'react';
import { BugLifecycle } from '../../../enums/bug-lifecycle';
import BugStatusChip from '../BugStatusChip';

type Props = {
  title: string;
  description?: string;
};

function BugCard({ title, description }: Props): ReactNode {
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
          <Button size="small">Learn More</Button>
        </CardActions>
      </Card>
    </>
  );
}

export default BugCard;
