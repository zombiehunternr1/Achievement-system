using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _totalCompletionProgressionText;
    [SerializeField] Slider _totalCompletionProgressionSlider;
    [SerializeField] TextMeshProUGUI _collectionProgressionText;
    [SerializeField] Slider _collectionProgressionSlider;
    [SerializeField] TextMeshProUGUI _achievementProgressionText;
    [SerializeField] Slider _achievementProgressionSlider;
    public void UpdateUIDisplay(GameData data)
    {
        _totalCompletionProgressionText.text = data.PercentageTotalComplete.ToString() + "%";
        _totalCompletionProgressionSlider.maxValue = 100;
        _totalCompletionProgressionSlider.value = data.PercentageTotalComplete;
        _collectionProgressionText.text = data.PercentageCollectionComplete.ToString() + "%";
        _collectionProgressionSlider.maxValue = 100;
        _collectionProgressionSlider.value = data.PercentageCollectionComplete;
        _achievementProgressionText.text = data.PercentageAchievementsComplete.ToString() + "%";
        _achievementProgressionSlider.maxValue = 100;
        _achievementProgressionSlider.value = data.PercentageAchievementsComplete;
    }
    public void UpdateData(GameData data, bool isLoading)
    {
        if (isLoading)
        {
            UpdateUIDisplay(data);
        }
    }
}
