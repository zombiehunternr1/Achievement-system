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
    [SerializeField] private EmptyEvent _resetEvent;
    [SerializeField] private DoubleEvent _updateStorageDataEvent;
    [SerializeField] private SingleEvent _updateProgressionEvent;
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
        _resetEvent.Invoke();
        SaveGame();
    }
    public void LoadGame()
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
        _updateStorageDataEvent.Invoke(_gameData, true);
        _updateProgressionEvent.Invoke(_gameData);
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
        _dataHandler.Save(_gameData, _selectedProfileId);
        _updateStorageDataEvent.Invoke(_gameData, false);
        _updateProgressionEvent.Invoke(_gameData);
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
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
