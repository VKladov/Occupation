using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PersonGenerator : MonoBehaviour
{
    [Serializable]
    public class TeamPreset
    {
        public int TeamId;
        public List<GameObject> Parts;
    }

    [SerializeField] private List<TeamPreset> Presets;

    public void GenerateForTeam(int teamID)
    {
        foreach (var teamPreset in Presets)
            foreach (var part in teamPreset.Parts)
                part.gameObject.SetActive(false);
        
        
        foreach (var preset in Presets.Where(item => item.TeamId == teamID))
        {
            ActivateRandom(preset.Parts);
        }
    }

    private static void ActivateRandom(IReadOnlyList<GameObject> gameObjects)
    {
        var active = gameObjects[Random.Range(0, gameObjects.Count)];
        foreach (var o in gameObjects)
        {
            o.SetActive(active == o);
        }
    }
}
