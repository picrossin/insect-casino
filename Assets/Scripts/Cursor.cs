using UnityEngine;

public class Cursor : MonoBehaviour
{
    private void Update()
    {
        Vector3Int gridPosition = GameManager.Instance.TileGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        Unit unit = GameManager.Instance.GameGrid.GetUnit(gridPosition);
        
        switch (GameManager.Instance.State)
        {
            case GameManager.GameState.Normal:
            {
                if (unit != null)
                {
                    unit.ShowStrength();
                }

                break;
            }
            case GameManager.GameState.Upgrading:
                if (unit != null)
                {
                    // TODO: Highlight unit
                    if (Input.GetMouseButtonDown(0))
                    {
                        GameManager.Instance.UnitToUpgrade = unit;
                    }
                }
                
                break;
        }
    }
}
