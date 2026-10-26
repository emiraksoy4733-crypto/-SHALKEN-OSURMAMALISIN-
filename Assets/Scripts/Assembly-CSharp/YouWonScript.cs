using UnityEngine;
using UnityEngine.SceneManagement;

public class YouWonScript : MonoBehaviour
{
	private float delay;

	private void Start()
	{
		delay = 36f;
	}

	private void Update()
	{
		delay -= Time.deltaTime;
		if (delay <= 0f)
		{
			SceneManager.LoadScene("MainMenu");
		}
	}
}
