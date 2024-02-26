using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI abilityName;
    [SerializeField] TextMeshProUGUI abilityDescription;

    public void Init(string abilityName, string abilityDescription, Color color)
    {
        this.abilityName.text = abilityName;
        this.abilityName.color = color;
        abilityDescription = abilityDescription.Replace("\n", " ");
        this.abilityDescription.text = abilityDescription;
    }
}
