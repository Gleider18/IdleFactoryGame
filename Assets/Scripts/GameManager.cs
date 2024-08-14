using ScriptableObjects;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private PlayerController _playerController;

    private LevelExperienceDataScriptableObject _levelExperienceData;
    
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
            _levelExperienceData = Resources.Load<LevelExperienceDataScriptableObject>("LevelExperienceDataScriptableObject");

            if (_levelExperienceData == null)
            {
                throw new System.Exception("LevelExperienceDataScriptableObject not found in Resources!");
            }
            
            Debug.Log("LevelExperienceDataScriptableObject successfully loaded.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("An error occurred while loading LevelExperienceDataScriptableObject: " + ex.Message);
        }
    }

    public int GetCurrentExperienceRequired() => _levelExperienceData.GetExperienceRequiredByLevel(_playerController.GetCurrentLevel());

    public int GetMaxPlayerLevel() => _levelExperienceData.GetMaxPlayerLevel();

    public int GetCurrentMoney() => _playerController.GetCurrentMoney();

    public void AddMoney(int i) => _playerController.AddMoney(i);

    public bool SpendMoney(int i) => _playerController.SpendMoney(i);
}