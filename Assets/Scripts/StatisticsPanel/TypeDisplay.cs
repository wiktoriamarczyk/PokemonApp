using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TypeDisplay : MonoBehaviour
{
    [SerializeField] Image background;
    [SerializeField] TextMeshProUGUI nameDisplay;

    public void Init(string typeName, Color color)
    {
        nameDisplay.text = typeName.FirstCharacterToUpper();
        background.color = color;
    }
}
