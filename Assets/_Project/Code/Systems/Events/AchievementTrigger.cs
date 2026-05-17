public readonly struct AchievementTrigger
{
    public readonly string Id;
    public readonly object Value;

    public AchievementTrigger(string id, object value = null)
    {
        Id = id;
        Value = value;
    }
}