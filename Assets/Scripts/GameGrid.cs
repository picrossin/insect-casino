using UnityEngine;

public class GameGrid : MonoBehaviour
{
	[SerializeField] private Grid _tileGrid;

	private Unit[,] _grid = new Unit[12,12];

	public Vector3Int GetSafeBoardPosition(Vector3Int tilegridPosition, bool isCards=false)
	{
		int xPos = tilegridPosition.x + 6;
		int yPos = tilegridPosition.y + 6;
		
		return new Vector3Int(Mathf.Clamp(xPos, 0,  isCards ? 10 : 11) - 6, Mathf.Clamp(yPos, 0, isCards ? 10 : 11) - 6);
	}
	
	public bool CanPlaceUnit(Vector3Int tilegridPosition, bool isCards=false)
	{
		int xPos = tilegridPosition.x + 6;
		int yPos = tilegridPosition.y + 6;
		
		if (isCards && (xPos > 10 || yPos > 10)) return false;

		bool cardsExtraQuery = isCards &&
		                       _grid[xPos + 1, yPos] == null &&
		                       _grid[xPos, yPos + 1] == null &&
		                       _grid[xPos + 1, yPos + 1] == null;
		if (!isCards)
		{
			cardsExtraQuery = true;
		}
		
		return xPos >= 0 && xPos <= 11 && yPos >= 0 && yPos <= 11 && _grid[xPos, yPos] == null && cardsExtraQuery;
	}
	
	public void InsertUnit(Vector3Int tilegridPosition, Unit unitToPlace)
	{
		if (!CanPlaceUnit(tilegridPosition)) return;

		int xPos = tilegridPosition.x + 6;
		int yPos = tilegridPosition.y + 6;

		_grid[xPos, yPos] = unitToPlace;

		if (unitToPlace.BugType == GameManager.UnitType.Cards)
		{
			_grid[xPos + 1, yPos] = unitToPlace;
			_grid[xPos, yPos + 1] = unitToPlace;
			_grid[xPos + 1, yPos + 1] = unitToPlace;
		}
	}

	public Unit GetUnit(Vector3Int tilegridPosition)
	{
		int xPos = tilegridPosition.x + 6;
		int yPos = tilegridPosition.y + 6;
		
		if (xPos >= 0 && xPos <= 11 && yPos >= 0 && yPos <= 11)
		{
			return _grid[xPos, yPos];
		}
		
		return null;
	}
}
