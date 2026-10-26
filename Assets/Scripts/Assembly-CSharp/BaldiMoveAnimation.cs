using UnityEngine;

public class BaldiMoveAnimation : MonoBehaviour
{
	public Animator anim;

	public SpriteRenderer spriteRenderer;

	public Sprite idleSprite;

	public AudioSource audioSource;

	[Header("Movement Settings")]
	public float moveThreshold = 0.01f;

	public float slowThreshold = 0.05f;

	public float dragThreshold = 1.5f;

	[Header("Animation Settings")]
	public float normalAnimSpeed = 1f;

	public float slowAnimSpeed = 0.5f;

	public float dragAnimSpeed = 2.5f;

	[Header("Audio Settings")]
	public float normalPitch = 1f;

	public float slowPitch = 0.5f;

	public float dragPitch = 3f;

	public float normalVolume = 1f;

	private Vector3 lastPosition;

	private void Start()
	{
		lastPosition = base.transform.position;
	}

	private void Update()
	{
		float num = Vector3.Distance(base.transform.position, lastPosition);
		if (num > moveThreshold)
		{
			anim.enabled = true;
			if (num > dragThreshold)
			{
				anim.speed = dragAnimSpeed;
				audioSource.pitch = dragPitch;
				audioSource.volume = normalVolume;
			}
			else if (num < slowThreshold)
			{
				anim.speed = slowAnimSpeed;
				audioSource.pitch = slowPitch;
				audioSource.volume = normalVolume;
			}
			else
			{
				anim.speed = normalAnimSpeed;
				audioSource.pitch = normalPitch;
				audioSource.volume = normalVolume;
			}
		}
		else
		{
			anim.enabled = false;
			spriteRenderer.sprite = idleSprite;
			audioSource.volume = 0f;
		}
		lastPosition = base.transform.position;
	}
}
