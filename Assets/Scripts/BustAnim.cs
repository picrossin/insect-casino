using DG.Tweening;
using UnityEngine;

public class BustAnim : MonoBehaviour
{
	private void Start()
	{
		transform.DOPunchScale(transform.localScale * 1.25f, 2f);
		Destroy(gameObject, 1.5f);
	}
}
