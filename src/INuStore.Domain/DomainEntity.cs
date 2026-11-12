namespace INuStore.Domain;
public abstract class DomainEntity<T>
{
    public virtual T Id { get; set; }

    public bool IsTransient()
    {
        return Id.Equals(default(T));
    }
}
