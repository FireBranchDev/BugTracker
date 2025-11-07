import { Box, Button, Stack, TextField } from '@mui/material';
import { useMutation } from '@tanstack/react-query';
import { Model, Store } from 'json-api-models';
import { type FC } from 'react';
import { useForm, type SubmitHandler } from 'react-hook-form';
import type { Bug as BugType } from '../../../types/bug';
import { useAxios } from '../../AxiosProvider/AxiosProvider';

type Props = {
  projectId: string | undefined;
  addCreatedBug: (bug: BugType) => void;
};

type Inputs = {
  title: string;
  description: string;
};

type NewBug = {
  title: string;
  description: string | undefined;
};

type BugsSchema = {
  type: 'bugs';
  id: string;
  attributes: {
    title: string;
    description: string | undefined;
    created: string;
    updated: string;
  };
};

class BugModel extends Model<BugsSchema, Schemas> {}

type Schemas = {
  bugs: BugModel;
};

const models = new Store<Schemas>({
  bugs: BugModel,
});

const CreateBugForm: FC<Props> = ({ projectId, addCreatedBug }) => {
  const axios = useAxios();

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<Inputs>({
    mode: 'onChange',
  });

  const mutation = useMutation({
    mutationFn: (newBug: NewBug) => {
      let attributes = {
        title: newBug.title,
        description: newBug.description,
      } as { title: string; description: string | undefined };

      return axios.post(
        `/bugs/${projectId}`,
        {
          data: {
            type: 'bug',
            attributes,
          },
        },
        {
          headers: {
            'Content-Type': 'application/vnd.api+json',
            Accept: 'application/vnd.api+json',
          },
        },
      );
    },
  });

  const onSubmit: SubmitHandler<Inputs> = async (data) => {
    const { title, description } = data;

    const newBug: NewBug = {
      title,
      description: description.trim().length > 0 ? description : undefined,
    };

    try {
      const result = await mutation.mutateAsync(newBug);
      const model = models.sync(result.data);
      if (model) {
        if (!Array.isArray(model)) {
          const bug: BugType = {
            id: model.id,
            title: model.title,
            description: model.description,
            created: model.created,
            updated: model.updated,
          };

          addCreatedBug(bug);

          reset();
        }
      }
    } catch (ex) {
      console.log(ex);
    }
  };

  return (
    <Box>
      {mutation.isError ? (
        <div>An error occurred: {mutation.error.message}</div>
      ) : null}
      <Stack
        component="form"
        noValidate
        autoComplete="off"
        sx={{
          p: 1,
          width: {
            lg: '50%',
            xl: '35%',
          },
        }}
        spacing={1}
        onSubmit={handleSubmit(onSubmit)}
      >
        <TextField
          required
          id="create-bug-form-title-field"
          label="Title"
          placeholder="Title"
          {...register('title', { required: true })}
          error={errors.title != undefined}
          helperText={errors.title && 'This field is required'}
        />
        <TextField
          id="create-bug-form-description-field"
          label="Description"
          placeholder="Description"
          multiline
          {...register('description')}
        />
        <Button type="submit">Submit</Button>
      </Stack>
    </Box>
  );
};

export default CreateBugForm;
