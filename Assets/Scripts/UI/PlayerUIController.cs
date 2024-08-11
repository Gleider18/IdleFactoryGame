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

        private void UpdateCurrencyUI(int newCurrency)
        {
            _currencyText.text = "Coins: " + newCurrency;
        }

        private void UpdateExperienceUI(int newExperience)
        {
            _experienceSlider.value = newExperience;
        }

        private void UpdateLevelUI(int newLevel)
        {
            _levelText.text = newLevel.ToString();
        }

        private void SetMaxExperience(int maxExperience)
        {
            _experienceSlider.maxValue = maxExperience;
        }
    }
}