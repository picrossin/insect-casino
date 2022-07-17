using UnityEngine;

public class Chips : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    
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
        }
    }

    public bool TakeChip()
    {
        _chips--;

        if (_chips <= 0)
        {
            GameManager.Instance.RemoveChipPile(this);
            Destroy(gameObject);
            return false;
        }
        
        _sprite.sprite = frames[_chips - 1];

        return true;
    }
}
