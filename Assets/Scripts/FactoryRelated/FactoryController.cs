using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FactoryController : MonoBehaviour
{
    [SerializeField] private int _uniqueFactoryID;
    
    [Tooltip("Base Values")]
    [SerializeField] private int _buildCost = 100;
    [SerializeField] private int _factoryMergeLevel = 0;
    [SerializeField] private int _factoryProductionLevel = 0;
    [SerializeField] private int _factoryProductionAmount = 1;
    [SerializeField] private int _factoriesNeededAroundToBeAbleToBuild = 0;
    
    [Tooltip("UI")]
    [SerializeField] private Button _buildButton;
    [SerializeField] private Button _upgradeFactoryButton;
    [SerializeField] private Button _upgradeProductionAmountButton;
    [SerializeField] private Button _upgradeFactoryProductionLevelButton;
    [SerializeField] private TextMeshProUGUI _buildCostText;
    
    [Tooltip("References")]
    [SerializeField] private PartsDatabaseScriptableObject _partsDatabase;
    [SerializeField] private List<GameObject> _onReadyToBuildObjects;
    [SerializeField] private List<GameObject> _onBuiltObjects;
    [SerializeField] private List<ConveyorController> _conveyorControllers;
    [SerializeField] private List<FactoryController> _nextFactories;
    [SerializeField] private List<FactoryController> _nextFactoriesToActivateWhenBuilded;
    [SerializeField] private FactoryVisualsController _factoryVisualsController;

    private FactoryState _currentState = FactoryState.ReadyToBuild;
    private readonly List<Part> _holdingParts = new();
    private int _currentConveyorIndex = 0;
    private Coroutine _currentSendPartsToConveyorsCoroutine;
    
    public int UniqueFactoryID => _uniqueFactoryID;

    private void Start()
    {
        _buildButton.onClick.AddListener(BuildFactory);
        _upgradeFactoryButton.onClick.AddListener(UpgradeFactory);
        _upgradeProductionAmountButton.onClick.AddListener(UpgradeFactoryProductionAmount);
        _upgradeFactoryProductionLevelButton.onClick.AddListener(UpgradeFactoryProductionLevel);
        
        GameTickController.Instance.OnTick += OnGameTick;

        for (int i = 0; i < _nextFactories.Count; i++) _conveyorControllers[i].SetNextFactory(_nextFactories[i]);

        UpdateFactoryState();
    }

    private void OnDestroy()
    {
        _buildButton.onClick.RemoveAllListeners();
        _upgradeFactoryButton.onClick.RemoveAllListeners();
        _upgradeProductionAmountButton.onClick.RemoveAllListeners();
        _upgradeFactoryProductionLevelButton.onClick.RemoveAllListeners();
        
        GameTickController.Instance.OnTick -= OnGameTick;
    }

    public FactoryData GetFactoryData()
    {
        return new FactoryData
        {
            factoryID = _uniqueFactoryID,
            factoryState = _currentState,
            mergeLevel = _factoryMergeLevel,
            productionLevel = _factoryProductionLevel,
            productionAmount = _factoryProductionAmount
        };
    }

    public void LoadFactoryData(FactoryData data)
    {
        _currentState = data.factoryState;
        _factoryMergeLevel = data.mergeLevel;
        _factoryProductionLevel = data.productionLevel;
        _factoryProductionAmount = data.productionAmount;

        // Обновление состояния фабрики на основе загруженных данных
        UpdateFactoryState();
    }
    
    private void PingActivateFactory()
    {
        if (_factoriesNeededAroundToBeAbleToBuild <= 0) return;

        _buildButton.gameObject.SetActive(--_factoriesNeededAroundToBeAbleToBuild <= 0);
    }

    public void PingNextFactories()
    {
        foreach (var factory in _nextFactoriesToActivateWhenBuilded) factory.PingActivateFactory();
    }
    
    private void BuildFactory()
    {
        if (_currentState != FactoryState.ReadyToBuild) return;
        if (GameManager.Instance.SpendMoney(_buildCost))
        {
            _buildButton.gameObject.SetActive(false);
            _currentState = FactoryState.Built;
            UpdateFactoryState();
            PingNextFactories();
        }
        else Debug.Log("Not enough currency to build the factory.");
    }

    public void ReceivePart(Part part) => _holdingParts.Add(part);

    private void OnGameTick()
    {
        if (_currentState != FactoryState.Built) return;

        ProducePart();

        if (_holdingParts.Count - _conveyorControllers.Count + 1 >= 2) MergePartsInFactory();
        if (_holdingParts.Count > 0 && _currentSendPartsToConveyorsCoroutine == null) _currentSendPartsToConveyorsCoroutine = StartCoroutine(SendPartsToConveyors());
    }
    
    private void ProducePart()
    {
        PartModel newPartModel = _partsDatabase.GetPartByLevel(_factoryProductionLevel);
        if (newPartModel != null)
        {
            for (int i = 0; i < _factoryProductionAmount; i++)//_conveyorControllers.Count - (_conveyorControllers.Count - _factoryProductionAmount); i++)
            {
                _holdingParts.Add(PartPool.Instance.GetPart(newPartModel.Level));
            }
        }
    }

    private IEnumerator SendPartsToConveyors()
    {
        List<Part> tempPartsArray = new List<Part>(_holdingParts);
        _holdingParts.Clear();
        foreach (var part in tempPartsArray)
        {
            _conveyorControllers[_currentConveyorIndex].ReceivePart(part);
            if (_currentConveyorIndex == _conveyorControllers.Count - 1 || _factoryProductionAmount < _conveyorControllers.Count) yield return new WaitForSeconds(0.1f);
            _currentConveyorIndex = _currentConveyorIndex >= _conveyorControllers.Count - 1 ? 0 : _currentConveyorIndex + 1;
        }
        _currentSendPartsToConveyorsCoroutine = _holdingParts.Count <= 0 ? null : StartCoroutine(SendPartsToConveyors());
    }

    private void MergePartsInFactory()
    {
        _holdingParts.Sort((a, b) => a.tPartModel.Level.CompareTo(b.tPartModel.Level));

        for (int i = 0; i < _holdingParts.Count - 1; i++)
        {
            if (_holdingParts[i].tPartModel.Level == _holdingParts[i + 1].tPartModel.Level && _holdingParts[i].tPartModel.Level < _factoryMergeLevel)
            {
                int newLevel = _holdingParts[i].tPartModel.Level + 1;

                PartModel newPartModel = _partsDatabase.GetPartByLevel(newLevel);
                if (newPartModel != null)
                {
                    PartPool.Instance.ReturnPart(_holdingParts[i]);
                    PartPool.Instance.ReturnPart(_holdingParts[i + 1]);

                    _holdingParts.Add(PartPool.Instance.GetPart(newPartModel.Level));

                    _holdingParts.RemoveAt(i + 1);
                    _holdingParts.RemoveAt(i);

                    break;
                }
            }
        }
    }

    private void UpdateFactoryState()
    {
        var isBuilt = IsBuilt();

        _buildCostText.text = _buildCost.ToString();
        _buildButton.gameObject.SetActive(_factoriesNeededAroundToBeAbleToBuild <= 0);
        foreach (var gm in _onReadyToBuildObjects) gm.SetActive(!isBuilt);
        foreach (var gm in _onBuiltObjects) gm.SetActive(isBuilt);
    }

    private void UpgradeFactory()
    {
        _factoryMergeLevel++;
        _factoryVisualsController.UpdateFactoryLevelVisual(_factoryMergeLevel);
        if (_factoryMergeLevel >= _partsDatabase.GetMaxPartLevel()) _upgradeFactoryButton.gameObject.SetActive(false);
    }

    private void UpgradeFactoryProductionAmount()
    {
        _factoryProductionAmount++;
    }

    private void UpgradeFactoryProductionLevel()
    {
        _factoryProductionLevel++;
        _factoryVisualsController.UpdateProductionLevelVisual(_factoryProductionLevel);
        if (_factoryProductionLevel >= _partsDatabase.GetMaxPartLevel()) _upgradeFactoryProductionLevelButton.gameObject.SetActive(false);
    }

    public bool IsBuilt() => _currentState == FactoryState.Built;
}

public class Part
{
    public PartModel tPartModel;
    public GameObject thisPart;
}

public enum FactoryState
{
    ReadyToBuild,
    Built
}
