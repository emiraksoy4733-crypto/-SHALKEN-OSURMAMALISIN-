using System.Collections.Generic;
using UnityEngine;
using ControlFreak2;

public class InputManager : Singleton<InputManager>
{
	public Dictionary<InputAction, InputBinding> KeyboardMapping = new Dictionary<InputAction, InputBinding>();

	private Dictionary<KeyCode, SimulatedKeyState> simulatedKeys = new Dictionary<KeyCode, SimulatedKeyState>();

	private bool[] currentKeyStates = new bool[15];

	private bool[] previousKeyStates = new bool[15];

	private const string SavePrefix = "InputBinding_";

	public Dictionary<InputAction, InputBinding> Mappings
	{
		get
		{
			if (KeyboardMapping == null || KeyboardMapping.Count == 0)
			{
				Load();
			}
			return KeyboardMapping;
		}
	}

	protected override void Awake()
	{
		if (Singleton<InputManager>.Instance != null && Singleton<InputManager>.Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		base.Awake();
		Object.DontDestroyOnLoad(base.gameObject);
		Load();
	}

	private void Update()
	{
		bool[] array = previousKeyStates;
		previousKeyStates = currentKeyStates;
		currentKeyStates = array;
		UpdateSimulatedKeys();
		for (int i = 0; i < 15; i++)
		{
			if (KeyboardMapping.TryGetValue((InputAction)i, out var value))
			{
				bool flag = (value.primaryKey != KeyCode.None && GetKeyState(value.primaryKey)) || (value.secondaryKey != KeyCode.None && GetKeyState(value.secondaryKey));
				currentKeyStates[i] = flag;
			}
		}
	}

	private void UpdateSimulatedKeys()
	{
		if (simulatedKeys.Count == 0)
		{
			return;
		}
		foreach (KeyCode item in new List<KeyCode>(simulatedKeys.Keys))
		{
			SimulatedKeyState value = simulatedKeys[item];
			value.previousFrame = value.currentFrame;
			value.currentFrame = false;
			simulatedKeys[item] = value;
			if (!value.currentFrame && !value.previousFrame)
			{
				simulatedKeys.Remove(item);
			}
		}
	}

	public void SimulateKey(KeyCode key)
	{
		if (!simulatedKeys.TryGetValue(key, out var value))
		{
			value = default;
		}
		value.currentFrame = true;
		simulatedKeys[key] = value;
	}

	private bool GetKeyState(KeyCode key)
	{
		if (simulatedKeys.TryGetValue(key, out var value) && value.currentFrame)
		{
			return true;
		}
		// Control Freak 2 "Interaction" button is bound to the Fire1 axis.
		// The original game maps Interact to Mouse0, so bridge it here.
		if (key == KeyCode.Mouse0 && CF2Input.GetButton("Fire1"))
		{
			return true;
		}
		return CF2Input.GetKey(key);
	}

	public bool GetActionKey(InputAction action)
	{
		return currentKeyStates[(int)action];
	}

	public bool GetActionKeyDown(InputAction action)
	{
		if (currentKeyStates[(int)action])
		{
			return !previousKeyStates[(int)action];
		}
		return false;
	}

	public bool GetActionKeyUp(InputAction action)
	{
		if (!currentKeyStates[(int)action])
		{
			return previousKeyStates[(int)action];
		}
		return false;
	}

	public void ClearPrimaryKey(InputAction action)
	{
		if (KeyboardMapping.TryGetValue(action, out var value))
		{
			value.primaryKey = KeyCode.None;
			KeyboardMapping[action] = value;
		}
	}

	public void ClearSecondaryKey(InputAction action)
	{
		if (KeyboardMapping.TryGetValue(action, out var value))
		{
			value.secondaryKey = KeyCode.None;
			KeyboardMapping[action] = value;
		}
	}

	public string ConvertInputActionToString(InputAction action)
	{
		if (!KeyboardMapping.TryGetValue(action, out var value))
		{
			return action.ToString();
		}
		if (value.primaryKey != KeyCode.None)
		{
			return KeyCodeToDisplayString(value.primaryKey);
		}
		if (value.secondaryKey != KeyCode.None)
		{
			return KeyCodeToDisplayString(value.secondaryKey);
		}
		return action.ToString();
	}

	public void Modify(InputAction action, KeyCode newKey, KeyCode secondaryKey = KeyCode.None)
	{
		if (!KeyboardMapping.TryGetValue(action, out var value))
		{
			value = default;
		}
		if (newKey != KeyCode.None)
		{
			value.primaryKey = newKey;
		}
		if (secondaryKey != KeyCode.None)
		{
			value.secondaryKey = secondaryKey;
		}
		KeyboardMapping[action] = value;
	}

	public void Save()
	{
		foreach (KeyValuePair<InputAction, InputBinding> item in KeyboardMapping)
		{
			string key = "InputBinding_" + item.Key;
			string value = $"{(int)item.Value.primaryKey},{(int)item.Value.secondaryKey}";
			PlayerPrefs.SetString(key, value);
		}
		PlayerPrefs.Save();
	}

	public void Load()
	{
		KeyboardMapping.Clear();
		for (int i = 0; i < 15; i++)
		{
			InputAction inputAction = (InputAction)i;
			string key = "InputBinding_" + inputAction;
			if (PlayerPrefs.HasKey(key))
			{
				string[] array = PlayerPrefs.GetString(key).Split(',');
				if (array.Length == 2 && int.TryParse(array[0], out var result) && int.TryParse(array[1], out var result2))
				{
					KeyboardMapping[inputAction] = new InputBinding((KeyCode)result, (KeyCode)result2);
					continue;
				}
			}
			KeyboardMapping[inputAction] = GetDefaultBinding(inputAction);
		}
	}

	public void SetDefaults()
	{
		KeyboardMapping.Clear();
		for (int i = 0; i < 15; i++)
		{
			InputAction inputAction = (InputAction)i;
			KeyboardMapping[inputAction] = GetDefaultBinding(inputAction);
		}
		Save();
	}

	public string KeyCodeToDisplayString(KeyCode key)
	{
		return key switch
		{
			KeyCode.Escape => "ESC", 
			KeyCode.Mouse0 => "Left Mouse Button", 
			KeyCode.Mouse1 => "Right Mouse Button", 
			KeyCode.Mouse2 => "Middle Mouse Button", 
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

	private InputBinding GetDefaultBinding(InputAction action)
	{
		return action switch
		{
			InputAction.MoveLeft => new InputBinding(KeyCode.A), 
			InputAction.MoveRight => new InputBinding(KeyCode.D), 
			InputAction.MoveForward => new InputBinding(KeyCode.W), 
			InputAction.MoveBackward => new InputBinding(KeyCode.S), 
			InputAction.Interact => new InputBinding(KeyCode.None, KeyCode.Mouse0), 
			InputAction.UseItem => new InputBinding(KeyCode.None, KeyCode.Mouse1), 
			InputAction.Slot0 => new InputBinding(KeyCode.Alpha1), 
			InputAction.Slot1 => new InputBinding(KeyCode.Alpha2), 
			InputAction.Slot2 => new InputBinding(KeyCode.Alpha3), 
			InputAction.Run => new InputBinding(KeyCode.LeftShift), 
			InputAction.LookBehind => new InputBinding(KeyCode.Space), 
			InputAction.Jump => new InputBinding(KeyCode.Space), 
			InputAction.PauseOrCancel => new InputBinding(KeyCode.Escape), 
			_ => new InputBinding(KeyCode.None), 
		};
	}
}
