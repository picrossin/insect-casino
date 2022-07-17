using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    protected bool _placing;
    public bool Placing
    {
        get => _placing;
        set => _placing = value;
    }

    protected Vector2 _spriteOffset = Vector2.zero;
    protected int _strength = 0;

    [SerializeField] protected GameManager.UnitType type;
    public GameManager.UnitType BugType => type;

    [SerializeField] private bool showStrength = true;
    [SerializeField] private GameObject splat;
    [SerializeField] private GameObject place;
    [SerializeField] private GameObject fortify;
    [SerializeField] private GameObject bust;
    [SerializeField] private GameObject smash;

    private SpriteRenderer _sprite;
    private GameObject _strengthCanvas;
    private Text _strengthText;
    private StrengthBar _strengthBar;
    private bool _strengthening;
    private Tween _jiggle;

    protected void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _strengthCanvas = transform.Find("HealthCanvas").gameObject;
        _strengthText = _strengthCanvas.GetComponentInChildren<Text>();
        _strengthBar = _strengthCanvas.GetComponentInChildren<StrengthBar>();
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
                _sprite.color = Color.gray;
            }
            else
            {
                _sprite.color = Color.white;
            }
            
            transform.position = gridPosition + new Vector3(0.5f + _spriteOffset.x, 0.813f + _spriteOffset.y, 0.0f);
            
            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                GameManager.Instance.GameGrid.InsertUnit(gridPosition, this);
                Cursor.Instance.SetCursor(false);
                Instantiate(place, transform.position, Quaternion.identity);
                _placing = false;
            }
        }

        if (_strengthening)
        {
            ShowStrength();
        }
    }

    public void ShowStrength()
    {
        if (showStrength)
        {
            StartCoroutine(ShowStrengthAnim());
        }
    }

    private IEnumerator ShowStrengthAnim()
    {
        _strengthCanvas.SetActive(true);
        _strengthText.text = $"{_strength}/6";
        _strengthBar.SetStrength(_strength);
        yield return new WaitUntil(() => GameManager.Instance.State != GameManager.GameState.Upgrading);
        yield return new WaitForEndOfFrame();
        _strengthCanvas.SetActive(false);
    }

    public void AddStrength(int strengthToAdd)
    {
        StartCoroutine(AddStrengthAnim(strengthToAdd));
    }

    protected virtual IEnumerator AddStrengthAnim(int strengthToAdd)
    {
        _strengthening = true;
        Instantiate(fortify, transform.position, Quaternion.identity);
        
        for (int i = 0; i < strengthToAdd; i++)
        {
            _strength++;
            yield return new WaitForSeconds(0.1f);
        }

        if (_strength > 6)
        {
            Instantiate(bust, transform.position, Quaternion.identity);
            StartCoroutine(DieAnim());
        }

        yield return new WaitForSeconds(0.25f);
        _strengthening = false;
    }

    public void Die()
    {
        StartCoroutine(DieAnim());
    }
    
    protected IEnumerator DieAnim()
    {
        Instantiate(smash, transform.position, Quaternion.identity);
        GameManager.Instance.GameGrid.RemoveUnit(this);
        Instantiate(splat, transform.position, Quaternion.identity);
        Destroy(gameObject);
        yield break;
    }

    public void Jiggle()
    {
        if (!showStrength)
        {
            return;
        }
        
        transform.localScale = Vector3.one;
        _jiggle.Kill();
        _jiggle = transform.DOPunchScale(transform.localScale / 4, 0.25f, 10, 0.1f);
    }
}
