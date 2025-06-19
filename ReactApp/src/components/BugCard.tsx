import { Typography } from '@mui/material';
import type { FC } from 'react';

type BugCardProps = {
  title: string;
};

const BugCard: FC<BugCardProps> = ({ title }) => {
  return <Typography variant="h5">{title}</Typography>;
};

export default BugCard;
