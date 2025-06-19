import { Box, Button, Typography } from '@mui/material';
import type { FC } from 'react';

export type CollaboratorCardProps = {
  displayName: string;
  onClickRemoveButton: () => void;
};

const CollaboratorCard: FC<CollaboratorCardProps> = ({
  displayName,
  onClickRemoveButton,
}) => {
  return (
    <Box>
      <Typography variant="h6">{displayName}</Typography>
      <Button onClick={onClickRemoveButton}>Remove</Button>
    </Box>
  );
};

export default CollaboratorCard;
