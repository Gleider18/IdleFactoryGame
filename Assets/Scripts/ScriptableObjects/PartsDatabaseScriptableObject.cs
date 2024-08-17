using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PartsDatabase", menuName = "ScriptableObjects/PartsDatabase")]
public class PartsDatabaseScriptableObject : ScriptableObject
{
    [SerializeField] private List<PartModel> _parts;

    public PartModel GetPartByLevel(int level) => _parts.Find(part => part.Level == level);

    public int GetMaxPartLevel() => _parts.Count - 1;
}

[System.Serializable]
public class PartModel
{
    public int Level;
    public int Price;
    public GameObject PartPrefab;
}
