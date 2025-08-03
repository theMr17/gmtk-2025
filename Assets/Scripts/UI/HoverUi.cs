using UnityEngine;
using UnityEngine.EventSystems;

public class HoverUi : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
  [SerializeField] private Texture2D cursorTexture;

  private void Reset()
  {
    // Automatically disable on parent objects
    if (transform.parent == null || transform.GetComponentInParent<HoverUi>() == null) return;

    // Disable this script if attached to a parent without visible UI interaction
    if (transform == transform.root)
      enabled = false;
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    // Apply only if this is not the root GameObject
    if (transform != transform.root)
    {
      Vector2 hotspot = new Vector2(cursorTexture.width / 2f, cursorTexture.height / 2f);
      Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    if (transform != transform.root)
    {
      Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
  }

  void OnDestroy()
  {
    if (transform != transform.root)
    {
      Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
  }
}
