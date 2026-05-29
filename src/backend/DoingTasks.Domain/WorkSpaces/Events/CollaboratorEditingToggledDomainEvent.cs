using DoingTasks.SharedKernel.Domain;

namespace DoingTasks.Domain;

/// <summary>
/// Domain event raised when collaborator editing permission is toggled.
/// </summary>
public sealed record CollaboratorEditingToggledDomainEvent(
    Guid WorkspaceId,
    bool AllowCollaboratorEditing) : IDomainEvent;
