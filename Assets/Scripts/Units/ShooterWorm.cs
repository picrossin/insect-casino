using System.Collections;
using UnityEngine;

public class ShooterWorm : Unit
{
	[SerializeField] private float baseShootTime = 7;
	[SerializeField] private GameObject projectile;

	private new void Start()
	{
		base.Start();
		StartCoroutine(Shoot());
	}

	private IEnumerator Shoot()
	{
		yield return new WaitUntil(() => GameManager.Instance.State == GameManager.GameState.Normal);
		
		while (true)
		{
			Instantiate(projectile, transform.position, Quaternion.identity);
			yield return new WaitForSeconds(baseShootTime);
		}
	}
}
