using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartsDatabase", menuName = "ScriptableObjects/PartsDatabase")]
public class PartsDatabaseScriptableObject : ScriptableObject
{
    [SerializeField] private List<PartModel> _parts;

    public PartModel GetPartByLevel(int level)
    {
        if (level >= 0 && level < _parts.Count) return _parts[level];
        else throw new Exception($"There is no {level} level in parts database!");
    }

    public int GetMaxPartLevel() => _parts.Count - 1;
}

[System.Serializable]
public class PartModel
{
    public int Level;
    public int Price;
    public GameObject PartPrefab;
}
