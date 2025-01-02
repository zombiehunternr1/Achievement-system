using System.Threading.Tasks;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string _fileName;
    [SerializeField] private bool _useEncryption;
    private GameData _gameData;
    private FileDataHandler _dataHandler;
    private string _selectedProfileId = "";
    [Header("Debugging")]
    [SerializeField] private bool _disableDataPersistence = false;
    [SerializeField] private bool _initializeDataIfNull = false;
    [SerializeField] private bool _overrideSelectedProfileId = false;
    [SerializeField] private string _profileName = "Game Data";
    [Header("Event references")]
    [SerializeField] private EventPackage _resetGame;
    [SerializeField] private EventPackage _updateAchievementData;
    [SerializeField] private EventPackage _updateCollectableData;
    [SerializeField] private EventPackage _updateProgression;
    private void Awake()
    {
        if (_disableDataPersistence)
        {
            Debug.LogWarning("Data persistence is currently disabled!");
        }
        _dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useEncryption);
        InitializeSelectedProfileId();
    }
    private void Start()
    {
        LoadGame();
    }
    public void NewGame()
    {
        _gameData = new GameData();
        EventPackageFactory.BuildAndInvoke(_resetGame);
        SaveGame();
    }
    public async void LoadGame()
    {
        if(_disableDataPersistence)
        {
            return;
        }
        _gameData = _dataHandler.Load(_selectedProfileId);
        if(_gameData == null && _initializeDataIfNull)
        {
            NewGame();
        }
        if(_gameData == null)
        {
            return;
        }
        await LoadAllSystems();
        EventPackageFactory.BuildAndInvoke(_updateProgression, _gameData);
    }
    public void SaveGame()
    {
        if (_disableDataPersistence)
        {
            return;
        }
        if(_gameData == null)
        {
            Debug.LogWarning("No data was found! A new game needs to be started before data can be saved!");
        }
        _gameData.SetLastUpdated(System.DateTime.Now.ToBinary());
        SaveAllSystems();
        _dataHandler.Save(_gameData, _selectedProfileId);
        EventPackageFactory.BuildAndInvoke (_updateProgression, _gameData);
    }
    private void InitializeSelectedProfileId()
    {
        _selectedProfileId = _dataHandler.MostRecentlyUpdatedProfileId;
        if(_overrideSelectedProfileId)
        {
            _selectedProfileId = _profileName;
            Debug.LogWarning("Overrode selected profile ID with test ID: " + _profileName);
        }
    }
    private async Task LoadAllSystems()
    {
        EventPackageFactory.BuildAndInvoke(_updateCollectableData, _gameData, true);
        await Task.Yield();
        EventPackageFactory.BuildAndInvoke(_updateAchievementData, _gameData, true);
    }
    private void SaveAllSystems()
    {
        EventPackageFactory.BuildAndInvoke(_updateCollectableData, _gameData, false);
        EventPackageFactory.BuildAndInvoke(_updateAchievementData, _gameData, false);
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
