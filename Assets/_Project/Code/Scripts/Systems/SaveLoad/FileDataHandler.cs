using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class FileDataHandler
{
    private string _dataDirPath = "";
    private string _dataFileName = "";
    private bool _useEncryption = false;
    private readonly string _encryptionCodeWord = "word";
    private readonly string _backupExtension = ".bak";
    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
    {
        _dataDirPath = dataDirPath;
        _dataFileName = dataFileName;
        _useEncryption = useEncryption;
    }
    public GameData Load(string profileId, bool allowRestoreFromBackup = true)
    {
        if(profileId == null)
        {
            return null;
        }
        string fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
        GameData loadedData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using FileStream stream = new(fullPath, FileMode.Open);
                using StreamReader reader = new(stream);
                dataToLoad = reader.ReadToEnd();
                if (_useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception exceptionMessage)
            {
                if (allowRestoreFromBackup)
                {
                    Debug.LogWarning("Failed to load data file. Attempting to roll back.\n" + exceptionMessage);
                    if (AttemptRollBack(fullPath))
                    {
                        loadedData = Load(profileId, false);
                    }
                }
                else
                {
                    Debug.LogError("Error occured when trying to load file at path: " + fullPath + " and backup did not work.\n" + exceptionMessage);
                }
            }
        }
        return loadedData;
    }
    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(_dataDirPath).EnumerateDirectories();
        foreach(DirectoryInfo dirInfo in dirInfos)
        {
            string profileId = dirInfo.Name;
            string fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all profiles because it does not contain data: " + profileId + "!");
                continue;
            }
            GameData profileData = Load(profileId);
            if(profileData != null)
            {
                profileDictionary.Add(profileId, profileData);
            }
            else
            {
                Debug.LogError("Tried to load profile but something went wrong with profile ID: " + profileId + "!");
            }
        }
        return profileDictionary;
    }
    public string MostRecentlyUpdatedProfileId
    {
        get
        {
            string mostRecentlyprofileId = null;
            Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
            foreach(KeyValuePair<string, GameData> pair in profilesGameData)
            {
                string profileId = pair.Key;
                GameData gameData = pair.Value;
                if(gameData == null)
                {
                    continue;
                }
                if(mostRecentlyprofileId == null)
                {
                    mostRecentlyprofileId = profileId;
                }
                else
                {
                    DateTime mostRecentDataTime = DateTime.FromBinary(profilesGameData[mostRecentlyprofileId].LastUpdated);
                    DateTime newDataTime = DateTime.FromBinary(gameData.LastUpdated);
                    if(newDataTime > mostRecentDataTime)
                    {
                        mostRecentlyprofileId = profileId;
                    }
                }
            }
            return mostRecentlyprofileId;
        }
    }
    public void Save(GameData data, string profileId)
    {
        if(profileId == null)
        {
            return;
        }
        string fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
        string backupPath = fullPath + _backupExtension;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(data, true);
            if (_useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }
            using (FileStream stream = new(fullPath, FileMode.Create))
            {
                using StreamWriter writer = new(stream);
                writer.Write(dataToStore);
            }
            GameData verifiedGameData = Load(profileId);
            if (verifiedGameData != null)
            {
                File.Copy(fullPath, backupPath, true);
            }
            else
            {
                throw new Exception("Save file could not be verified and backup could not be created!");
            }
        }
        catch (Exception exceptionMessage)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + exceptionMessage + "!");
        }
    }
    public void Delete(string profileId)
    {
        if(profileId == null)
        {
            return;
        }
        string fullPath = Path.Combine(_dataDirPath, profileId, _dataFileName);
        try
        {
            if (File.Exists(fullPath))
            {
                Directory.Delete(Path.GetDirectoryName(fullPath), true);
            }
            else
            {
                Debug.LogWarning("Tried to delete profile data, but data was not found at path: " + fullPath + "!");
            }
        }
        catch (Exception exceptionMessage)
        {
            Debug.LogError("Failed to delete profile data for profile ID: " + profileId + " at path: " + fullPath + "\n" + exceptionMessage + "!");
        }
    }
    private string EncryptDecrypt(string data)
    {
        StringBuilder modifiedData = new StringBuilder(data.Length);
        int keyLength = _encryptionCodeWord.Length;
        for(int i = 0; i < data.Length; i++)
        {
            int keyIndex = i - (i / keyLength) * keyLength;
            char modifiedChar;
            if (data[i] >= _encryptionCodeWord[keyIndex])
            {
                modifiedChar = (char)(data[i] - _encryptionCodeWord[keyIndex]);
            }
            else
            {
                modifiedChar = (char)(data[i] + _encryptionCodeWord[keyIndex]);
            }
            modifiedData.Append(modifiedChar);
        }
        return modifiedData.ToString();
    }
    private bool AttemptRollBack(string fullPath)
    {
        bool success = false;
        string backupFilePath = fullPath + _backupExtension;
        try
        {
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                success = true;
                Debug.LogWarning("Had to roll back up to backup file at: " + backupFilePath + "!");
            }
            else
            {
                throw new Exception("Tried to roll back, but no backup file exists to roll back to!");
            }
        }
        catch(Exception exceptionMessage)
        {
            Debug.LogError("Error occured when trying to roll back to backup file at: " + backupFilePath + "\n" + exceptionMessage + "!");
        }
        return success;
    }
}
