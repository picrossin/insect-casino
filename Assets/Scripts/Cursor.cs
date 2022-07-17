using UnityEngine;

public class Cursor : MonoBehaviour
{
    public static Cursor Instance;
    
    [SerializeField] private Texture2D cursor;
    [SerializeField] private Texture2D clickCursor;
    [SerializeField] private Vector2 cursorHotSpot = Vector2.zero;
    [SerializeField] private Vector2 clickCursorHotSpot = Vector2.zero;

    private bool _jiggled;
    
    private void Start()
    {
        Instance = this;
        UnityEngine.Cursor.SetCursor(cursor, cursorHotSpot, CursorMode.Auto);
    }

    private void Update()
    {
        Vector3Int gridPosition = GameManager.Instance.TileGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        Unit unit = GameManager.Instance.GameGrid.GetUnit(gridPosition);

        if (unit != null && !_jiggled)
        {
            unit.Jiggle();
            _jiggled = true;
        }
        else if (unit == null)
        {
            _jiggled = false;
        }
        
        switch (GameManager.Instance.State)
        {
            case GameManager.GameState.Placing:
            {
                SetCursor(true);
                break;
            }
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
                    SetCursor(true);
                    if (Input.GetMouseButtonDown(0))
                    {
                        GameManager.Instance.UnitToUpgrade = unit;
                    }
                }
                else
                {
                    SetCursor(false);
                }
                break;
        }
    }

    public void SetCursor(bool click)
    {
        UnityEngine.Cursor.SetCursor(click ? clickCursor : cursor, click ? clickCursorHotSpot : cursorHotSpot, CursorMode.Auto);
    }
}
