using System;
using System.Collections.Generic;
using UnityEngine;

public class FactoryVisualsController : MonoBehaviour
{
    public List<GameObject> productionLevelVisuals;
    public List<GameObject> factoryLevelVisuals;

    public void UpdateProductionLevelVisual(int productionLevel) => UpdateVisual(productionLevelVisuals, productionLevel);
    
    public void UpdateFactoryLevelVisual(int factoryLevel) => UpdateVisual(factoryLevelVisuals, factoryLevel);

    private void UpdateVisual(List<GameObject> visuals, int level)
    {
        if (level >= 0 && level < visuals.Count)
        {
            foreach (var visual in visuals) visual.SetActive(false);
            visuals[level].SetActive(true);
        }
        else throw new Exception("There is no " + level + " level in " + visuals + " visuals");
    }
}