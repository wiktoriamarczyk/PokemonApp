using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesContainer : MonoBehaviour
{
    [SerializeField] AbilityDisplay abilityDisplayPrefab;
    [SerializeField] Transform container;

    List<AbilityDisplay> abilities = new List<AbilityDisplay>();

    public void Init(List<AbilityCompactData> abilitiesData, Color color)
    {
        Deinit();
        foreach (var ability in abilitiesData)
        {
            var abilityDisplay = Instantiate(abilityDisplayPrefab, container);
            abilityDisplay.Init(ability.name, ability.description, color);
            abilities.Add(abilityDisplay);
        }
    }

    public void Deinit()
    {
        foreach (var ability in abilities)
        {
            Destroy(ability.gameObject);
        }
        abilities.Clear();
    }
}
