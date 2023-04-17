public interface IEquip
{
    float useDuration { get; }
    void Set();
    void Use(EquipmentHandler handler);
    void Cancel();
}