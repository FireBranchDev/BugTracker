import { Button, Dialog, DialogActions, DialogTitle } from '@mui/material';
import type { FC } from 'react';

type RemoveCollaboratorConfirmationDialogProps = {
  open: boolean;
  onClose: () => void;
  onConfirm: () => void;
};

const RemoveCollaboratorConfirmationDialog: FC<
  RemoveCollaboratorConfirmationDialogProps
> = ({ open, onClose, onConfirm }) => {
  return (
    <Dialog
      open={open}
      onClose={onClose}
      aria-labelledby="remove-collaborator-confirmation-alert-dialog-title"
    >
      <DialogTitle id="remove-collaborator-confirmation-alert-dialog-title">
        Remove collaborator?
      </DialogTitle>
      <DialogActions>
        <Button onClick={onClose}>Abort</Button>
        <Button onClick={onConfirm} autoFocus>
          Confirm
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default RemoveCollaboratorConfirmationDialog;
