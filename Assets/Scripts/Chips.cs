using UnityEngine;

public class Chips : MonoBehaviour
{
    private int _chips = 6;
    private bool _initialized;

    private void Update()
    {
        if (!_initialized)
        {
            _initialized = true;
            GameManager.Instance.AddChipPile(this);
        }
    }

    public void TakeChip()
    {
        // TODO: Add animation
        
        _chips--;

        if (_chips <= 0)
        {
            GameManager.Instance.RemoveChipPile(this);
            Destroy(gameObject);
        }
    }
}
