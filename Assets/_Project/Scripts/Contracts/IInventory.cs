namespace TitansOfTheSea.Contracts
{
    public interface IInventory
    {
        int Gold { get; }
        int GetCount(string itemId);
        bool TryAdd(string itemId, int amount);
        bool TryRemove(string itemId, int amount);
        System.Collections.Generic.IReadOnlyList<(string itemId, int count)> All { get; }
        event System.Action Changed;
    }
}
