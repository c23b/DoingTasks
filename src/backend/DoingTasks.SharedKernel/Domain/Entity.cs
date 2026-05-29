namespace DoingTasks.SharedKernel.Domain;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    protected Entity()
    {
        Id = Guid.NewGuid();
    }
    internal void SetCreatedAt(DateTime createdAt) => CreatedAt = createdAt;
    internal void SetUpdatedAt(DateTime updatedAt) => UpdatedAt = updatedAt;
}
