using UnityEngine;

public class Cursor : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.Instance.State == GameManager.GameState.Normal)
        {
            Vector3Int gridPosition = GameManager.Instance.TileGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            Unit unit = GameManager.Instance.GameGrid.GetUnit(gridPosition);
            
            if (unit != null)
            {
                unit.ShowHideStrength();
            }
        }
    }
}
