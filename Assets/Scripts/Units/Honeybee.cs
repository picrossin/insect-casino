using System.Collections;
using UnityEngine;

public class Honeybee : Unit
{
	[SerializeField] private float produceTime = 8f;
	[SerializeField] private float addTimeBase = 1f;
	
	private new void Start()
	{
		base.Start();
		StartCoroutine(Produce());
	}

	
	private IEnumerator Produce()
	{
		yield return new WaitUntil(() => GameManager.Instance.State == GameManager.GameState.Normal);

		while (true)
		{
			yield return new WaitUntil(() => !GameManager.Instance.Choosing);
			GameManager.Instance.TimeWaited += addTimeBase * (_strength + 1);
			yield return new WaitForSeconds(produceTime);
		}
	}
}
