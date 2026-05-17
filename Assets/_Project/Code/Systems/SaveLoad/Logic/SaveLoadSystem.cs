using System;
using System.Threading.Tasks;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string _fileName;
    [SerializeField] private bool _useEncryption;

    [Header("Debugging")]
    [SerializeField] private bool _disableDataPersistence = false;
    [SerializeField] private bool _initializeDataIfNull = false;
    [SerializeField] private bool _overrideSelectedProfileId = false;
    [SerializeField] private string _profileName = "Game Data";

    [Header("Event Channels")]
    [SerializeField] private EventChannel _resetGame;
    [SerializeField] private EventChannel _updateAchievementData;
    [SerializeField] private EventChannel _updateCollectableData;
    [SerializeField] private EventChannel _updateProgression;

    private GameData _gameData;
    private FileDataHandler _dataHandler;
    private string _selectedProfileId = "";

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
        EventDispatcher.Raise(_resetGame);
        SaveGame();
    }

    public void LoadGame()
    {
        if (_disableDataPersistence)
        {
            return;
        }
        // Start async load of saved game state (fire-and-forget)
        // It will automatically clear itself after it finishes
        _ = LoadGameAsync();
    }

    private async Task LoadGameAsync()
    {
        _gameData = _dataHandler.Load(_selectedProfileId);

        if (_gameData == null && _initializeDataIfNull)
        {
            NewGame();
            return;
        }

        if (_gameData == null)
        {
            return;
        }

        await LoadAllSystems();
        EventDispatcher.Raise(_updateProgression, _gameData);
    }

    public void SaveGame()
    {
        if (_disableDataPersistence) return;

        if (_gameData == null)
        {
            Debug.LogWarning("No data was found! A new game needs to be started before data can be saved!");
            return;
        }

        _gameData.SetLastUpdated(DateTime.Now.ToBinary());
        SaveAllSystems();
        _dataHandler.Save(_gameData, _selectedProfileId);

        EventDispatcher.Raise(_updateProgression, _gameData);
    }

    private void InitializeSelectedProfileId()
    {
        _selectedProfileId = _dataHandler.MostRecentlyUpdatedProfileId;

        if (_overrideSelectedProfileId)
        {
            _selectedProfileId = _profileName;
            Debug.LogWarning("Overrode selected profile ID with test ID: " + _profileName);
        }
    }

    private async Task LoadAllSystems()
    {
        EventDispatcher.Raise(_updateCollectableData, _gameData, true);
        await Task.Yield();
        EventDispatcher.Raise(_updateAchievementData, _gameData, true);
    }

    private void SaveAllSystems()
    {
        EventDispatcher.Raise(_updateCollectableData, _gameData, false);
        EventDispatcher.Raise(_updateAchievementData, _gameData, false);
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}