using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactoryController : MonoBehaviour
{
    public enum FactoryState
    {
        ReadyToBuild,
        Built
    }

    public FactoryState currentState = FactoryState.ReadyToBuild;
    public int buildCost = 100;
    public int factoryLevel = 0;
    public int factoryProductionLevel = 0;
    public Button buildButton;
    public Button upgradeFactoryButton;
    public Button upgradeFactoryProductionLevelButton;
    public GameObject factoryBuilding;
    public GameObject buildCanvas;
    public GameObject upgradeCanvas;
    public Conveyor conveyor;
    public FactoryController nextFactory;

    public PartsDatabaseScriptableObject partsDatabase;

    private List<Part> incomingParts = new(); // Список деталей, пришедших на завод

    private void Start()
    {
        buildButton.onClick.AddListener(BuildFactory);
        upgradeFactoryButton.onClick.AddListener(UpgradeFactory);
        upgradeFactoryProductionLevelButton.onClick.AddListener(UpgradeFactoryProductionLevel);
        
        GameTickController.Instance.OnTick += ProcessParts;
        
        UpdateFactoryState();
    }

    private void OnDestroy()
    {
        buildButton.onClick.RemoveAllListeners();
        upgradeFactoryButton.onClick.RemoveAllListeners();
        upgradeFactoryProductionLevelButton.onClick.RemoveAllListeners();
        
        GameTickController.Instance.OnTick -= ProcessParts;
    }

    private void UpdateFactoryState()
    {
        var isBuilt = IsBuilt();
        
        buildCanvas.SetActive(!isBuilt);
        upgradeCanvas.SetActive(isBuilt);
        factoryBuilding.SetActive(isBuilt);
    }

    private void BuildFactory()
    {
        if (currentState == FactoryState.ReadyToBuild)
        {
            if (GameManager.Instance.SpendMoney(buildCost))
            {
                buildButton.gameObject.SetActive(false);
                currentState = FactoryState.Built;
                UpdateFactoryState();
            }
            else
            {
                Debug.Log("Not enough currency to build the factory.");
            }
        }
    }

    public void ReceivePart(Part part)
    {
        incomingParts.Add(part);
    }

    private void ProcessParts()
    {
        if (currentState != FactoryState.Built) return;
        print("ON TICK");

        ProducePart();

        if (incomingParts.Count >= 2)
        {
            incomingParts.Sort((a, b) => a.tPartModel.level.CompareTo(b.tPartModel.level));

            for (int i = 0; i < incomingParts.Count - 1; i++)
            {
                if (incomingParts[i].tPartModel.level == incomingParts[i + 1].tPartModel.level && incomingParts[i].tPartModel.level < factoryLevel)
                {
                    int newLevel = incomingParts[i].tPartModel.level + 1;

                    PartModel newPartModel = partsDatabase.GetPartByLevel(newLevel);
                    if (newPartModel != null)
                    {
                        // Уничтожаем старые детали
                        Destroy(incomingParts[i].thisPart.gameObject);
                        Destroy(incomingParts[i + 1].thisPart.gameObject);

                        // Создаем новую деталь
                        GameObject newPartObject = Instantiate(newPartModel.prefab, transform.position, Quaternion.identity);

                        // Добавляем новую деталь в список
                        incomingParts.Add(new Part()
                        {
                            thisPart = newPartObject,
                            tPartModel = newPartModel
                        });

                        // Удаляем старые детали из списка
                        incomingParts.RemoveAt(i + 1);
                        incomingParts.RemoveAt(i);

                        break;
                    }
                }
            }
        }

        if (incomingParts.Count > 0)
        {
            foreach (var part in incomingParts) conveyor.ReceivePart(part);
            incomingParts.Clear();
        }
    }
    
    private void ProducePart()
    {
        PartModel newPartModel = partsDatabase.GetPartByLevel(factoryProductionLevel);
        if (newPartModel != null)
        {
            GameObject newPartObject = Instantiate(newPartModel.prefab, transform.position, Quaternion.identity);

            incomingParts.Add(new Part()
            {
                thisPart = newPartObject,
                tPartModel = newPartModel
            });
        }
    }

    public void UpgradeFactory() => factoryLevel++;

    public void UpgradeFactoryProductionLevel() => factoryProductionLevel++;

    public bool IsBuilt() => currentState == FactoryState.Built;
}

public class Part
{
    public PartModel tPartModel;
    public GameObject thisPart;
}