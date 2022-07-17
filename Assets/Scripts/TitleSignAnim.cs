using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleSignAnim : MonoBehaviour
{
    [SerializeField] private Sprite frame1;
    [SerializeField] private Sprite frame2;
    [SerializeField] private float waitTime = 0.5f;

    private Image _image;
    private bool _frame1 = true;
    
    private void Start()
    {
        _image = GetComponent<Image>();
        StartCoroutine(Anim());
    }

    private IEnumerator Anim()
    {
        while (true)
        {
            _image.sprite = _frame1 ? frame1 : frame2;
            _frame1 = !_frame1;
            yield return new WaitForSeconds(waitTime);
        }
    }
}
