using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    private bool _placing;
    public bool Placing
    {
        get => _placing;
        set => _placing = value;
    }

    protected Vector2 _spriteOffset = Vector2.zero;

    [SerializeField] protected GameManager.UnitType type;
    public GameManager.UnitType BugType => type;

    private int _strength = 1;
    private GameObject _strengthCanvas;
    private Text _strengthText;

    protected void Start()
    {
        _strengthCanvas = transform.Find("HealthCanvas").gameObject;
        _strengthText = _strengthCanvas.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        if (_placing && GameManager.Instance.State == GameManager.GameState.Placing)
        {
            Vector3Int gridPosition = GameManager.Instance.TileGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            gridPosition = GameManager.Instance.GameGrid.GetSafeBoardPosition(gridPosition, type == GameManager.UnitType.Cards);
            
            bool canPlace = true;
            if (!GameManager.Instance.GameGrid.CanPlaceUnit(gridPosition, type == GameManager.UnitType.Cards))
            {
                canPlace = false;
                print("Invalid spot");
                // TODO: color red or something
            }
            
            transform.position = gridPosition + new Vector3(0.5f + _spriteOffset.x, 1.25f + _spriteOffset.y, 0.0f);
            
            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                GameManager.Instance.GameGrid.InsertUnit(gridPosition, this);
                _placing = false;
            }
        }
    }

    public void ShowHideStrength()
    {
        StartCoroutine(ShowStrength());
    }

    private IEnumerator ShowStrength()
    {
        _strengthText.text = $"{_strength}/7";
        _strengthCanvas.SetActive(true);
        yield return new WaitForEndOfFrame();
        _strengthCanvas.SetActive(false);
    }
}
