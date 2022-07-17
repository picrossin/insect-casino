using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject click;
    [SerializeField] private GameObject hover;

    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => PlayClickSound());
    }

    private void PlayClickSound()
    {
        if (click != null)
            Instantiate(click, transform.position, Quaternion.identity);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hover != null)
            Instantiate(hover, transform.position, Quaternion.identity);
        Cursor.Instance.SetCursor(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.Instance.SetCursor(false);
    }

    private void OnDisable()
    {
        Cursor.Instance.SetCursor(false);
    }
    
    private void OnDestroy()
    {
        Cursor.Instance.SetCursor(false);
    }
}
