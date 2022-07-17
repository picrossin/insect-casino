using System.Collections;
using UnityEngine;

public class GoopSnail : Unit
{
	[SerializeField] private float baseShootTime = 12;
	[SerializeField] private GameObject projectile;
	[SerializeField] private GameObject shootSFX;

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
			yield return new WaitUntil(() => !GameManager.Instance.Choosing);
			Instantiate(projectile, transform.position, Quaternion.identity);
			Instantiate(shootSFX, transform.position, Quaternion.identity);
			yield return new WaitForSeconds(baseShootTime - (_strength * 1.5f));
		}
	}
}
