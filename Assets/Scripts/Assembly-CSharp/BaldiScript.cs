using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BaldiScript : MonoBehaviour
{
	public bool db;

	[Header("Start Delay Settings")]
	[Tooltip("Сколько СЕКУНД Балди стоит на месте перед тем, как начать бежать")]
	public float baseTime = 10f;

	public float speed;

	public float timeToMove;

	public float baldiAnger;

	public float baldiTempAnger;

	public float baldiWait;

	public float baldiSpeedScale;

	private float moveFrames;

	private float currentPriority;

	public bool antiHearing;

	public float antiHearingTime;

	public float vibrationDistance;

	public float angerRate;

	public float angerRateRate;

	public float angerFrequency;

	public float timeToAnger;

	public bool endless;

	public Transform player;

	public Transform wanderTarget;

	public AILocationSelectorScript wanderer;

	private AudioSource baldiAudio;

	public AudioClip slap;

	public Animator baldiAnimator;

	public float coolDown;

	private Vector3 previous;

	private bool rumble;

	private NavMeshAgent agent;

	public bool canChase;

	[Header("Music Settings")]
	public AudioSource music;

	[Tooltip("Через сколько СЕКУНД после старта сцены включится музыка")]
	public float waitTime = 5f;

	[Header("Numberslop Timing Settings")]
	[Tooltip("Интервал между шагами/звуками в МИЛЛИСЕКУНДАХ (1000 мс = 1 секунда)")]
	[Range(10f, 2000f)]
	public float baldiWaitMS = 250f;

	private bool startTimerFinished;

	private void Start()
	{
		StartCoroutine(wait());
		baldiAudio = GetComponent<AudioSource>();
		agent = GetComponent<NavMeshAgent>();
		baldiSpeedScale = 0.2f;
		vibrationDistance = 50f;
		baldiWait = baldiWaitMS / 1000f;
		timeToMove = baseTime;
		Wander();
		if (PlayerPrefs.GetInt("Rumble") == 1)
		{
			rumble = true;
		}
	}

	private IEnumerator wait()
	{
		yield return new WaitForSeconds(waitTime);
		if (music != null)
		{
			music.Play();
		}
	}

	private void Update()
	{
		if (!startTimerFinished)
		{
			if (timeToMove > 0f)
			{
				timeToMove -= Time.deltaTime;
				if (agent != null)
				{
					agent.speed = 0f;
				}
				return;
			}
			startTimerFinished = true;
			canChase = true;
			baldiWait = baldiWaitMS / 1000f;
			timeToMove = baldiWait;
		}
		if (canChase)
		{
			if (timeToMove > 0f)
			{
				timeToMove -= Time.deltaTime;
			}
			else
			{
				Move();
			}
		}
		if (coolDown > 0f)
		{
			coolDown -= 1f * Time.deltaTime;
		}
		if (antiHearingTime > 0f)
		{
			antiHearingTime -= Time.deltaTime;
		}
		else
		{
			antiHearing = false;
		}
	}

	private void FixedUpdate()
	{
		if (canChase && moveFrames > 0f && agent != null)
		{
			moveFrames--;
			agent.speed = speed;
		}
		else if (agent != null)
		{
			agent.speed = 0f;
		}
		Vector3 direction = player.position - base.transform.position;
		if (Physics.Raycast(base.transform.position + Vector3.up * 2f, direction, out var hitInfo, float.PositiveInfinity, 769, QueryTriggerInteraction.Ignore) & (hitInfo.transform.tag == "Player"))
		{
			db = true;
			TargetPlayer();
		}
		else
		{
			db = false;
		}
	}

	private void Move()
	{
		if (canChase)
		{
			if (base.transform.position == previous && coolDown < 0f)
			{
				Wander();
			}
			moveFrames = 10f;
			baldiWait = baldiWaitMS / 1000f;
			timeToMove = baldiWait;
			previous = base.transform.position;
			if (baldiAudio != null && slap != null)
			{
				baldiAudio.PlayOneShot(slap);
			}
		}
	}

	private void Wander()
	{
		if (wanderer != null && wanderTarget != null)
		{
			wanderer.GetNewTarget();
			if (agent != null && agent.isActiveAndEnabled)
			{
				agent.SetDestination(wanderTarget.position);
			}
		}
		coolDown = 1f;
		currentPriority = 0f;
	}

	public void TargetPlayer()
	{
		if (canChase && agent != null && agent.isActiveAndEnabled && player != null)
		{
			agent.SetDestination(player.position);
			coolDown = 1f;
			currentPriority = 0f;
		}
	}

	public void GetAngry(float value)
	{
	}

	public void GetTempAngry(float value)
	{
	}

	public void Hear(Vector3 soundLocation, float priority)
	{
		if (!antiHearing && priority >= currentPriority && canChase && agent != null && agent.isActiveAndEnabled)
		{
			agent.SetDestination(soundLocation);
			currentPriority = priority;
		}
	}

	public void ActivateAntiHearing(float t)
	{
		Wander();
		antiHearing = true;
		antiHearingTime = t;
	}
}
