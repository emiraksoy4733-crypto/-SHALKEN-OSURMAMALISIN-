using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ControlFreak2;

public class GameControllerScript : MonoBehaviour
{
	public CursorControllerScript cursorController;

	public PlayerScript player;

	public Transform playerTransform;

	public CharacterController playerCharacter;

	public Collider playerCollider;

	public AILocationSelectorScript AILocationSelector;

	public Transform cameraTransform;

	public Camera playerCamera;

	private int cullingMask;

	public EntranceScript entrance_0;

	public EntranceScript entrance_1;

	public EntranceScript entrance_2;

	public EntranceScript entrance_3;

	public GameObject[] baldiTutor;

	public GameObject[] baldi;

	public BaldiScript baldiScrpt;

	public AudioClip aud_Prize;

	public AudioClip aud_PrizeMobile;

	public AudioClip aud_AllNotebooks;

	public AudioClip aud_Teleport;

	private bool flipped;

	public GameObject principal;

	public GameObject crafters;

	public GameObject playtime;

	public PlaytimeScript playtimeScript;

	public GameObject gottaSweep;

	public GameObject bully;

	public GameObject firstPrize;

	public GameObject TestEnemy;

	public FirstPrizeScript firstPrizeScript;

	public GameObject quarter;

	public AudioSource tutorBaldi;

	public RectTransform boots;

	public string mode;

	public int notebooks;

	public int failedNotebooks;

	public bool spoopMode;

	public bool finaleMode;

	public bool debugMode;

	public bool mouseLocked;

	public int exitsReached;

	public int itemSelected;

	public int[] item = new int[5];

	public RawImage[] itemSlot = new RawImage[5];

	public string[] itemNames = new string[12]
	{
		"Nothing", "Energy flavored Zesty Bar", "Yellow Door Lock", "Principal's Keys", "BSODA", "Quarter", "Baldi Anti Hearing and Disorienting Tape", "Alarm Clock", "WD-NoSquee (Door Type)", "Safety Scissors",
		"Big Ol' Boots", "Teleportation Teleporter"
	};

	public TMP_Text itemText;

	public Texture[] itemTextures = new Texture[10];

	public GameObject bsodaSpray;

	public GameObject alarmClock;

	public TMP_Text notebookCount;

	public GameObject pauseMenu;

	public string ExitGameScene;

	public GameObject highScoreText;

	public GameObject warning;

	public GameObject reticle;

	public RectTransform itemSelect;

	public int[] itemSelectOffset;

	public int itemSelectPosition;

	private bool gamePaused;

	private bool learningActive;

	private float gameOverDelay;

	public string GameOverScene;

	private AudioSource audioDevice;

	public AudioClip aud_Soda;

	public AudioClip aud_Spray;

	public AudioClip aud_buzz;

	public AudioClip aud_Hang;

	public AudioClip aud_MachineQuiet;

	public AudioClip aud_MachineStart;

	public AudioClip aud_MachineRev;

	public AudioClip aud_MachineLoop;

	public AudioClip aud_Switch;

	public AudioSource schoolMusic;

	public AudioSource learnMusic;

	public AudioSource soundSource;

	public TextFadeOut fade;

	public string sceneName;

	public DefterHizArttirici hizArttirici;

	public ItemEffect itemPickupEffect;

	public RectTransform[] itemSlots;

	public AudioClip aud_ItemPickup;

	public bool karakterGelsinmi = true;

	[Header("Numberslop Custom Screams")]
	public AudioClip KahretsinScreamSound;

	private void Start()
	{
		sceneName = SceneManager.GetActiveScene().name;
		cullingMask = playerCamera.cullingMask;
		audioDevice = GetComponent<AudioSource>();
		mode = PlayerPrefs.GetString("CurrentMode");
		if (mode == "endless")
		{
			baldiScrpt.endless = true;
		}
		schoolMusic.Play();
		LockMouse();
		UpdateNotebookCount();
		itemSelected = 0;
		gameOverDelay = 0.5f;
	}

