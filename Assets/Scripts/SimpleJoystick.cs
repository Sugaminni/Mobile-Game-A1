using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform handle;
    public float moveRange = 100f;
    public PlayerController player;

    private Vector2 inputVector = Vector2.zero;

    // Called when the joystick is pressed down
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    // Called when the joystick is dragged
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pos
        );

        pos = Vector2.ClampMagnitude(pos, moveRange);
        handle.anchoredPosition = pos;

        inputVector = pos / moveRange;

        player.SetHorizontal(inputVector.x);
        player.SetVertical(inputVector.y);
    }

    // Called when the joystick is released
    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        player.SetHorizontal(0);
        player.SetVertical(0);
    }
}
