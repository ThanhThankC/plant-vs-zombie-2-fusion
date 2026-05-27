using UnityEngine;
using UnityEngine.UI;

public class CircleButton : MonoBehaviour
{
    void Start()
    {
        Image img = GetComponent<Image>();
        img.alphaHitTestMinimumThreshold = 0.1f;
    }
}