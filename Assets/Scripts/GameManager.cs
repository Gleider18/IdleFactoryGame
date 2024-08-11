using ScriptableObjects;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private LevelExperienceDataScriptableObject _levelExperienceData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLevelExperienceData();
        }
        else Destroy(gameObject);
    }

    private void LoadLevelExperienceData()
    {
        try
        {
            _levelExperienceData = Resources.Load<LevelExperienceDataScriptableObject>("LevelExperienceData");

            if (_levelExperienceData == null)
            {
                throw new System.Exception("LevelExperienceData not found in Resources!");
            }
            
            Debug.Log("LevelExperienceData successfully loaded.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred while loading LevelExperienceData: " + ex.Message);
        }
    }

    public int GetExperienceRequired(int level) => _levelExperienceData.GetExperienceRequiredByLevel(level);

    public int GetMaxPlayerLevel() => _levelExperienceData.GetMaxPlayerLevel();
}