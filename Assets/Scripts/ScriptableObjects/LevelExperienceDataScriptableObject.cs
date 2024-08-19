using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelExperienceData", menuName = "ScriptableObjects/LevelExperienceData", order = 1)]
    public class LevelExperienceDataScriptableObject : ScriptableObject
    {
        [SerializeField] private List<int> _experienceRequiredByLevels;

        public int GetExperienceRequiredByLevel(int level)
        {
            if (level >= 0 && level < _experienceRequiredByLevels.Count) return _experienceRequiredByLevels[level];
            else throw new Exception($"There is no {level} level in experience database!");
        }

        public int GetMaxPlayerLevel() => _experienceRequiredByLevels.Count;
    }
}

