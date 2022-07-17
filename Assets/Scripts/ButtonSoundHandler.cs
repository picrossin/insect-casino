using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundHandler : MonoBehaviour, IPointerEnterHandler
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
        Instantiate(click, transform.position, Quaternion.identity);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Instantiate(hover, transform.position, Quaternion.identity);
    }
}
