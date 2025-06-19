export type User = {
  id: number;
  displayName: string;
};

export type Collaborator = {
  isOwner: boolean;
  joined: string;
} & User;

export type SetCollaboratorsCallback = (
  prev: Array<Collaborator>,
) => Array<Collaborator>;
