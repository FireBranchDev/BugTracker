namespace ClassLib;

public static class ApiErrorMessages
{
    public const string MissingSubClaim = "The request is missing a sub claim.";
    public const string UserAccountAlreadyCreated = "A user account was already created.";
    public const string NoRecordOfAuth0UserId = "No record of the Auth0 user id being associated to a user account.";
    public const string NoRecordOfUserAccount = "There is no record of a user account being linked to this auth0 user id.";
    public const string ProjectNotFound = "The project was not found.";
    public const string UserNotProjectCollaborator = "The user is not a project collaborator.";
}
