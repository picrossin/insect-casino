using UnityEngine;

public class Unit : MonoBehaviour
{
    private bool _placing;
    public bool Placing
    {
        get => _placing;
        set => _placing = value;
    }

    protected Vector2 _spriteOffset = Vector2.zero;

    private void Update()
    {
        if (_placing && GameManager.Instance.State == GameManager.GameState.Placing)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition = GameManager.Instance.GameGrid.WorldToCell(worldPosition) + new Vector3(0.5f + _spriteOffset.x, 1.25f + _spriteOffset.y, 0.0f);
            transform.position = worldPosition;

            if (Input.GetMouseButtonDown(0))
            {
                _placing = false;
            }
        }
    }
}
