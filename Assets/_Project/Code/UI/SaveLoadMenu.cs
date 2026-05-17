using UnityEngine;

public class SaveLoadMenu : MonoBehaviour
{
    [SerializeField] private EventChannel _newGame;
    [SerializeField] private EventChannel _saveGame;
    [SerializeField] private EventChannel _loadGame;
    public void NewGame()
    {
        EventDispatcher.Raise(_newGame);
    }
    public void SaveGame()
    {
        EventDispatcher.Raise(_saveGame);
    }
    public void LoadGame()
    {
        EventDispatcher.Raise(_loadGame);
    }
}
