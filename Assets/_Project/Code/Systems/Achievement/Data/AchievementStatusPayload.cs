public readonly struct AchievementStatusPayload
{
    public readonly AchievementType Achievement;
    public readonly bool IsUnlocked;
    public readonly string ProgressionDisplay;

    public AchievementStatusPayload(AchievementType achievement, bool isUnlocked, string progressionDisplay)
    {
        Achievement = achievement;
        IsUnlocked = isUnlocked;
        ProgressionDisplay = progressionDisplay;
    }
}