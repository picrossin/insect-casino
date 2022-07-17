using UnityEngine;

public class ApplyShake : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float intensity = 1f;
    [SerializeField] private float timeUntilDestroy = 3f;

    private void Start()
    {
        ScreenShake.Instance.Shake(intensity, duration);
        Destroy(gameObject, timeUntilDestroy);
    }
}
