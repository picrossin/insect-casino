using System.Collections;
using DG.Tweening;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance;

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    public void Shake(float intensity, float duration)
    {
        StartCoroutine(ShakeAnim(intensity, duration));
    }

    private IEnumerator ShakeAnim(float intensity, float duration)
    {
        transform.DOShakePosition(duration, intensity);
        yield return new WaitForSeconds(duration);
        transform.position = new Vector3(0f, 0f, -10f);
        
    }
}
