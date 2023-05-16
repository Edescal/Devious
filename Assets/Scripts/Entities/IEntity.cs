namespace Edescal
{
    public interface IEntity : IDamageable
    {
        Inventory Inventory { get; }
        Health Health { get; }
    }
}
