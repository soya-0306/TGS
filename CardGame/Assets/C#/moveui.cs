using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DetectImageUnderCursor : MonoBehaviour
{
    public Camera uiCamera; // UI用カメラ（Screen Space - Camera のとき必要）
    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;

    public Image hoveredImage; // マウスが乗っているImageが格納される変数

    void Update()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        hoveredImage = null;

        foreach (RaycastResult result in results)
        {
            Image img = result.gameObject.GetComponent<Image>();
            if (img != null)
            {
                hoveredImage = img;
                break;
            }
        }

        if (hoveredImage != null)
        {
            Debug.Log($"Hovering over: {hoveredImage.name}");
        }
    }
}
