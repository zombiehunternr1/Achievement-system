using UnityEngine;

public class SaveLoadMenu : MonoBehaviour
{
    [SerializeField] private EventPackage _newGame;
    [SerializeField] private EventPackage _saveGame;
    [SerializeField] private EventPackage _loadGame;
    public void NewGame()
    {
        EventPackageFactory.BuildAndInvoke(_newGame);
    }
    public void SaveGame()
    {
        EventPackageFactory.BuildAndInvoke(_saveGame);
    }
    public void LoadGame()
    {
        EventPackageFactory.BuildAndInvoke(_loadGame);
    }
}
