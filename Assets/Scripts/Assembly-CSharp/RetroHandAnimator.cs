using UnityEngine;
using UnityEngine.UI;
using ControlFreak2;

public class RetroHandAnimator : MonoBehaviour
{
	[Header("Dependencies")]
	[SerializeField]
	private PlayerScript playerScript;

	[SerializeField]
	private Camera playerCam;

	private Image handImage;

	[Header("Sprites for Idle/Walk (3 Images)")]
	[Tooltip("Кадр 0 (Покой/Центр), Кадр 1 (Влево), Кадр 2 (Вправо)")]
	public Sprite[] idleSprites;

	[Header("Sprites for Grab/Interact (3 Images)")]
	[Tooltip("Кадр 0 (Замах), Кадр 1 (Хват), Кадр 2 (Фиксация при удержании)")]
	public Sprite[] grabSprites;

	[Header("Retro Animation Timers")]
	public float walkFrameTime = 0.2f;

	public float grabFrameTime = 0.08f;

	private float animTimer;

	private int currentWalkFrame;

	private int currentGrabFrame;

	private bool isHoveringObject;

	private bool wasHoveringLastFrame;

	private void Start()
	{
		handImage = GetComponent<Image>();
		if (playerScript == null)
		{
			playerScript = Object.FindObjectOfType<PlayerScript>();
		}
		if (playerCam == null)
		{
			playerCam = Camera.main;
		}
		if (idleSprites != null && idleSprites.Length != 0)
		{
			handImage.sprite = idleSprites[0];
		}
	}

	private void Update()
	{
		if (playerScript == null || handImage == null)
		{
			return;
		}
		CheckForInteractableHover();
		if (isHoveringObject)
		{
			if (!wasHoveringLastFrame)
			{
				currentGrabFrame = 0;
				animTimer = 0f;
			}
			PlayGrabHoverAnimation();
		}
		else
		{
			PlayRetroWalkAnimation();
		}
		wasHoveringLastFrame = isHoveringObject;
	}

	private void CheckForInteractableHover()
	{
		if (playerCam == null || playerScript.jumpRope)
		{
			isHoveringObject = false;
			return;
		}
		if (Physics.Raycast(playerCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), out var hitInfo))
		{
			bool num = hitInfo.collider.tag == "Door" || hitInfo.collider.tag == "Item" || hitInfo.collider.tag == "Notebook";
			float num2 = Vector3.Distance(playerScript.transform.position, hitInfo.collider.transform.position);
			if (num && num2 <= 10f)
			{
				isHoveringObject = true;
				return;
			}
		}
		isHoveringObject = false;
	}

	private void PlayRetroWalkAnimation()
	{
		if (idleSprites == null || idleSprites.Length < 3)
		{
			return;
		}
		if (playerScript.cc.velocity.magnitude > 0.1f)
		{
			float num = (((CF2Input.GetKey(KeyCode.LeftShift) || CF2Input.GetKey(KeyCode.RightShift)) && playerScript.stamina > 0f) ? (walkFrameTime * 0.6f) : walkFrameTime);
			animTimer += Time.deltaTime;
			if (animTimer >= num)
			{
				animTimer = 0f;
				currentWalkFrame++;
				switch (currentWalkFrame % 4)
				{
				case 0:
					handImage.sprite = idleSprites[0];
					break;
				case 1:
					handImage.sprite = idleSprites[1];
					break;
				case 2:
					handImage.sprite = idleSprites[0];
					break;
				case 3:
					handImage.sprite = idleSprites[2];
					break;
				}
			}
		}
		else
		{
			animTimer = 0f;
			currentWalkFrame = 0;
			handImage.sprite = idleSprites[0];
		}
	}

	private void PlayGrabHoverAnimation()
	{
		if (grabSprites == null || grabSprites.Length < 3)
		{
			return;
		}
		if (currentGrabFrame < 2)
		{
			animTimer += Time.deltaTime;
			if (animTimer >= grabFrameTime)
			{
				animTimer = 0f;
				currentGrabFrame++;
				handImage.sprite = grabSprites[currentGrabFrame];
			}
		}
		else
		{
			handImage.sprite = grabSprites[2];
		}
	}
}
