using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EvolutionElementDisplay : MonoBehaviour
{
    [field: SerializeField] public OutlineImage outlineImage { get; private set; }
    [SerializeField] TextMeshProUGUI elementNameDisplay;

    public string elementName
    {
        get => _name;
        set
        {
            _name = value;
            elementNameDisplay.text = value.FirstCharacterToUpper();
        }
    }
    string _name;


}
