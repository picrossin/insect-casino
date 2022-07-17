using UnityEngine;

public class Chips : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private GameObject pinchSound;

    private int _chips = 6;
    private bool _initialized;
    private SpriteRenderer _sprite;

    private void Start()
    {
        _sprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        if (!_initialized)
        {
            _initialized = true;
            GameManager.Instance.AddChipPile(this);
            
            Vector3Int tilePos = GameManager.Instance.TileGrid.WorldToCell(transform.position);
            GameManager.Instance.GameGrid.InsertUnit(tilePos, GetComponent<ChipUnit>());
        }
    }

    public bool TakeChip()
    {
        _chips--;
        Instantiate(pinchSound, transform.position, Quaternion.identity);

        if (_chips <= 0)
        {
            GameManager.Instance.RemoveChipPile(this);
            GameManager.Instance.GameGrid.RemoveUnit(GetComponent<ChipUnit>());
            Destroy(gameObject);
            return false;
        }
        
        _sprite.sprite = frames[_chips - 1];

        return true;
    }
}
