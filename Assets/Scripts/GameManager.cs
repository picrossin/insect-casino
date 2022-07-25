using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class Helper
{
    public Sprite sprite;
    public string text;
}

public class GameManager : MonoBehaviour
{
    public enum ProgramState
    {
        Title,
        Game,
        Leaderboard
    }
    
    public enum GameState
    {
        Normal,
        Placing,
        Upgrading
    }

    public enum UnitType
    {
        Cards = 1,
        ExplodingAnt = 2,
        ShooterWorm = 3,
        GoopSnail = 4,
        Honeybee = 5,
        Spider = 6
    }

    public static GameManager Instance;

    [SerializeField] private bool debugForceUnitType;
    [SerializeField] private UnitType debugUnitType;
    
    [SerializeField] private GameObject[] units; 
    [SerializeField] private float produceTime = 10f;
    [SerializeField] private DieUIManager dieUIManager;
    [SerializeField] private GameObject[] hands;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject titleUI;
    [SerializeField] private GameObject leaderboardUI;
    [SerializeField] private HelpBox helpBox;
    [SerializeField] private Helper[] helpers;
    [SerializeField] private SpriteRenderer bg;

    [SerializeField] private AudioClip hiHat;
    [SerializeField] private AudioClip mainTrack;
    
    [SerializeField] private Grid grid;
    public Grid TileGrid => grid;

    [SerializeField] private GameGrid gameGrid;
    public GameGrid GameGrid => gameGrid;

    private GameState _state;
    public GameState State
    {
        get => _state;
        set => _state = value;
    }
    
    private ProgramState _programState;
    public ProgramState CurrentProgramState
    {
        get => _programState;
        set => _programState = value;
    }

    private bool _dieReady;
    public bool DieReady
    {
        get => _dieReady;
        set => _dieReady = value;
    }

    private Unit _unitToUpgrade;
    public Unit UnitToUpgrade
    {
        get => _unitToUpgrade;
        set => _unitToUpgrade = value;
    }
    
    private float _productionPercentage;
    public float ProductionPercentage => _productionPercentage;

    private bool _busy;
    public bool Busy => _busy;
    
    private HashSet<Hand> _handsInPlay = new HashSet<Hand>();
    public HashSet<Hand> HandsInPlay => _handsInPlay;
    
    private HashSet<float> _availableHandAngles = new HashSet<float> {0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f};
    public HashSet<float> AvailableHandAngles => _availableHandAngles;
    
    private float _timeWaited = 0f;
    public float TimeWaited
    {
        get => _timeWaited;
        set => _timeWaited = value;
    }

    private bool _choosing;
    public bool Choosing => _choosing;

    
    private float _handWaitTime = 10f;
    private List<Chips> _chipTargets = new List<Chips>();
    private HashSet<Chips> _chipPiles = new HashSet<Chips>();
    private TextMeshProUGUI _scoreText;
    private AudioSource _music;
    private float _score;
    private bool _gameInitialized;
    private bool _titleInitialized;
    private bool _leaderboardInitialized;
    private bool _keepScore;

    private void Start()
    {
        Instance = this;

        _scoreText = gameUI.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        _music = GetComponent<AudioSource>();
        
        _state = GameState.Normal;
        _programState = ProgramState.Title;
    }

    private void Update()
    {
        switch (_programState)
        {
            case ProgramState.Title:
                if (!_titleInitialized)
                {
                    gameUI.SetActive(false);
                    titleUI.SetActive(true);
                    titleUI.GetComponent<Animation>().Play("TitleIn");
                    _music.clip = hiHat;
                    _music.Play();
                    _titleInitialized = true;
                }
                break;
            case ProgramState.Game:
                if (!_gameInitialized)
                {
                    gameUI.SetActive(true);
                    _state = GameState.Normal;
                    _score = 0;
                    _keepScore = true;
                    StartCoroutine(ProduceDice());
                    StartCoroutine(SpawnHands());
                    _music.clip = mainTrack;
                    _music.Play();
                    _gameInitialized = true;
                }
                else
                {
                    if (_keepScore && !_choosing)
                    {
                        _score += Time.deltaTime;
                        _scoreText.text = $"SCORE: {Mathf.FloorToInt(_score)}";
                    }
                }
                break;
            case ProgramState.Leaderboard:
                if (!_leaderboardInitialized)
                {
                    leaderboardUI.SetActive(true);
                    leaderboardUI.GetComponent<Animation>().Play("In");
                    leaderboardUI.transform.Find("Leaderboard/MyScore").GetComponent<TextMeshProUGUI>().text =
                        Mathf.FloorToInt(_score).ToString();
                    _music.clip = hiHat;
                    _music.Play();
                    _leaderboardInitialized = true;
                }
                break;
        }
    }

    public void PlayGame()
    {
        StartCoroutine(TitleScreenOutAnim());
        _programState = ProgramState.Game;
        _gameInitialized = false;
    }

    public void TutorialIn()
    {
        titleUI.GetComponent<Animation>().Play("TutorialIn");
    }

    public void TutorialOut()
    {
        titleUI.GetComponent<Animation>().Play("TutorialOut");
    }

    public void BackToTitle()
    {
        StartCoroutine(LeaderboardOutAnim());
    }

    private IEnumerator TitleScreenOutAnim()
    {
        titleUI.GetComponent<Animation>().Play("TitleOut");
        yield return new WaitForSeconds(1f);
        titleUI.SetActive(false);
    }

