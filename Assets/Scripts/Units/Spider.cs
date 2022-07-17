using System.Collections;
using UnityEngine;

public class Spider : Unit
{
	[SerializeField] private float baseShootTime = 12;
	[SerializeField] private GameObject web;
	[SerializeField] private GameObject webSFX;

	private Vector2Int[] _webPattern1 =
	{
		new Vector2Int(-1, 0),
		new Vector2Int(1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(0, -1)
	};
	
	private Vector2Int[] _webPattern2 =
	{
		new Vector2Int(-1, 0),
		new Vector2Int(1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(0, -1),
		new Vector2Int(1, 1), // NEW
		new Vector2Int(1, -1),
		new Vector2Int(-1, 1),
		new Vector2Int(-1, -1)
	};
	
	private Vector2Int[] _webPattern3 =
	{
		new Vector2Int(-1, 0),
		new Vector2Int(1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(0, -1),
		new Vector2Int(1, 1),
		new Vector2Int(1, -1),
		new Vector2Int(-1, 1),
		new Vector2Int(-1, -1),
		new Vector2Int(-2, 0), // NEW
		new Vector2Int(2, 0),
		new Vector2Int(0, 2),
		new Vector2Int(0, -2)
	};
	
	private Vector2Int[] _webPattern4 =
	{
		new Vector2Int(-1, 0),
		new Vector2Int(1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(0, -1),
		new Vector2Int(1, 1),
		new Vector2Int(1, -1),
		new Vector2Int(-1, 1),
		new Vector2Int(-1, -1),
		new Vector2Int(-2, 0),
		new Vector2Int(2, 0),
		new Vector2Int(0, 2),
		new Vector2Int(0, -2),
		new Vector2Int(-2, 1), // NEW
		new Vector2Int(-2, 2),
		new Vector2Int(-1, 2),
		new Vector2Int(1, 2),
		new Vector2Int(2, 2),
		new Vector2Int(2, 1),
		new Vector2Int(2, -1),
		new Vector2Int(2, -2),
		new Vector2Int(1, -2),
		new Vector2Int(-1, -2),
		new Vector2Int(-2, -2),
		new Vector2Int(-2, -1)
	};
	
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
			Instantiate(webSFX, transform.position, Quaternion.identity);
			
			Vector3Int tilePos = GameManager.Instance.TileGrid.WorldToCell(transform.position);
			
			if (_strength < 2)
			{
				foreach (Vector2Int pos in _webPattern1)
				{
					if (GameManager.Instance.GameGrid.CanPlaceUnit(tilePos + new Vector3Int(pos.x, pos.y, 0)))
					{
						Instantiate(web, 
							tilePos + new Vector3Int(pos.x, pos.y, 0) + new Vector3(0.5f, 0.813f, 0.0f), 
							Quaternion.identity);
					}
				}
			}
			else if (_strength < 4)
			{
				foreach (Vector2Int pos in _webPattern2)
				{
					if (GameManager.Instance.GameGrid.CanPlaceUnit(tilePos + new Vector3Int(pos.x, pos.y, 0)))
					{
						Instantiate(web, 
							tilePos + new Vector3Int(pos.x, pos.y, 0) + new Vector3(0.5f, 0.813f, 0.0f), 
							Quaternion.identity);
					}
				}	
			}
			else if (_strength < 6)
			{
				foreach (Vector2Int pos in _webPattern3)
				{
					if (GameManager.Instance.GameGrid.CanPlaceUnit(tilePos + new Vector3Int(pos.x, pos.y, 0)))
					{
						Instantiate(web, 
							tilePos + new Vector3Int(pos.x, pos.y, 0) + new Vector3(0.5f, 0.813f, 0.0f), 
							Quaternion.identity);
					}
				}
			}
			else
			{
				foreach (Vector2Int pos in _webPattern4)
				{
					if (GameManager.Instance.GameGrid.CanPlaceUnit(tilePos + new Vector3Int(pos.x, pos.y, 0)))
					{
						Instantiate(web, 
							tilePos + new Vector3Int(pos.x, pos.y, 0) + new Vector3(0.5f, 0.813f, 0.0f), 
							Quaternion.identity);
					}
				}
			}
			
			yield return new WaitForSeconds(baseShootTime);
		}
	}
}
