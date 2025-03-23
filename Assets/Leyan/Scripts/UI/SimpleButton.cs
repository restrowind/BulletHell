using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class SimpleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("ÐüÍ£ÑÕÉ«¿ØÖÆ")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        text.color = normalColor;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = normalColor;
    }
}
