using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartsDatabase", menuName = "ScriptableObjects/PartsDatabase")]
public class PartsDatabaseScriptableObject : ScriptableObject
{
    public List<PartModel> parts; // Список всех возможных деталей

    public PartModel GetPartByLevel(int level)
    {
        // Возвращаем первую найденную деталь с нужным уровнем
        return parts.Find(part => part.level == level);
    }
}

[System.Serializable]
public class PartModel
{
    public int level;       // Уровень детали
    public string name;     // Название детали
    public GameObject prefab;  // Префаб детали
}
