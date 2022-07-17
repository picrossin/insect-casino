using UnityEngine;

public class DestroyAfterSecs : MonoBehaviour
{
    [SerializeField] private float secs = 1f;

    private void Start()
    {
        Destroy(gameObject, secs);
    }
}
