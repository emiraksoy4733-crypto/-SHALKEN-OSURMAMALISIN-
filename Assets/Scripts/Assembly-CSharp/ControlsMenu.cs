using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ControlFreak2;

public class ControlsMenu : MonoBehaviour
{
	[Header("UI Elements")]
	[SerializeField]
	private Button[] allButtons;

	[SerializeField]
	private TMP_Text[] buttonTexts;

	[SerializeField]
	private TMP_Text[] secondaryButtonTexts;

	[SerializeField]
	private TMP_Text[] labelTexts;

	[Header("UI Panels")]
	[SerializeField]
	private GameObject panel;

	[SerializeField]
	private GameObject confirmationPanel;

	[SerializeField]
	private GameObject resetConfirmationPanel;

	[Header("Counter Text")]
	[SerializeField]
	private TMP_Text counterText;

	[SerializeField]
	private TMP_Text confirmText;

	private int currentActionIndex;

	private Coroutine rebindCoroutine;

	private KeyCode[] allKeyCodes;

	private bool isBindingMouseInput;

	private InputManager inputMan;

	private void Awake()
	{
		allKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));
	}

	private void Start()
	{
		inputMan = Singleton<InputManager>.Instance;
		if (inputMan != null)
		{
			UpdateAllButtonTexts();
		}
	}

	private void OnEnable()
	{
		if (inputMan == null)
		{
			inputMan = Singleton<InputManager>.Instance;
		}
		if (inputMan == null)
		{
			Debug.LogError("ControlsMenu: InputManager not found! Is it in the scene?");
			return;
		}
		Debug.Log($"ControlsMenu: InputManager found. Mappings count: {inputMan.KeyboardMapping.Count}");
		UpdateAllButtonTexts();
	}

	private void OnDisable()
	{
		inputMan.Save();
	}

	public void UpdateAllButtonTexts()
	{
		for (int i = 0; i < labelTexts.Length; i++)
		{
			if (i < Enum.GetValues(typeof(InputAction)).Length)
			{
				InputAction key = (InputAction)i;
				if (inputMan.KeyboardMapping.TryGetValue(key, out var value))
				{
					buttonTexts[i].text = (IsMouseKey(value.primaryKey) ? FormatMouseText(value.primaryKey) : FormatKeyText(value.primaryKey));
					secondaryButtonTexts[i].text = (IsMouseKey(value.secondaryKey) ? FormatMouseText(value.secondaryKey) : FormatKeyText(value.secondaryKey));
				}
			}
		}
	}

	private string FormatKeyText(KeyCode key)
	{
		return key switch
		{
			KeyCode.Escape => "ESC", 
			KeyCode.Alpha0 => "0", 
			KeyCode.Alpha1 => "1", 
			KeyCode.Alpha2 => "2", 
			KeyCode.Alpha3 => "3", 
			KeyCode.Alpha4 => "4", 
			KeyCode.Alpha5 => "5", 
			KeyCode.Alpha6 => "6", 
			KeyCode.Alpha7 => "7", 
			KeyCode.Alpha8 => "8", 
			KeyCode.Alpha9 => "9", 
			KeyCode.None => "", 
			_ => key.ToString(), 
		};
	}

	private string FormatMouseText(KeyCode mouseKey)
	{
		return mouseKey switch
		{
			KeyCode.Mouse0 => "Left Mouse Button", 
			KeyCode.Mouse1 => "Right Mouse Button", 
			KeyCode.Mouse2 => "Middle Mouse Button", 
			_ => mouseKey.ToString(), 
		};
	}

	public void StartRebindKeyboard(int actionIndex)
	{
		if (!panel.activeSelf && !confirmationPanel.activeSelf && actionIndex < Enum.GetValues(typeof(InputAction)).Length)
		{
			currentActionIndex = actionIndex;
			isBindingMouseInput = false;
			confirmationPanel.SetActive(value: true);
			SetButtonsInteractable(state: false);
			confirmText.text = labelTexts[currentActionIndex].text + "\n\n" + buttonTexts[currentActionIndex].text;
		}
	}

	public void StartRebindMouse(int actionIndex)
	{
		if (!panel.activeSelf && !confirmationPanel.activeSelf && actionIndex < Enum.GetValues(typeof(InputAction)).Length)
		{
			currentActionIndex = actionIndex;
			isBindingMouseInput = true;
			confirmationPanel.SetActive(value: true);
			SetButtonsInteractable(state: false);
			confirmText.text = labelTexts[currentActionIndex].text + "\n\n" + secondaryButtonTexts[currentActionIndex].text;
		}
	}

	public void ShowResetConfirmation()
	{
		resetConfirmationPanel.SetActive(value: true);
		SetButtonsInteractable(state: false);
	}

	public void ResetKey()
	{
		if (!isBindingMouseInput)
		{
			inputMan.ClearPrimaryKey((InputAction)currentActionIndex);
		}
		else
		{
			inputMan.ClearSecondaryKey((InputAction)currentActionIndex);
		}
		inputMan.Save();
		UpdateAllButtonTexts();
		CancelRebind();
	}

	public void ConfirmRebind()
	{
		confirmationPanel.SetActive(value: false);
		panel.SetActive(value: true);
		if (rebindCoroutine != null)
		{
			StopCoroutine(rebindCoroutine);
		}
		rebindCoroutine = StartCoroutine(RebindCoroutine(5f));
	}

	public void ConfirmReset()
	{
		resetConfirmationPanel.SetActive(value: false);
		ResetToDefaults();
		SetButtonsInteractable(state: true);
	}

	public void CancelRebind()
	{
		confirmationPanel.SetActive(value: false);
		if (rebindCoroutine != null)
		{
			StopCoroutine(rebindCoroutine);
		}
		SetButtonsInteractable(state: true);
	}

	public void CancelReset()
	{
		resetConfirmationPanel.SetActive(value: false);
		SetButtonsInteractable(state: true);
	}

	private IEnumerator RebindCoroutine(float duration)
	{
		string actionName = labelTexts[currentActionIndex].text;
		float remainingTime = duration;
		counterText.text = $"<size=16>{actionName}<size=21>\nPress a key to assign it to {actionName}\n\n\n\n\n\n{Mathf.CeilToInt(remainingTime)}";
		while (CF2Input.anyKey)
		{
			yield return null;
		}
		yield return null;
		while (remainingTime > 0f)
		{
			counterText.text = $"<size=16>{actionName}<size=21>\nPress a key to assign it to {actionName}\n\n\n\n\n\n{Mathf.CeilToInt(remainingTime)}";
			if (GetPressedInput(out var pressedKey))
			{
				if (isBindingMouseInput && IsMouseKey(pressedKey))
				{
					inputMan.Modify((InputAction)currentActionIndex, KeyCode.None, pressedKey);
				}
				else if (!isBindingMouseInput && !IsMouseKey(pressedKey))
				{
					inputMan.Modify((InputAction)currentActionIndex, pressedKey);
				}
				remainingTime = 0.01f;
			}
			remainingTime -= Time.unscaledDeltaTime;
			yield return null;
		}
		panel.SetActive(value: false);
		rebindCoroutine = null;
		isBindingMouseInput = false;
		SetButtonsInteractable(state: true);
		inputMan.Save();
		UpdateAllButtonTexts();
	}

	private bool GetPressedInput(out KeyCode pressedKey)
	{
		pressedKey = KeyCode.None;
		if (isBindingMouseInput)
		{
			for (int i = 0; i < 7; i++)
			{
				if (CF2Input.GetMouseButtonDown(i))
				{
					pressedKey = (KeyCode)(323 + i);
					return true;
				}
			}
		}
		else if (CF2Input.anyKeyDown)
		{
			KeyCode[] array = allKeyCodes;
			foreach (KeyCode keyCode in array)
			{
				if (CF2Input.GetKeyDown(keyCode) && !IsMouseKey(keyCode))
				{
					pressedKey = keyCode;
					return true;
				}
			}
		}
		return false;
	}

	private bool IsMouseKey(KeyCode key)
	{
		if (key != KeyCode.Mouse0 && key != KeyCode.Mouse1 && key != KeyCode.Mouse2 && key != KeyCode.Mouse3)
		{
			return key == KeyCode.Mouse4;
		}
		return true;
	}

	private void SetButtonsInteractable(bool state)
	{
		Button[] array = allButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].interactable = state;
		}
	}

	public void ResetToDefaults()
	{
		SetButtonsInteractable(state: true);
		inputMan.SetDefaults();
		UpdateAllButtonTexts();
	}
}
