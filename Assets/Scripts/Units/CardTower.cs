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
	
	public void Hurt(int dmg)
	{
		_strength = Mathf.Max(_strength - dmg, 0);
		if (_strength <= 0)
		{
			Die();
		}
		else
		{
			_spriteRenderer.sprite = sprites[_strength - 1];
		}
	}
	
	protected override IEnumerator AddStrengthAnim(int strengthToAdd)
	{
		_strength += strengthToAdd;

		if (_strength > 6)
		{
			// BUST!
			StartCoroutine(DieAnim());
		}
        
		_spriteRenderer.sprite = sprites[_strength - 1];
		yield break; // TODO: Add anim
	}
}