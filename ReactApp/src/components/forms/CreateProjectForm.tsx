import { Alert, Button, Stack, TextField, Typography } from '@mui/material';
import { useState, type FC } from 'react';
import type { ValidationResult } from '../../types/validation-result';

type CreateProjectFormProps = {
  createProjectAsync: (name: string, description: string | null) => void;
  isErrorCreatingProject: boolean;
};

const CreateProjectForm: FC<CreateProjectFormProps> = ({
  createProjectAsync,
  isErrorCreatingProject,
}) => {
  const MAXIMUM_NAME_LENGTH = 120;
  const MINIMUM_NAME_LENGTH = 1;

  const ERROR_MESSAGE_NAME_EXCEEDS_MAXIMUM_LENGTH = `The name must be less or equal to ${MAXIMUM_NAME_LENGTH} characters.`;
  const ERROR_MESSAGE_NAME_LESS_THAN_MINIMUM_LENGTH = `The name must at least have one character.`;

  const MAXIMUM_DESCRIPTION_LENGTH = 512;
  const ERROR_MESSAGE_DESCRIPTION_EXCEEDS_MAXIMUM_LENGTH = `The name must be less or equal to ${MAXIMUM_DESCRIPTION_LENGTH} characters.`;

  const [name, setName] = useState('');
  const [description, setDescription] = useState('');

  const [nameHasError, setNameHasError] = useState(false);
  const [nameErrorMessage, setNameErrorMessage] = useState<string | null>();

  const [descriptionHasError, setDescriptionHasError] = useState(false);
  const [descriptionErrorMessage, setDescriptionErrorMessage] = useState<
    string | null
  >();

  const validateName = (name: string): ValidationResult => {
    const validationResult: ValidationResult = {
      isValid: false,
      isError: false,
      errorMessage: null,
    };

    if (name.length > MAXIMUM_NAME_LENGTH) {
      validationResult.isError = true;
      validationResult.errorMessage = ERROR_MESSAGE_NAME_EXCEEDS_MAXIMUM_LENGTH;
      return validationResult;
    }

    if (name.length < MINIMUM_NAME_LENGTH) {
      validationResult.isError = true;
      validationResult.errorMessage =
        ERROR_MESSAGE_NAME_LESS_THAN_MINIMUM_LENGTH;
      return validationResult;
    }

    validationResult.isValid = true;
    return validationResult;
  };

  const validateDescription = (description: string): ValidationResult => {
    const validationResult: ValidationResult = {
      isValid: false,
      isError: false,
      errorMessage: null,
    };

    if (description.length > MAXIMUM_DESCRIPTION_LENGTH) {
      validationResult.isError = true;
      validationResult.errorMessage =
        ERROR_MESSAGE_DESCRIPTION_EXCEEDS_MAXIMUM_LENGTH;
      return validationResult;
    }

    validationResult.isValid = true;
    return validationResult;
  };

  const onClickedSaveButton = () => {
    const nameValidationResult = validateName(name);
    setNameHasError(nameValidationResult.isError);
    setNameErrorMessage(nameValidationResult.errorMessage);

    let isDescriptionValid = true;
    if (description) {
      const descriptionValidationResult = validateDescription(description);

      isDescriptionValid = descriptionValidationResult.isValid;

      setDescriptionHasError(descriptionValidationResult.isError);
      setDescriptionErrorMessage(descriptionValidationResult.errorMessage);
    }

    if (nameValidationResult.isValid && isDescriptionValid) {
      createProjectAsync(name, description.length > 0 ? description : null);
      setName('');
      setDescription('');
    }
  };

  const onChangeNameTextField = (e: React.ChangeEvent<HTMLInputElement>) => {
    const nameValidationResult = validateName(e.target.value);
    setNameHasError(nameValidationResult.isError);
    setNameErrorMessage(nameValidationResult.errorMessage);
    setName(e.target.value);
  };

  const onChangeDescriptionTextField = (
    e: React.ChangeEvent<HTMLTextAreaElement>,
  ) => {
    const description = e.target.value;
    setDescription(description);

    if (description) {
      const descriptionValidationResult = validateDescription(e.target.value);
      setDescriptionHasError(descriptionValidationResult.isError);
      setDescriptionErrorMessage(descriptionValidationResult.errorMessage);
    }
  };

  return (
    <>
      <Typography variant="h4" textAlign="center" sx={{ mb: 1 }}>
        New project?
      </Typography>
      {isErrorCreatingProject && (
        <Alert severity="error" sx={{ mb: 2 }}>
          Failed to create a project. Please try again.
        </Alert>
      )}
      <Stack spacing={2}>
        <TextField
          id="new-project-name"
          label="Name"
          variant="outlined"
          value={name}
          onChange={onChangeNameTextField}
          error={nameHasError}
          helperText={nameErrorMessage}
        />
        <TextField
          id="new-project-description"
          label="Description"
          variant="outlined"
          multiline
          value={description}
          onChange={onChangeDescriptionTextField}
          error={descriptionHasError}
          helperText={descriptionErrorMessage}
        />
        <Button variant="contained" onClick={onClickedSaveButton}>
          Save
        </Button>
      </Stack>
    </>
  );
};

export default CreateProjectForm;
