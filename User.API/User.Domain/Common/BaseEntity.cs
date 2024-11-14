namespace Users.Domain.Common;

public abstract class BaseEntity
{
    public int? Id { get; set; }
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}

