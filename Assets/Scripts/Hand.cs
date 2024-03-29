using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
	public enum HandState
	{
		Spawning,
		ReachingIn,
		Attacking,
		Grabbing,
		Retracting,
		Hurting
	}
	
	[SerializeField] private float reachSpeed = 0.1f;
	public float ReachSpeedModifier;
	
	[SerializeField] private float retractSpeed = 0.1f;
	[SerializeField] private Sprite reachSprite;
	[SerializeField] private Sprite grabSprite;
	[SerializeField] private Sprite slamSprite;
	[SerializeField] private LayerMask handHurter;
	[SerializeField] private LayerMask playerMask;
	[SerializeField] private GameObject smash;
	
	private Chips _chipGoal;
	public Chips ChipGoal
	{
		get => _chipGoal;
		set => _chipGoal = value;
	}

	private int _queueDamage;
	public int QueueDamage
	{
		get => _queueDamage;
		set => _queueDamage = value;
	}

	private bool _queueSlowdown;
	public bool QueueSlowdown
	{
		get => _queueSlowdown;
		set => _queueSlowdown = value;
	}
	
	private HandState _state;
	public HandState State => _state;

	private float _angle;
	public float Angle
	{
		get => _angle;
		set => _angle = value;
	}

	private const float SLOWDOWN_TIME = 2f;

	private SpriteRenderer _sprite;
	private Transform _healthCanvas;
	private StrengthBar _healthBar;
	private int _health = 6;
	private bool _attacked;
	private bool _returnedChips;

	private void Start()
	{
		StartCoroutine(DisableColl());
		_state = HandState.Spawning;
		_sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		_healthCanvas = transform.Find("HealthCanvas");
		_healthBar = _healthCanvas.GetComponentInChildren<StrengthBar>();
		_healthBar.SetStrength(_health);
	}
	
	private void FixedUpdate()
	{
		CheckTransitions();
		
		switch (_state)
		{
			case HandState.Spawning:
				break;
			case HandState.ReachingIn:
				if (GameManager.Instance.Choosing) return;
				
				transform.position -= transform.right * (reachSpeed + ReachSpeedModifier);
				_healthCanvas.rotation = Quaternion.identity;
				break;
			case HandState.Attacking:
				_healthCanvas.rotation = Quaternion.identity;
				break;
			case HandState.Grabbing:
				break;
			case HandState.Retracting:
				_sprite.color = Color.gray;
				transform.position += transform.right * retractSpeed;
				break;
			case HandState.Hurting:
				_healthCanvas.rotation = Quaternion.identity;
				break;
		}
	}

	private void CheckTransitions()
	{
		switch (_state)
		{
			case HandState.Spawning:
				transform.position = _chipGoal.transform.position;
				transform.rotation = Quaternion.AngleAxis(_angle, Vector3.back);
				transform.position += transform.right * 16f;
				transform.Find("Sprite").gameObject.SetActive(true);
				_healthCanvas.gameObject.SetActive(true);
				_state = HandState.ReachingIn;
				break;
			case HandState.ReachingIn:
				Collider2D coll = Physics2D.OverlapCircle(transform.position, 0.75f, handHurter);
				Collider2D playerColl = Physics2D.OverlapCircle(transform.position, 0.75f, playerMask);

				if (coll != null)
				{
					if (coll.CompareTag("Web"))
					{
						_queueDamage += 2;
					}
					else
					{
						_queueDamage += 6;
					}
					
					Destroy(coll.gameObject);
				}
				
				if (_queueDamage > 0)
				{
					StartCoroutine(TakeDamage(_queueDamage));
				} 
				else if (_queueSlowdown)
				{
					StartCoroutine(Slowdown());
				}

				if (!_attacked && playerColl != null)
				{
					StartCoroutine(AttackPlayer(playerColl.GetComponent<Unit>()));
				}
				else if (_chipGoal == null)
				{
					_state = HandState.Retracting;
					GameManager.Instance.HandsInPlay.Remove(this);
					GameManager.Instance.AvailableHandAngles.Add(_angle);
					_healthCanvas.gameObject.SetActive(false);
					GetComponent<CircleCollider2D>().enabled = false;
				}
				else if (Vector2.Distance(transform.position, _chipGoal.transform.position) <= 0.5f)
				{
					transform.position = _chipGoal.transform.position;
					_state = HandState.Grabbing;
				}
				break;
			
			case HandState.Attacking:
				Collider2D coll1 = Physics2D.OverlapCircle(transform.position, 0.75f, handHurter);

				if (coll1 != null)
				{
					Destroy(coll1.gameObject);

					if (coll1.CompareTag("Web"))
					{
						_queueDamage += 2;
					}
					else
					{
						_queueDamage += 6;
					}
				}
				
				if (_queueDamage > 0)
				{
					StartCoroutine(TakeDamage(_queueDamage));
				}
				break;
			case HandState.Grabbing:
				if (_chipGoal.TakeChip() && !_returnedChips)
				{
					_returnedChips = true;
					GameManager.Instance.
						ReturnChipPile(_chipGoal);
				}
				_sprite.sprite = grabSprite;
				_state = HandState.Retracting;
				GameManager.Instance.HandsInPlay.Remove(this);
				GameManager.Instance.AvailableHandAngles.Add(_angle);
				_healthCanvas.gameObject.SetActive(false);
				GetComponent<CircleCollider2D>().enabled = false;
				break;
			case HandState.Retracting:
				if (Vector2.Distance(transform.position, Vector2.zero) > 16f)
				{
					Destroy(gameObject);
				}
				break;
			case HandState.Hurting:
				break;
		}
	}

	private IEnumerator TakeDamage(int dmg)
	{
		_state = HandState.Hurting;
		_health = Mathf.Max(_health - dmg, 0);
		_queueDamage = 0;
		_healthCanvas.Find("HealthText").GetComponent<TextMeshProUGUI>().text = $"{_health}/6";
		_healthBar.SetStrength(_health);
		Instantiate(smash, transform.position, Quaternion.identity);
		yield return new WaitForSeconds(0.25f);

		if (_health <= 0)
		{
			if (!_returnedChips)
			{
				_returnedChips = true;
				GameManager.Instance.ReturnChipPile(_chipGoal);
			}
			GameManager.Instance.HandsInPlay.Remove(this);
			GameManager.Instance.AvailableHandAngles.Add(_angle);
			_healthCanvas.gameObject.SetActive(false);
		}

		_state = _health > 0 ? HandState.ReachingIn : HandState.Retracting;
	}

	private IEnumerator Slowdown()
	{
		_queueSlowdown = false;
		float originalSpeed = reachSpeed;
		reachSpeed = originalSpeed / 2f;
		yield return new WaitForSeconds(SLOWDOWN_TIME);
		reachSpeed = originalSpeed;
	}

	private IEnumerator AttackPlayer(Unit unitToAttack)
	{
		if (unitToAttack.Placing) yield break;

		_state = HandState.Attacking;
		_attacked = true;
		_sprite.sprite = slamSprite;
		
		Vector3 originalPos = transform.position;
		Vector3 newPos = unitToAttack.transform.position;
		float timeTaken = 0f;
		while (timeTaken < 0.5f)
		{
			yield return new WaitUntil(() => !GameManager.Instance.Choosing);
			transform.position = Vector3.Slerp(originalPos, newPos, timeTaken / 0.5f);
			yield return new WaitForEndOfFrame();
			timeTaken += Time.deltaTime;
		}
		
		transform.position = newPos;
		yield return new WaitForSeconds(0.25f);

		bool retract = false;
		
		if (GameManager.Instance.State == GameManager.GameState.Upgrading && GameManager.Instance.UnitToUpgrade == unitToAttack)
		{
			print("Frame save");
		}
		else
		{
			if (unitToAttack.BugType != GameManager.UnitType.Cards && unitToAttack != null)
			{
				unitToAttack.Die();
			}
			else
			{
				if (!((CardTower) unitToAttack).Hurt(1))
				{
					// _attacked = false;
				}
			}
			
			if (!_returnedChips)
			{
				_returnedChips = true;
				GameManager.Instance.ReturnChipPile(_chipGoal);
			}
			GameManager.Instance.HandsInPlay.Remove(this);
			GameManager.Instance.AvailableHandAngles.Add(_angle);
			_healthCanvas.gameObject.SetActive(false);
			retract = true;
		}
		
		
		timeTaken = 0f;
		while (timeTaken < 0.5f)
		{
			transform.position = Vector3.Slerp(newPos, originalPos, timeTaken / 0.5f);
			yield return new WaitForEndOfFrame();
			timeTaken += Time.deltaTime;
		}
		
		transform.position = originalPos;

		_sprite.sprite = reachSprite;
		_state = retract ? HandState.Retracting : HandState.ReachingIn;
	}

	private IEnumerator DisableColl()
	{
		GetComponent<CircleCollider2D>().enabled = false;
		yield return new WaitForSeconds(1f);
		GetComponent<CircleCollider2D>().enabled = true;
	}
}
