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
	
	private Chips _chipGoal;
	public Chips ChipGoal
	{
		get => _chipGoal;
		set => _chipGoal = value;
	}

	private HandState _state;

	private void Start()
	{
		_state = HandState.Spawning;
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
				transform.rotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.back);
				transform.position = transform.right * 16f;
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
				// TODO: Change sprite and subtract chip, add chip target back
				_state = HandState.Retracting;
				break;
			case HandState.Retracting:
				if (Vector2.Distance(transform.position, _chipGoal.transform.position) > 16f)
				{
					Destroy(gameObject);
				}
				break;
			case HandState.Hurting:
				break;
		}
	}
}
