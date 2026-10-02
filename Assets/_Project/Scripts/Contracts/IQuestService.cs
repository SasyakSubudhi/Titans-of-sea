namespace TitansOfTheSea.Contracts
{
    public struct QuestInfo { public string Id, Title, Description, CurrentObjective; public bool Completed; }
    public interface IQuestService
    {
        System.Collections.Generic.IReadOnlyList<QuestInfo> Active { get; }
        System.Collections.Generic.IReadOnlyList<QuestInfo> Completed { get; }
        event System.Action Changed;
    }
}
