using System.Collections.Generic;
using UnityEngine;

public class PartPool : MonoBehaviour
{
    public static PartPool Instance { get; private set; }
    
    [SerializeField] private PartsDatabaseScriptableObject _partsDatabase;

    private readonly List<Part> _availableParts = new();
    private List<Part> _usedParts = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public Part GetPart(int level)
    {
        for (int i = 0; i < _availableParts.Count; i++)
        {
            if (_availableParts[i].tPartModel.Level == level)
            {
                Part part = _availableParts[i];
                _availableParts.RemoveAt(i);
                _usedParts.Add(part);
                return part;
            }
        }

        Part newPart = CreateNewPart(level);
        _usedParts.Add(newPart);
        return newPart;
    }

    public void ReturnPart(Part part)
    {
        _usedParts.Remove(part);
        _availableParts.Add(part);
        part.thisPart.SetActive(false);
    }

    private Part CreateNewPart(int level)
    {
        PartModel partModel = _partsDatabase.GetPartByLevel(level);
        GameObject partObject = Instantiate(partModel.PartPrefab);
        partObject.SetActive(false);
        return new Part { tPartModel = partModel, thisPart = partObject };
    }
}
