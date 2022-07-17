using System.Collections;
using UnityEngine;

public class Web : MonoBehaviour
{
    [SerializeField] private float lifetime;
    
    private void Start()
    {
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
