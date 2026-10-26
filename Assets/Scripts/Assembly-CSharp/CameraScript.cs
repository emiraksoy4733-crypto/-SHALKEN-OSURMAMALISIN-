using UnityEngine;
using ControlFreak2;

public class CameraScript : MonoBehaviour
{
	[Header("Original Baldi Camera Settings")]
	public GameObject player;

	public PlayerScript ps;

	public Transform baldi;

	public Vector3 BaldiOffset;

	public float initVelocity;

	public float velocity;

	public float gravity;

	private float lookBehind;

	public Vector3 offset;

	public float jumpHeight;

	public Vector3 jumpHeightV3;

	[Header("Dynamic Camera Polish")]
	public float lerpSpeed = 6f;

	[Tooltip("Скорость плавного разворота камеры назад (Look Behind)")]
	public float lookBehindSmoothSpeed = 12f;

	[Space]
	[Header("Head Bobbing")]
	public float bobFrequency = 4.5f;

	public float bobHorizontalAmplitude = 0.05f;

	public float bobVerticalAmplitude = 0.05f;

	[Space]
	[Header("Camera Sway")]
	public float swayAmount = 1.5f;

	public float forwardTiltAmount = 1f;

	public float maxSwayAngle = 4f;

	private Vector3 lastPlayerPos;

	private float currentMovementSpeed;

	private Vector3 localVelocity;

	private float bobTimer;

	private Vector3 currentBobOffset;

	private Quaternion currentSwayRotation;

	private Quaternion initialLocalRotation;

	private float currentLookBehindAngle;

	private void Start()
	{
		offset = base.transform.position - player.transform.position;
		initialLocalRotation = base.transform.localRotation;
	}

	private void Update()
	{
		if (ps.jumpRope)
		{
			velocity -= gravity * Time.deltaTime;
			jumpHeight += velocity * Time.deltaTime;
			if (jumpHeight <= 0f)
			{
				jumpHeight = 0f;
				if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Jump))
				{
					velocity = initVelocity;
				}
			}
			jumpHeightV3 = new Vector3(0f, jumpHeight, 0f);
		}
		else if (Singleton<InputManager>.Instance.GetActionKey(InputAction.LookBehind))
		{
			lookBehind = 180f;
		}
		else
		{
			lookBehind = 0f;
		}
		CalculatePlayerSpeed();
	}

	private void LateUpdate()
	{
		if (ps.gameOver)
		{
			Transform transform = null;
			if (ps.killerTransform != null)
			{
				transform = ps.killerTransform;
			}
			else
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag("NPC");
				float num = float.PositiveInfinity;
				GameObject[] array2 = array;
				foreach (GameObject gameObject in array2)
				{
					if (gameObject.name.Contains("Baldi") || gameObject.name.Contains("Clone") || gameObject.name.Contains("-28"))
					{
						float num2 = Vector3.Distance(player.transform.position, gameObject.transform.position);
						if (num2 < num)
						{
							num = num2;
							transform = gameObject.transform;
						}
					}
				}
			}
			if (transform == null)
			{
				transform = baldi.transform;
			}
			if (transform != null)
			{
				Vector3 position = transform.position + transform.forward * BaldiOffset.z + new Vector3(0f, BaldiOffset.y, 0f);
				base.transform.position = position;
				base.transform.LookAt(new Vector3(transform.position.x, transform.position.y + BaldiOffset.y, transform.position.z));
			}
			return;
		}
		Vector3 vector = player.transform.position + offset;
		if (ps.jumpRope)
		{
			vector += jumpHeightV3;
		}
		Vector3 vector2 = ApplyHeadBob();
		if (!float.IsNaN(vector2.x) && !float.IsNaN(vector2.y) && !float.IsNaN(vector2.z))
		{
			base.transform.position = vector + vector2;
		}
		else
		{
			base.transform.position = vector;
		}
		if (float.IsNaN(lookBehind) || float.IsInfinity(lookBehind))
		{
			lookBehind = 0f;
		}
		if (float.IsNaN(currentLookBehindAngle) || float.IsInfinity(currentLookBehindAngle))
		{
			currentLookBehindAngle = 0f;
		}
		lookBehind = Mathf.Clamp(lookBehind, -360f, 360f);
		currentLookBehindAngle = Mathf.Clamp(currentLookBehindAngle, -360f, 360f);
		currentLookBehindAngle = Mathf.Lerp(currentLookBehindAngle, lookBehind, Time.deltaTime * lookBehindSmoothSpeed);
		Quaternion quaternion = player.transform.rotation * Quaternion.Euler(0f, currentLookBehindAngle, 0f);
		Quaternion quaternion2 = ApplyCameraSway();
		if (float.IsNaN(quaternion2.x) || float.IsNaN(quaternion.x))
		{
			quaternion2 = Quaternion.identity;
		}
		Quaternion rotation = quaternion * quaternion2;
		if (!float.IsNaN(rotation.x) && !float.IsNaN(rotation.y) && !float.IsNaN(rotation.z) && !float.IsNaN(rotation.w) && !float.IsInfinity(rotation.x) && !float.IsInfinity(rotation.y))
		{
			rotation.Normalize();
			base.transform.rotation = rotation;
		}
		else
		{
			base.transform.rotation = player.transform.rotation;
		}
	}

	private void CalculatePlayerSpeed()
	{
		Vector3 position = player.transform.position;
		Vector3 direction = (position - lastPlayerPos) / Time.deltaTime;
		direction.y = 0f;
		currentMovementSpeed = direction.magnitude;
		localVelocity = player.transform.InverseTransformDirection(direction);
		lastPlayerPos = position;
	}

	private Vector3 ApplyHeadBob()
	{
		bool flag = CF2Input.GetKey(KeyCode.LeftShift) || CF2Input.GetKey(KeyCode.RightShift);
		if (currentMovementSpeed > 0.1f && !ps.jumpRope)
		{
			float num = ((flag && ps.stamina > 0f) ? 0.6f : 0.4f);
			bobTimer += Time.deltaTime * bobFrequency * (currentMovementSpeed * num);
			float num2 = Mathf.Sin(bobTimer) * bobHorizontalAmplitude;
			float num3 = Mathf.Cos(bobTimer * 2f) * bobVerticalAmplitude;
			if (flag && ps.stamina > 0f)
			{
				num2 *= 1.3f;
				num3 *= 1.3f;
			}
			currentBobOffset = Vector3.Lerp(currentBobOffset, new Vector3(num2, num3, 0f), Time.deltaTime * lerpSpeed);
		}
		else
		{
			bobTimer = 0f;
			currentBobOffset = Vector3.Lerp(currentBobOffset, Vector3.zero, Time.deltaTime * lerpSpeed);
		}
		return currentBobOffset;
	}

	private Quaternion ApplyCameraSway()
	{
		if (ps.jumpRope)
		{
			return Quaternion.identity;
		}
		float value = (0f - localVelocity.x) * swayAmount;
		float value2 = (0f - localVelocity.z) * forwardTiltAmount;
		value = Mathf.Clamp(value, 0f - maxSwayAngle, maxSwayAngle);
		value2 = Mathf.Clamp(value2, 0f - maxSwayAngle, maxSwayAngle);
		if (float.IsNaN(value))
		{
			value = 0f;
		}
		if (float.IsNaN(value2))
		{
			value2 = 0f;
		}
		Quaternion b = Quaternion.Euler(value2, 0f, value);
		currentSwayRotation = Quaternion.Slerp(currentSwayRotation, b, Time.deltaTime * lerpSpeed);
		return currentSwayRotation;
	}
}
