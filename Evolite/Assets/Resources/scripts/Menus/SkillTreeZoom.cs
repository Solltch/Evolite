using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeZoom : MonoBehaviour, IScrollHandler
{
    public ScrollRect scrollRect;
    public RectTransform content;

    public float zoomMin = 0.5f;
    public float zoomMax = 2f;
    public float zoomSpeed = 0.1f;

    private float currentZoom = 1f;

    public void OnScroll(PointerEventData eventData)
    {
        float scroll = eventData.scrollDelta.y;

        float oldZoom = currentZoom;
        currentZoom += scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, zoomMin, zoomMax);

        // Viewport (janela visível)
        RectTransform viewport = scrollRect.viewport != null
            ? scrollRect.viewport
            : scrollRect.GetComponent<RectTransform>();

        // 1) Pega posição do mouse em espaço local do CONTENT
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            content,
            Input.mousePosition,
            eventData.pressEventCamera,
            out Vector2 localMousePosContent
        );

        // 2) Aplica novo zoom ao scale
        content.localScale = Vector3.one * currentZoom;

        // 3) Recalcula posição para manter o ponto do mouse fixo
        float zoomFactor = currentZoom / oldZoom;

        Vector2 newPos = (Vector2)content.localPosition - localMousePosContent * (zoomFactor - 1f);

        content.localPosition = newPos;
    }
}