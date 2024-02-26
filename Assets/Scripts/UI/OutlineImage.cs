using UnityEngine;
using UnityEngine.UI;

public class OutlineImage : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] Image outline;

    public void SetImage(Sprite sprite, bool showOutline)
    {
        image.sprite = sprite;
        outline.enabled = showOutline;
    }
}
