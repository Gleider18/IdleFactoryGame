using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string SaveFileName = "playerSave.json";

    public static void SavePlayerData(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        File.WriteAllText(filePath, json);
    }

    public static PlayerData LoadPlayerData()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            return new PlayerData
            {
                Money = 0,
                Experience = 0,
                Level = 1
            };
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public int Money;
    public int Experience;
    public int Level;
}

