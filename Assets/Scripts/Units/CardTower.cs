using System.Collections;
using UnityEngine;

public class CardTower : Unit
{
	[SerializeField] private Sprite[] sprites;

	private SpriteRenderer _spriteRenderer;

	private new void Start()
	{
		base.Start();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_spriteRenderer.sprite = sprites[5];
		_spriteOffset = new Vector2(0.5f, 0.5f);
		_strength = 6;
	}
	
	public bool Hurt(int dmg)
	{
		_strength = Mathf.Max(_strength - dmg, 0);
		if (_strength <= 0)
		{
			Die();
			return true;
		}
		else
		{
			Instantiate(smash, transform.position, Quaternion.identity);
			_spriteRenderer.sprite = sprites[_strength - 1];
		}

		return false;
	}
	
	protected override IEnumerator AddStrengthAnim(int strengthToAdd)
	{
		_strengthening = true;
		Instantiate(fortify, transform.position, Quaternion.identity);
        
		for (int i = 0; i < strengthToAdd; i++)
		{
			_strength++;
			yield return new WaitForSeconds(0.1f);
			_spriteRenderer.sprite = sprites[Mathf.Min(_strength - 1,  5)];
		}
        
		_upgrading = false;

		if (_strength > 6)
		{
			Instantiate(bust, transform.position, Quaternion.identity);
			StartCoroutine(DieAnim());
		}

		yield return new WaitForSeconds(0.25f);
		_strengthening = false;
	}
}