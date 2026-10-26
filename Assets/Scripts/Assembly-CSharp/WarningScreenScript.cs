using UnityEngine;
using ControlFreak2;
using UnityEngine.SceneManagement;

public class WarningScreenScript : MonoBehaviour
{
	public string LoadScene;

	private void Update()
	{
		if (CF2Input.anyKeyDown || Input.touchCount > 0 || Input.GetMouseButtonDown(0))
		{
			SceneManager.LoadScene(LoadScene);
		}
	}
}
