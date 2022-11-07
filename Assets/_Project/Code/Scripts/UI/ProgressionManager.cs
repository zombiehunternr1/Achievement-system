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
        _totalCompletionProgressionText.text = data.percentageTotalComplete.ToString() + "%";
        _totalCompletionProgressionSlider.maxValue = 100;
        _totalCompletionProgressionSlider.value = data.percentageTotalComplete;
        _collectionProgressionText.text = data.percentageCollectionComplete.ToString() + "%";
        _collectionProgressionSlider.maxValue = 100;
        _collectionProgressionSlider.value = data.percentageCollectionComplete;
        _achievementProgressionText.text = data.percentageAchievementsComplete.ToString() + "%";
        _achievementProgressionSlider.maxValue = 100;
        _achievementProgressionSlider.value = data.percentageAchievementsComplete;
    }
    public void UpdateData(GameData data, bool isLoading)
    {
        if (isLoading)
        {
            UpdateUIDisplay(data);
        }
    }
}
