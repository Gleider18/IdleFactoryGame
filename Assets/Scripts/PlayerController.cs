using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static event Action<int> OnCurrencyChanged;
    public static event Action<int> OnExperienceChanged;
    public static event Action<int, bool> OnPlayerLevelChanged;
    
    private PlayerData _playerData { get; set; }
    private bool _isMaxLevel = false;

    private void Awake()
    {
        LoadPlayerData();
        _isMaxLevel = GameManager.Instance.GetMaxPlayerLevel() <= _playerData.Level;
    }

    private void Start()
    {
        OnCurrencyChanged?.Invoke(_playerData.Money);
        OnExperienceChanged?.Invoke(_playerData.Experience);
        OnLevelUp();
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
        if (amount <= 0 || _isMaxLevel) return;
        
        _playerData.Experience += amount;
        OnExperienceChanged?.Invoke(_playerData.Experience);
        CheckLevelUp();
    }

    private void CheckLevelUp()
    {
        int requiredExperience = GameManager.Instance.GetCurrentExperienceRequired();
        if (_playerData.Experience >= requiredExperience)
        {
            _playerData.Level++;
            _isMaxLevel = GameManager.Instance.GetMaxPlayerLevel() <= _playerData.Level;
            if (_isMaxLevel) OnExperienceChanged?.Invoke(-1);
            _playerData.Experience -= requiredExperience;
            OnLevelUp();
        }
    }

    private void OnLevelUp() => OnPlayerLevelChanged?.Invoke(_playerData.Level, _isMaxLevel);

    private void LoadPlayerData() => _playerData = SaveSystem.LoadPlayerData();

    private void SavePlayerData() => SaveSystem.SavePlayerData(_playerData);

    public int GetCurrentLevel() => _playerData.Level;

    public int GetCurrentMoney() => _playerData.Money;
}

