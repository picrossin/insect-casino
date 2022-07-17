using UnityEngine;

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
	[SerializeField] private float retractSpeed = 0.1f;
	[SerializeField] private Sprite reachSprite;
	[SerializeField] private Sprite grabSprite;
	[SerializeField] private Sprite slamSprite;
	
	private Chips _chipGoal;
	public Chips ChipGoal
	{
		get => _chipGoal;
		set => _chipGoal = value;
	}

	private HandState _state;
	private float[] _anglesTest = new[] {0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f};
	private SpriteRenderer _sprite;

	private void Start()
	{
		_state = HandState.Spawning;
		_sprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
	}
	
	private void FixedUpdate()
	{
		CheckTransitions();
		
		switch (_state)
		{
			case HandState.Spawning:
				break;
			case HandState.ReachingIn:
				transform.position -= transform.right * reachSpeed;
				break;
			case HandState.Attacking:
				break;
			case HandState.Grabbing:
				break;
			case HandState.Retracting:
				transform.position += transform.right * retractSpeed;
				break;
			case HandState.Hurting:
				break;
		}
	}

	private void CheckTransitions()
	{
		switch (_state)
		{
			case HandState.Spawning:
				transform.position = _chipGoal.transform.position;
				transform.rotation = Quaternion.AngleAxis(_anglesTest[Random.Range(0, _anglesTest.Length)], Vector3.back);
				transform.position += transform.right * 16f;
				transform.Find("Sprite").gameObject.SetActive(true);
				_state = HandState.ReachingIn;
				break;
			case HandState.ReachingIn:
				if (Vector2.Distance(transform.position, _chipGoal.transform.position) <= 0.5f)
				{
					transform.position = _chipGoal.transform.position;
					_state = HandState.Grabbing;
				}
				break;
			case HandState.Attacking:
				break;
			case HandState.Grabbing:
				if (_chipGoal.TakeChip())
				{
					GameManager.Instance.ReturnChipPile(_chipGoal);
				}
				_sprite.sprite = grabSprite;
				_state = HandState.Retracting;
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
}
