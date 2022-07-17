using UnityEngine;
using UnityEngine.UI;

public class HelpBox : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text text;
    private Animation _anim;

    private void Start()
    {
        _anim = GetComponent<Animation>();
    }

    public void SetHelp(Sprite sprite, string flavorText)
    {
        _anim.Play("HelpIn");
        image.sprite = sprite;
        text.text = flavorText;
    }

    public void Dismiss()
    {
        _anim.Play("HelpOut");
    }
}
