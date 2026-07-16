using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public TextMeshProUGUI buttonText;

    public float pressYOffset = -5f;

    private Vector3 originPosition;
    private RectTransform textRectTransform;
    private bool isHovering = false;

    [Header("»ö»ó »óÅÂ")]
    public Color normalColor = new Color(0.8f, 0.8f, 0.8f, 1f); // ±âº» »ö»ó (»ìÂ¦ È¸»öºû)
    public Color hoverColor = Color.white;                      // ¿Ã·ÈÀ» ¶§ (°¡Àå ¹à°Ô)
    public Color pressedColor = new Color(0.5f, 0.5f, 0.5f, 1f);// ´­·¶À» ¶§ (¾îµÓ°Ô)



    private void Start()
    {
        if(buttonText != null)
        {
            textRectTransform = buttonText.rectTransform;
            originPosition = textRectTransform.anchoredPosition;
            buttonText.color = normalColor;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        if(buttonText!=null) buttonText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        if (buttonText != null) buttonText.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (buttonText != null) buttonText.color = pressedColor;
        if (textRectTransform != null)
            textRectTransform.anchoredPosition = new Vector3(originPosition.x, originPosition.y + pressYOffset, originPosition.z);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(buttonText!=null) buttonText.color=isHovering ? hoverColor : normalColor;
        if (textRectTransform != null)
            textRectTransform.anchoredPosition = originPosition;
    }
}
