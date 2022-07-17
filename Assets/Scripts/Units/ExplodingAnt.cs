using UnityEngine;

public class ExplodingAnt : Unit
{
	[SerializeField] private GameObject explosion;
	[SerializeField] private LayerMask projDestroyers;
	[SerializeField] private GameObject explodeSFX;

	private Vector2Int[] _explodePattern1 =
	{
		new Vector2Int(-1, 0),
		new Vector2Int(1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(0, -1)
	};
	
	private Vector2Int[] _explodePattern2 =
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
	
	private Vector2Int[] _explodePattern3 =
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
	
	private Vector2Int[] _explodePattern4 =
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
	}

	private void FixedUpdate()
	{
		if (_placing || GameManager.Instance.Choosing) return;
		
		float range = 1.5f;

		if (_strength >= 2 && _strength < 4)
		{
			range = 1.75f;
		}
		else if (_strength >= 4 && _strength < 6)
		{
			range = 2f;
		}
		else if (_strength == 6)
		{
			range = 2.5f;
		}
		
		Collider2D coll = Physics2D.OverlapCircle(transform.position, range, projDestroyers);

		if (coll != null && coll.CompareTag("Hand"))
		{
			Vector3Int tilePos = GameManager.Instance.TileGrid.WorldToCell(transform.position);
                    
			Instantiate(explosion, 
				tilePos + new Vector3(0.5f, 0.813f, 0.0f), 
				Quaternion.identity);
			Instantiate(explodeSFX, transform.position, Quaternion.identity);
			
            if (_strength < 2)
            {
                foreach (Vector2Int pos in _explodePattern1)
                {
                    if (GameManager.Instance.GameGrid.CanPlaceUnit(tilePos + new Vector3Int(pos.x, pos.y, 0)))
                    {
                    	Instantiate(explosion, 
                    		tilePos + new Vector3Int(pos.x, pos.y, 0) + new Vector3(0.5f, 0.813f, 0.0f), 
                    		Quaternion.identity);
                    }
                }
            }
            else if (_strength < 4)
            {
                foreach (Vector2Int pos in _explodePattern2)
                {
                    if (GameManager.Instance.GameGrid.CanPlaceUnit(tilePos + new Vector3Int(pos.x, pos.y, 0)))
                    {
                    	Instantiate(explosion, 
                    		tilePos + new Vector3Int(pos.x, pos.y, 0) + new Vector3(0.5f, 0.813f, 0.0f), 
                    		Quaternion.identity);
                    }
                }	
            }
            else if (_strength < 6)
            {
                foreach (Vector2Int pos in _explodePattern3)
                {
                    if (GameManager.Instance.GameGrid.CanPlaceUnit(tilePos + new Vector3Int(pos.x, pos.y, 0)))
                    {
                    	Instantiate(explosion, 
                    		tilePos + new Vector3Int(pos.x, pos.y, 0) + new Vector3(0.5f, 0.813f, 0.0f), 
                    		Quaternion.identity);
                    }
                }
            }
            else
            {
                foreach (Vector2Int pos in _explodePattern4)
                {
                    if (GameManager.Instance.GameGrid.CanPlaceUnit(tilePos + new Vector3Int(pos.x, pos.y, 0)))
                    {
                    	Instantiate(explosion, 
                    		tilePos + new Vector3Int(pos.x, pos.y, 0) + new Vector3(0.5f, 0.813f, 0.0f), 
                    		Quaternion.identity);
                    }
                }
            }
         
            GameManager.Instance.GameGrid.RemoveUnit(this);
			Destroy(gameObject);
		}
	}
}