	private void Update()
	{
		if (!learningActive)
		{
			if (Singleton<InputManager>.Instance.GetActionKeyDown(InputAction.PauseOrCancel) && !player.gameOver)
			{
				if (!gamePaused)
				{
					PauseGame();
				}
				else
				{
					UnpauseGame();
				}
			}
			if (CF2Input.GetKeyDown(KeyCode.Y) & gamePaused)
			{
				ExitGame();
			}
			else if (CF2Input.GetKeyDown(KeyCode.N) & gamePaused)
			{
				UnpauseGame();
			}
			if (!gamePaused & (Time.timeScale != 1f))
			{
				Time.timeScale = 1f;
			}
			if (Singleton<InputManager>.Instance.GetActionKeyDown(InputAction.UseItem) && Time.timeScale != 0f)
			{
				UseItem();
			}
			if (CF2Input.GetAxis("Mouse ScrollWheel") > 0f && Time.timeScale != 0f)
			{
				DecreaseItemSelection();
			}
			else if (CF2Input.GetAxis("Mouse ScrollWheel") < 0f && Time.timeScale != 0f)
			{
				IncreaseItemSelection();
			}
			if (Time.timeScale != 0f)
			{
				if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Slot0))
				{
					itemSelected = 0;
					UpdateItemSelection();
				}
				else if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Slot1))
				{
					itemSelected = 1;
					UpdateItemSelection();
				}
				else if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Slot2))
				{
					itemSelected = 2;
					UpdateItemSelection();
				}
				else if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Slot3))
				{
					itemSelected = 3;
					UpdateItemSelection();
				}
				else if (Singleton<InputManager>.Instance.GetActionKey(InputAction.Slot4))
				{
					itemSelected = 4;
					UpdateItemSelection();
				}
			}
		}
		else if (Time.timeScale != 0f)
		{
			Time.timeScale = 0f;
		}
		if ((player.stamina < 0f) & !warning.activeSelf)
		{
			warning.SetActive(value: true);
		}
		else if ((player.stamina > 0f) & warning.activeSelf)
		{
			warning.SetActive(value: false);
		}
		if (player.gameOver)
		{
			if (mode == "endless" && notebooks > PlayerPrefs.GetInt("HighBooks") && !highScoreText.activeSelf)
			{
				highScoreText.SetActive(value: true);
			}
			if (gameOverDelay > 0.48f)
			{
				if (player.killerTransform != null && player.killerTransform.name.Contains("KBaldi") && KahretsinScreamSound != null)
				{
					aud_buzz = KahretsinScreamSound;
				}
				if (aud_buzz != null)
				{
					Vector3 position = playerCamera.transform.position + playerCamera.transform.forward;
					AudioSource.PlayClipAtPoint(aud_buzz, position, 1f);
				}
			}
			Time.timeScale = 0f;
			gameOverDelay -= Time.unscaledDeltaTime * 0.5f;
			playerCamera.farClipPlane = gameOverDelay * 400f;
			if (gameOverDelay <= 0f)
			{
				if (mode == "endless")
				{
					if (notebooks > PlayerPrefs.GetInt("HighBooks"))
					{
						PlayerPrefs.SetInt("HighBooks", notebooks);
					}
					PlayerPrefs.SetInt("CurrentBooks", notebooks);
				}
				Time.timeScale = 1f;
				SceneManager.LoadScene(GameOverScene);
			}
		}
		if (finaleMode && !audioDevice.isPlaying && exitsReached == 3)
		{
			audioDevice.clip = aud_MachineLoop;
			audioDevice.loop = true;
			audioDevice.Play();
		}
	}

	private void UpdateNotebookCount()
	{
		if (mode == "story")
		{
			notebookCount.text = notebooks + "/20 OsurmamalisinBooks";
		}
		else if (sceneName == "55Defter")
		{
			notebookCount.text = notebooks + "/55 KahretsinBooks";
		}
		if ((notebooks == 20) & (mode == "story"))
		{
			ActivateFinaleMode();
		}
		else if ((notebooks == 55) & (sceneName == "55Defter"))
		{
			ActivateFinaleMode();
		}
	}

	public void CollectNotebook()
	{
		notebooks++;
		soundSource.Play();
		hizArttirici.HizArttir();
		fade.StartCoroutine(fade.FadeOut());
		UpdateNotebookCount();
	}

	public void LockMouse()
	{
		if (!learningActive)
		{
			cursorController.LockCursor();
			mouseLocked = true;
			reticle.SetActive(value: true);
		}
	}

	public void UnlockMouse()
	{
		cursorController.UnlockCursor();
		mouseLocked = false;
		reticle.SetActive(value: false);
	}

	public void PauseGame()
	{
		if (!learningActive)
		{
			UnlockMouse();
			AudioSource[] array = Object.FindObjectsOfType<AudioSource>();
			Time.timeScale = 0f;
			gamePaused = true;
			pauseMenu.SetActive(value: true);
			AudioSource[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Pause();
			}
		}
	}

	public void ExitGame()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void UnpauseGame()
	{
		AudioSource[] array = Object.FindObjectsOfType<AudioSource>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].UnPause();
		}
		Time.timeScale = 1f;
		gamePaused = false;
		pauseMenu.SetActive(value: false);
		LockMouse();
	}

	public void ActivateSpoopMode()
	{
		spoopMode = true;
		entrance_0.Lower();
		entrance_1.Lower();
		entrance_2.Lower();
		entrance_3.Lower();
		GameObject[] array = baldiTutor;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(value: false);
		}
		array = baldi;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].SetActive(value: true);
		}
		if (karakterGelsinmi)
		{
			principal.SetActive(value: true);
			crafters.SetActive(value: true);
			playtime.SetActive(value: true);
			gottaSweep.SetActive(value: true);
			bully.SetActive(value: true);
			firstPrize.SetActive(value: true);
		}
		audioDevice.PlayOneShot(aud_Hang);
		learnMusic.Stop();
		schoolMusic.Stop();
	}

	private void ActivateFinaleMode()
	{
		finaleMode = true;
		entrance_0.Raise();
		entrance_1.Raise();
		entrance_2.Raise();
		entrance_3.Raise();
	}

	public void GetAngry(float value)
	{
		if (!spoopMode)
		{
			ActivateSpoopMode();
		}
		baldiScrpt.GetAngry(value);
	}

	public void ActivateLearningGame()
	{
		learningActive = true;
		UnlockMouse();
		tutorBaldi.Stop();
		if (!spoopMode)
		{
			schoolMusic.Stop();
			learnMusic.Play();
		}
	}

	public void DeactivateLearningGame(GameObject subject)
	{
		playerCamera.cullingMask = cullingMask;
		learningActive = false;
		Object.Destroy(subject);
		LockMouse();
		if (player.stamina < 100f)
		{
			player.stamina = 100f;
		}
		if (!spoopMode)
		{
			learnMusic.Stop();
		}
		if ((notebooks == 1) & !spoopMode)
		{
			quarter.SetActive(value: true);
			tutorBaldi.PlayOneShot(aud_Prize);
		}
		else if ((notebooks == 20) & (mode == "story"))
		{
			audioDevice.PlayOneShot(aud_AllNotebooks, 0.8f);
		}
		else if ((notebooks == 55) & (sceneName == "55Defter"))
		{
			audioDevice.PlayOneShot(aud_AllNotebooks, 0.8f);
		}
	}

	private void IncreaseItemSelection()
	{
		itemSelected++;
		if (itemSelected > 4)
		{
			itemSelected = 0;
		}
		itemSelect.anchoredPosition = new Vector3(itemSelectOffset[itemSelected], -2f);
		UpdateItemName();
	}

	private void DecreaseItemSelection()
	{
		itemSelected--;
		if (itemSelected < 0)
		{
			itemSelected = 4;
		}
		itemSelect.anchoredPosition = new Vector3(itemSelectOffset[itemSelected], -2f);
		UpdateItemName();
	}

	private void UpdateItemSelection()
	{
		itemSelect.anchoredPosition = new Vector3(itemSelectOffset[itemSelected], -2f);
		UpdateItemName();
	}

	public void CollectItem(int item_ID)
	{
		audioDevice.PlayOneShot(aud_ItemPickup);
		int num = -1;
		if (item[0] == 0)
		{
			item[0] = item_ID;
			itemSlot[0].texture = itemTextures[item_ID];
			num = 0;
		}
		else if (item[1] == 0)
		{
			item[1] = item_ID;
			itemSlot[1].texture = itemTextures[item_ID];
			num = 1;
		}
		else if (item[2] == 0)
		{
			item[2] = item_ID;
			itemSlot[2].texture = itemTextures[item_ID];
			num = 2;
		}
		else if (item[3] == 0)
		{
			item[3] = item_ID;
			itemSlot[3].texture = itemTextures[item_ID];
			num = 3;
		}
		else if (item[4] == 0)
		{
			item[4] = item_ID;
			itemSlot[4].texture = itemTextures[item_ID];
			num = 4;
		}
		else
		{
			item[itemSelected] = item_ID;
			itemSlot[itemSelected].texture = itemTextures[item_ID];
			num = itemSelected;
		}
		UpdateItemName();
		if (itemPickupEffect != null && num != -1 && num < itemSlots.Length)
		{
			Texture texture = itemTextures[item_ID];
			if (texture != null)
			{
				Vector3 position = itemSlots[num].position;
				itemPickupEffect.StartEffect(texture, position);
			}
		}
	}

	private void UseItem()
	{
		if (item[itemSelected] == 0)
		{
			return;
		}
		if (item[itemSelected] == 1)
		{
			player.stamina = player.maxStamina * 2f;
			ResetItem();
		}
		else if (item[itemSelected] == 2)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), out var hitInfo) && ((hitInfo.collider.tag == "SwingingDoor") & (Vector3.Distance(playerTransform.position, hitInfo.transform.position) <= 10f)))
			{
				hitInfo.collider.gameObject.GetComponent<SwingingDoorScript>().LockDoor(15f);
				ResetItem();
			}
		}
		else if (item[itemSelected] == 3)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), out var hitInfo2) && ((hitInfo2.collider.tag == "Door") & (Vector3.Distance(playerTransform.position, hitInfo2.transform.position) <= 10f)))
			{
				DoorScript component = hitInfo2.collider.gameObject.GetComponent<DoorScript>();
				if (component.DoorLocked)
				{
					component.UnlockDoor();
					component.OpenDoor();
					ResetItem();
				}
			}
		}
		else if (item[itemSelected] == 4)
		{
			Object.Instantiate(bsodaSpray, playerTransform.position, cameraTransform.rotation);
			ResetItem();
			player.ResetGuilt("drink", 1f);
			audioDevice.PlayOneShot(aud_Soda);
		}
		else if (item[itemSelected] == 5)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), out var hitInfo3))
			{
				if ((hitInfo3.collider.name == "BSODAMachine") & (Vector3.Distance(playerTransform.position, hitInfo3.transform.position) <= 10f))
				{
					ResetItem();
					CollectItem(4);
				}
				else if ((hitInfo3.collider.name == "ZestyMachine") & (Vector3.Distance(playerTransform.position, hitInfo3.transform.position) <= 10f))
				{
					ResetItem();
					CollectItem(1);
				}
				else if ((hitInfo3.collider.name == "PayPhone") & (Vector3.Distance(playerTransform.position, hitInfo3.transform.position) <= 10f))
				{
					hitInfo3.collider.gameObject.GetComponent<TapePlayerScript>().Play();
					ResetItem();
				}
			}
		}
		else if (item[itemSelected] == 6)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), out var hitInfo4) && ((hitInfo4.collider.name == "TapePlayer") & (Vector3.Distance(playerTransform.position, hitInfo4.transform.position) <= 10f)))
			{
				hitInfo4.collider.gameObject.GetComponent<TapePlayerScript>().Play();
				ResetItem();
			}
		}
		else if (item[itemSelected] == 7)
		{
			Object.Instantiate(alarmClock, playerTransform.position, cameraTransform.rotation).GetComponent<AlarmClockScript>().baldi = baldiScrpt;
			ResetItem();
		}
		else if (item[itemSelected] == 8)
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), out var hitInfo5) && ((hitInfo5.collider.tag == "Door") & (Vector3.Distance(playerTransform.position, hitInfo5.transform.position) <= 10f)))
			{
				hitInfo5.collider.gameObject.GetComponent<DoorScript>().SilenceDoor();
				ResetItem();
				audioDevice.PlayOneShot(aud_Spray);
			}
		}
		else if (item[itemSelected] == 9)
		{
			Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
			RaycastHit hitInfo6;
			if (player.jumpRope)
			{
				player.DeactivateJumpRope();
				playtimeScript.Disappoint();
				ResetItem();
			}
			else if (Physics.Raycast(ray, out hitInfo6) && hitInfo6.collider.name == "1st Prize")
			{
				firstPrizeScript.GoCrazy();
				ResetItem();
			}
		}
		else if (item[itemSelected] == 10)
		{
			player.ActivateBoots();
			StartCoroutine(BootAnimation());
			ResetItem();
		}
		else if (item[itemSelected] == 11)
		{
			StartCoroutine(Teleporter());
			ResetItem();
		}
		else if (item[itemSelected] == 12)
		{
			StartCoroutine(OyuncuHizlandirma());
			ResetItem();
		}
	}

	private IEnumerator OyuncuHizlandirma()
	{
		player.walkSpeed *= 2f;
		player.runSpeed *= 2f;
		yield return new WaitForSeconds(10f);
		player.walkSpeed /= 2f;
		player.runSpeed /= 2f;
	}

	private IEnumerator BootAnimation()
	{
		float time = 15f;
		float height = 375f;
		boots.gameObject.SetActive(value: true);
		Vector3 localPosition;
		while (height > -375f)
		{
			height -= 375f * Time.deltaTime;
			time -= Time.deltaTime;
			localPosition = boots.localPosition;
			localPosition.y = height;
			boots.localPosition = localPosition;
			yield return null;
		}
		localPosition = boots.localPosition;
		localPosition.y = -375f;
		boots.localPosition = localPosition;
		boots.gameObject.SetActive(value: false);
		while (time > 0f)
		{
			time -= Time.deltaTime;
			yield return null;
		}
		boots.gameObject.SetActive(value: true);
		while (height < 375f)
		{
			height += 375f * Time.deltaTime;
			localPosition = boots.localPosition;
			localPosition.y = height;
			boots.localPosition = localPosition;
			yield return null;
		}
		localPosition = boots.localPosition;
		localPosition.y = 375f;
		boots.localPosition = localPosition;
		boots.gameObject.SetActive(value: false);
	}

	private IEnumerator Teleporter()
	{
		playerCharacter.enabled = false;
		playerCollider.enabled = false;
		int teleports = Random.Range(12, 16);
		int teleportCount = 0;
		float baseTime = 0.2f;
		float currentTime = baseTime;
		float increaseFactor = 1.1f;
		while (teleportCount < teleports)
		{
			currentTime -= Time.deltaTime;
			if (currentTime < 0f)
			{
				Teleport();
				teleportCount++;
				baseTime *= increaseFactor;
				currentTime = baseTime;
			}
			if (flipped)
			{
				player.height = 6f;
			}
			else
			{
				player.height = 4f;
			}
			yield return null;
		}
		playerCharacter.enabled = true;
		playerCollider.enabled = true;
	}

	private void Teleport()
	{
		AILocationSelector.GetNewTarget();
		player.transform.position = AILocationSelector.transform.position + Vector3.up * player.height;
		audioDevice.PlayOneShot(aud_Teleport);
	}

	private void ResetItem()
	{
		item[itemSelected] = 0;
		itemSlot[itemSelected].texture = itemTextures[0];
		UpdateItemName();
	}

	public void LoseItem(int id)
	{
		item[id] = 0;
		itemSlot[id].texture = itemTextures[0];
		UpdateItemName();
	}

	private void UpdateItemName()
	{
		itemText.text = itemNames[item[itemSelected]];
	}

	public void ExitReached()
	{
		exitsReached++;
		RenderSettings.ambientLight = Color.green;
		audioDevice.PlayOneShot(aud_Switch, 0.8f);
	}

	public void Fliparoo()
	{
		flipped = true;
		player.height = 6f;
		player.fliparoo = 180f;
		player.flipaturn = -1f;
		Camera.main.GetComponent<CameraScript>().offset = new Vector3(0f, -1f, 0f);
	}
}
