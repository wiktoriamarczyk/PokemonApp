using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class StatisticDisplay : MonoBehaviour
{
    [field: SerializeField] public string statName { get; private set; }
    [SerializeField] TextMeshProUGUI statNameDisplay;
    [SerializeField] TextMeshProUGUI statValue;
    [SerializeField] Image statValueDisplayContainer;
    [SerializeField] Image statValueDisplay;

    const float maxValue = 255;

    public void Init(int value, Color color)
    {
        InitValue(value);
        float normalizedValue = (float)value / maxValue;
        statValueDisplay.fillAmount = normalizedValue;
        statValueDisplay.color = color;
    }

    private void InitValue(int value)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(value);
        sb.Append(" / ");
        sb.Append(maxValue);
        statValue.text = sb.ToString();
    }
}
