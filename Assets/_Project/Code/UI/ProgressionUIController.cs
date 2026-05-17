using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionUIController : MonoBehaviour
{
    [Header("Total Completion")]
    [SerializeField] private TextMeshProUGUI _totalCompletionProgressionText;
    [SerializeField] private Slider _totalCompletionProgressionSlider;

    [Header("Collection")]
    [SerializeField] private TextMeshProUGUI _collectionProgressionText;
    [SerializeField] private Slider _collectionProgressionSlider;

    [Header("Achievements")]
    [SerializeField] private TextMeshProUGUI _achievementProgressionText;
    [SerializeField] private Slider _achievementProgressionSlider;

    public void UpdateUIDisplay(EventPayload payload)
    {
        GameData gameData = EventReader.Get<GameData>(payload);

        _totalCompletionProgressionText.text = gameData.PercentageTotalComplete.ToString() + "%";
        _totalCompletionProgressionSlider.value = gameData.PercentageTotalComplete;

        _collectionProgressionText.text = gameData.PercentageCollectionComplete.ToString() + "%";
        _collectionProgressionSlider.value = gameData.PercentageCollectionComplete;

        _achievementProgressionText.text = gameData.PercentageAchievementsComplete.ToString() + "%";
        _achievementProgressionSlider.value = gameData.PercentageAchievementsComplete;
    }

    public void UpdateData(EventPayload payload)
    {
        bool isLoading = EventReader.Get<bool>(payload);

        if (isLoading)
        {
            UpdateUIDisplay(payload);
        }
    }
}