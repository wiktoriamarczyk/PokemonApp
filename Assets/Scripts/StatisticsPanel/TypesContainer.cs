using System.Collections.Generic;
using UnityEngine;

public class TypesContainer : MonoBehaviour
{
    [SerializeField] TypeDisplay typeDisplayPrefab;
    [SerializeField] Transform container;

    List<TypeDisplay> typeDisplays = new List<TypeDisplay>();

    public void Init(List<string> types, Color color)
    {
        Deinit();
        foreach (var type in types)
        {
            TypeDisplay typeDisplay = Instantiate(typeDisplayPrefab, container);
            typeDisplay.Init(type, color);
            typeDisplays.Add(typeDisplay);
        }
    }

    void Deinit()
    {
        foreach (var typeDisplay in typeDisplays)
        {
            Destroy(typeDisplay.gameObject);
        }
        typeDisplays.Clear();
    }
}
