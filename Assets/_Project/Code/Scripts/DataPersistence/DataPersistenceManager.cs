using System.Collections.Generic;
using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    [SerializeField] private SaveAndLoadEvent _updateStorageDataEvent;
    [Header("Debugging")]
    [SerializeField] private bool _disableDataPersistence = false;
    [SerializeField] private bool _initializeDataIfNull = false;
    [SerializeField] private bool _overrideSelectedProfileId = false;
    [SerializeField] private string _testSelectedProfileId = "Test";
    [Header("File Storage Config")]
    [SerializeField] private string _fileName;
    [SerializeField] private bool _useEncryption;
    private GameData _gameData;
    private FileDataHandler _dataHandler;
    private string _selectedProfileId = "";
    private static DataPersistenceManager _instance;
    public bool HasGameData
    {
        get
        {
            return _gameData != null;
        }
    }
    public Dictionary<string, GameData> GetAllProfilesGameData
    {
        get
        {
            return _dataHandler.LoadAllProfiles();
        }
    }
    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        if (_disableDataPersistence)
        {
            Debug.LogWarning("Data persistence is currently disabled!");
        }
        _dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName, _useEncryption);
        InitializeSelectedProfileId();
        LoadGame();
    }
    public void NewGame()
    {
        _gameData = new GameData();
        SaveGame();
    }
    public void LoadGame()
    {
        if (_disableDataPersistence)
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
        _updateStorageDataEvent.Invoke(_gameData, true);
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
        _updateStorageDataEvent.Invoke(_gameData, false);
        //_updateStorageDataEvent.RaiseSaveLoadEvent(_gameData, false);
        _gameData.LastUpdated = System.DateTime.Now.ToBinary();
        _dataHandler.Save(_gameData, _selectedProfileId);
    }
    private void InitializeSelectedProfileId()
    {
        _selectedProfileId = _dataHandler.MostRecentlyUpdatedProfileId;
        if (_overrideSelectedProfileId)
        {
            _selectedProfileId = _testSelectedProfileId;
            Debug.LogWarning("Overrode selected profile ID with test ID: " + _testSelectedProfileId);
        }
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}