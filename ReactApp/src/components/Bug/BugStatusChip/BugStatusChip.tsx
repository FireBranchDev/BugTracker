import { Chip } from '@mui/material';
import type { FC } from 'react';
import { BugLifecycle } from '../../../enums/bug-lifecycle';

type Props = {
  lifecycle: BugLifecycle;
};

const BugStatusChip: FC<Props> = ({ lifecycle }) => {
  let label = '';

  let colour:
    | 'default'
    | 'info'
    | 'primary'
    | 'secondary'
    | 'error'
    | 'success'
    | 'warning'
    | undefined;

  switch (lifecycle) {
    case BugLifecycle.New: {
      label = 'New';
      colour = 'default';
      break;
    }
    case BugLifecycle.Assigned: {
      label = 'Assigned';
      colour = 'default';
      break;
    }
    case BugLifecycle.Verified: {
      label = 'Verified';
      colour = 'success';
      break;
    }
    case BugLifecycle.Resolved: {
      label = 'Resolved';
      colour = 'success';
      break;
    }
    case BugLifecycle.Reopened: {
      label = 'Reopened';
      colour = 'default';
      break;
    }
    case BugLifecycle.Closed: {
      label = 'Closed';
      colour = 'error';
      break;
    }
  }

  return <Chip label={label} color={colour} />;
};

export default BugStatusChip;
