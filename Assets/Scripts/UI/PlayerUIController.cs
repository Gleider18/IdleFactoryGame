using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currencyText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private Slider _experienceSlider;
        [SerializeField] private TextMeshProUGUI _experienceText;

        private int _maxExperience;
        private int _currentExperience;

        private void Start()
        {
            SetMaxExperience(GameManager.Instance.GetCurrentExperienceRequired());
        }

        private void OnEnable()
        {
            PlayerController.OnCurrencyChanged += UpdateCurrencyUI;
            PlayerController.OnExperienceChanged += UpdateExperienceUI;
            PlayerController.OnPlayerLevelChanged += UpdateLevelUI;
        }

        private void OnDisable()
        {
            PlayerController.OnCurrencyChanged -= UpdateCurrencyUI;
            PlayerController.OnExperienceChanged -= UpdateExperienceUI;
            PlayerController.OnPlayerLevelChanged -= UpdateLevelUI;
        }

        private void UpdateCurrencyUI(int newCurrency) => _currencyText.text = newCurrency.ToString();

        private void UpdateExperienceUI(int newExperience)
        {
            if (newExperience < 0)
            {
                _experienceText.text = "MAX!";
                return;
            }
            _currentExperience = newExperience;
            _experienceSlider.value = _currentExperience;
            UpdateExperienceText();
        }

        private void UpdateLevelUI(int newLevel, bool isMaxLevel = false)
        {
            _levelText.text = newLevel.ToString();
            if (!isMaxLevel) SetMaxExperience(GameManager.Instance.GetCurrentExperienceRequired());
        }

        private void SetMaxExperience(int maxExperience)
        {
            _maxExperience = maxExperience;
            _experienceSlider.maxValue = _maxExperience;
            UpdateExperienceText();
        }

        private void UpdateExperienceText() => _experienceText.text = _currentExperience + "/" + _maxExperience;
    }
}