using UnityEngine;

public class CardTower : Unit
{
	private new void Start()
	{
		base.Start();
		_spriteOffset = new Vector2(0.5f, 0.5f);
	}
}