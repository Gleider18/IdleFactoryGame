using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerData _playerData { get; set; }

    public PlayerController() => LoadPlayerData();

    public void AddMoney(float amount)
    {
        if (amount <= 0) return;
        
        _playerData.Money += amount;
        SavePlayerData();
    }

    public bool SpendMoney(float amount)
    {
        if (amount < 0 || _playerData.Money < amount) return false;
        
        _playerData.Money -= amount;
        SavePlayerData();
        return true;

    }

    public void AddExperience(int amount)
    {
        if (amount <= 0) return;
        
        _playerData.Experience += amount;
        CheckLevelUp();
        SavePlayerData();
    }

    private void CheckLevelUp()
    {
        if (GameManager.Instance.GetMaxPlayerLevel() >= _playerData.Level)
        {
            return;
        }
        int requiredExperience = GameManager.Instance.GetExperienceRequired(_playerData.Level);
        if (_playerData.Experience >= requiredExperience)
        {
            _playerData.Level++;
            _playerData.Experience -= requiredExperience;
            OnLevelUp();
            SavePlayerData();
        }
    }

    private void OnLevelUp() { }

    private void LoadPlayerData() => _playerData = SaveSystem.LoadPlayerData();

    private void SavePlayerData() => SaveSystem.SavePlayerData(_playerData);
}

