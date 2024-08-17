using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static event Action<int> OnCurrencyChanged;
    public static event Action<int> OnExperienceChanged;
    public static event Action<int> OnPlayerLevelChanged;
    
    private PlayerData _playerData { get; set; }

    private void Awake()
    {
        LoadPlayerData();
    }

    private void Start()
    {
        OnCurrencyChanged?.Invoke(_playerData.Money);
        OnExperienceChanged?.Invoke(_playerData.Experience);
        OnPlayerLevelChanged?.Invoke(_playerData.Level);
        GameTickController.Instance.OnTick += SavePlayerData;
    }

    public void AddMoney(int amount)
    {
        if (amount <= 0) return;
        
        _playerData.Money += amount;
        OnCurrencyChanged?.Invoke(_playerData.Money);
    }

    public bool SpendMoney(int amount)
    {
        if (amount < 0 || _playerData.Money < amount) return false;
        
        _playerData.Money -= amount;
        OnCurrencyChanged?.Invoke(_playerData.Money);
        return true;
    }

    public void AddExperience(int amount)
    {
        if (amount <= 0) return;
        
        _playerData.Experience += amount;
        CheckLevelUp();
        OnExperienceChanged?.Invoke(_playerData.Experience);
    }

    private void CheckLevelUp()
    {
        if (GameManager.Instance.GetMaxPlayerLevel() <= _playerData.Level) return;
        int requiredExperience = GameManager.Instance.GetCurrentExperienceRequired();
        if (_playerData.Experience >= requiredExperience)
        {
            _playerData.Level++;
            _playerData.Experience -= requiredExperience;
            OnLevelUp();
        }
    }

    private void OnLevelUp() => OnPlayerLevelChanged?.Invoke(_playerData.Level);

    private void LoadPlayerData() => _playerData = SaveSystem.LoadPlayerData();

    private void SavePlayerData() => SaveSystem.SavePlayerData(_playerData);

    public int GetCurrentLevel() => _playerData.Level;

    public int GetCurrentMoney() => _playerData.Money;
}

