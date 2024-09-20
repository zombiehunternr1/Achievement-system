using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionUIController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _totalCompletionProgressionText;
    [SerializeField] Slider _totalCompletionProgressionSlider;
    [SerializeField] TextMeshProUGUI _collectionProgressionText;
    [SerializeField] Slider _collectionProgressionSlider;
    [SerializeField] TextMeshProUGUI _achievementProgressionText;
    [SerializeField] Slider _achievementProgressionSlider;
    public void UpdateUIDisplay(object gameDataObj)
    {
        GameData gameData = (GameData)gameDataObj;
        _totalCompletionProgressionText.text = gameData.PercentageTotalComplete.ToString() + "%";
        _totalCompletionProgressionSlider.maxValue = 100;
        _totalCompletionProgressionSlider.value = gameData.PercentageTotalComplete;
        _collectionProgressionText.text = gameData.PercentageCollectionComplete.ToString() + "%";
        _collectionProgressionSlider.maxValue = 100;
        _collectionProgressionSlider.value = gameData.PercentageCollectionComplete;
        _achievementProgressionText.text = gameData.PercentageAchievementsComplete.ToString() + "%";
        _achievementProgressionSlider.maxValue = 100;
        _achievementProgressionSlider.value = gameData.PercentageAchievementsComplete;
    }
    public void UpdateData(object gameDataObj, object isLoadingObj)
    {
        GameData gameData = (GameData)gameDataObj;
        bool isLoading = (bool)isLoadingObj;
        if (isLoading)
        {
            UpdateUIDisplay(gameData);
        }
    }
}