    private IEnumerator LeaderboardOutAnim()
    {
        leaderboardUI.GetComponent<Animation>().Play("Out");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
    }

    public void AddChipPile(Chips chipPile)
    {
        _chipPiles.Add(chipPile);
        _chipTargets.Add(chipPile);
    }

    public void RemoveChipPile(Chips chipPile)
    {
        _chipPiles.Remove(chipPile);

        if (_chipPiles.Count == 0)
        {
            _keepScore = false;
            gameUI.SetActive(false);
            StartCoroutine(LoseGame());
        }
    }

    public void ReturnChipPile(Chips chipPile)
    {
        if (chipPile != null && !_chipTargets.Contains(chipPile))
        {
            _chipTargets.Add(chipPile);
        }
    }

    public void MakeUnit(DieRoller dieToRoll)
    {
        StartCoroutine(MakeUnitAnim(dieToRoll));
    }
    
    public void UpgradeUnit(DieRoller dieToRoll)
    {
        StartCoroutine(UpgradeUnitAnim(dieToRoll));
    }
    
    private IEnumerator MakeUnitAnim(DieRoller dieToRoll)
    {
        _busy = true;
        
        // Immediately roll glyph die
        dieToRoll.Throw();
        Rigidbody dieRb = dieToRoll.GetComponent<Rigidbody>();
        
        yield return new WaitForSeconds(0.25f); // Buffer for velocity
        yield return new WaitUntil(() => dieRb.velocity.magnitude <= float.Epsilon && dieRb.angularVelocity.magnitude <= float.Epsilon);
        Unit newUnit = Instantiate(units[debugForceUnitType ? (int) debugUnitType - 1 : dieToRoll.GetDieSide() - 1], 
            TileGrid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)) + new Vector3(0.5f, 1.5f, 0.0f), 
            Quaternion.identity).GetComponent<Unit>();
        newUnit.Placing = true;
        helpBox.SetHelp(helpers[(int)newUnit.BugType - 1].sprite, helpers[(int)newUnit.BugType - 1].text);
        _state = GameState.Placing;
        yield return new WaitForSeconds(1f);
        dieToRoll.Spin();
        dieToRoll.SetSpawned(false);
        dieToRoll.transform.parent.gameObject.SetActive(false);
        yield return new WaitUntil(() => !newUnit.Placing);
        helpBox.Dismiss();
        _state = GameState.Normal;
        
        _busy = false;
    }

    private IEnumerator UpgradeUnitAnim(DieRoller dieToRoll)
    {
        _state = GameState.Upgrading;
        _choosing = true;
        bg.color = Color.gray;
        foreach (Unit unit in gameGrid.GetAllUnits())
        {
            unit.ShowStrength();
        }
        
        yield return new WaitUntil(() => UnitToUpgrade != null);

        _choosing = false;
        bg.color = Color.white;

        UnitToUpgrade.Upgrading = true;
        
        dieToRoll.Throw();
        Rigidbody dieRb = dieToRoll.GetComponent<Rigidbody>();
        yield return new WaitForSeconds(0.25f); // Buffer for velocity
        yield return new WaitUntil(() => dieRb.velocity.magnitude <= float.Epsilon && dieRb.angularVelocity.magnitude <= float.Epsilon);
        int extraStrength = dieToRoll.GetDieSide();
        _unitToUpgrade.AddStrength(extraStrength);
        
        dieToRoll.Spin();
        dieToRoll.SetSpawned(false);
        dieToRoll.transform.parent.gameObject.SetActive(false);
        
        _unitToUpgrade = null;
        _state = GameState.Normal;
    }

    private IEnumerator ProduceDice()
    {
        _productionPercentage = 0f;
        
        while (true)
        {
            if (!_dieReady && !dieUIManager.AllSpawned())
            {
                _timeWaited = 0f;
                while (_timeWaited < produceTime)
                {
                    yield return new WaitUntil(() => !_busy && !_choosing);
                    yield return new WaitForEndOfFrame();
                    _timeWaited += Time.deltaTime;
                    _productionPercentage = _timeWaited / produceTime;
                }

                _dieReady = true;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private IEnumerator SpawnHands()
    {
        while (true)
        {
            yield return new WaitForSeconds(_handWaitTime);
            _handWaitTime *= 0.97f;

            Chips goal = GetChipPile();
            if (goal != null && _availableHandAngles.Count > 0)
            {
                float angle = _availableHandAngles.ToArray()[Random.Range(0, _availableHandAngles.Count)];
                _availableHandAngles.Remove(angle);
                Hand hand = Instantiate(hands[Random.Range(0, hands.Length)], goal.transform.position, Quaternion.identity).GetComponent<Hand>();
                hand.ChipGoal = goal;
                hand.ReachSpeedModifier += Mathf.Floor(_score / 50f) * .0075f;
                hand.Angle = angle;
                _handsInPlay.Add(hand);
            }
        }
    }

    private IEnumerator LoseGame()
    {
        // TODO: Display you lose text
        yield return new WaitForSeconds(3f);
        gameUI.SetActive(false);
        _leaderboardInitialized = false;
        _programState = ProgramState.Leaderboard;
    }
    
    private Chips GetChipPile()
    {
        Chips target = null;

        if (_chipTargets.Count > 0)
        {
            int targetIdx = Random.Range(0, _chipTargets.Count);
            target = _chipTargets[targetIdx];
            _chipTargets.RemoveAt(targetIdx);
        }
        
        return target;
    }
}
