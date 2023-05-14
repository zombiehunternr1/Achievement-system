using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    [SerializeField] private GenericEmptyEvent _resetEvent;
    [SerializeField] private SaveAndLoadEvent _updateStorageDataEvent;
    [SerializeField] private UpdateProgressionEvent _updateProgressionEvent;
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
        _gameData.LastUpdated = System.DateTime.Now.ToBinary();
        _dataHandler.Save(_gameData, _selectedProfileId);
        _updateStorageDataEvent.Invoke(_gameData, false);
        _updateProgressionEvent.Invoke(_gameData);
    }
    private void InitializeSelectedProfileId()
    {
        _selectedProfileId = _dataHandler.mostRecentlyUpdatedProfileId;
        if(_overrideSelectedProfileId)
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
