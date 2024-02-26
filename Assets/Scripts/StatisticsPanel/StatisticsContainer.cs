using System.Collections.Generic;
using UnityEngine;

public class StatisticsContainer : MonoBehaviour
{
    [SerializeField] List<StatisticDisplay> statistics;

    public void Init(List<StatisticsCompactData> stats, Color color)
    {
        foreach (var stat in stats)
        {
            StatisticDisplay statisticDisplay = statistics.Find(x => x.statName == stat.name);
            if (statisticDisplay != null)
            {
                statisticDisplay.Init(stat.value, color);
            }
        }
    }
}
