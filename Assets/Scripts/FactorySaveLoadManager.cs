using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FactorySaveLoadManager : MonoBehaviour
{
    [SerializeField] private List<FactoryController> allFactories; // Список всех фабрик на сцене

    private const string saveFileName = "factories_save_data.json";

    private void Start()
    {
        LoadFactoriesState();
    }

    // Сохранение состояния всех фабрик
    public void SaveFactoriesState()
    {
        var factoriesData = new List<FactoryData>();

        // Собираем данные со всех фабрик на сцене
        foreach (var factory in allFactories)
        {
            factoriesData.Add(factory.GetFactoryData());
        }

        // Преобразуем список данных в JSON и сохраняем в файл
        string json = JsonUtility.ToJson(new FactoryDataCollection { Factories = factoriesData }, true);
        File.WriteAllText(Application.persistentDataPath + "/" + saveFileName, json);
        Debug.Log("Factories saved to " + Application.persistentDataPath + "/" + saveFileName);
    }

    // Загрузка состояния всех фабрик
    public void LoadFactoriesState()
    {
        string filePath = Application.persistentDataPath + "/" + saveFileName;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            FactoryDataCollection loadedData = JsonUtility.FromJson<FactoryDataCollection>(json);

            foreach (var factoryData in loadedData.Factories)
            {
                FactoryController factory = allFactories.Find(f => f.UniqueFactoryID == factoryData.factoryID);
                if (factory != null)
                {
                    factory.LoadFactoryData(factoryData);
                    if (factory.IsBuilt()) factory.PingNextFactories();
                }
            }
        }
        else
        {
            Debug.Log("Save file not found at " + filePath);
        }
    }

    private void OnApplicationQuit()
    {
        SaveFactoriesState();
    }
}

[System.Serializable]
public class FactoryDataCollection
{
    public List<FactoryData> Factories;  // Список всех фабрик для сохранения
}

[System.Serializable]
public class FactoryData
{
    public int factoryID;              // Уникальный идентификатор фабрики
    public FactoryState factoryState;  // Текущее состояние фабрики
    public int mergeLevel;             // Уровень слияния фабрики
    public int productionLevel;        // Уровень производства фабрики
    public int productionAmount;       // Количество производимых деталей
}