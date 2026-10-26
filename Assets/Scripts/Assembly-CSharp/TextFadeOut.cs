using System.Collections;
using TMPro;
using UnityEngine;

public class TextFadeOut : MonoBehaviour
{
	public TMP_Text text;

	public float fadeDuration = 2f;

	private Color originalColor;

	private void Start()
	{
		if (text == null)
		{
			text = GetComponent<TMP_Text>();
		}
		originalColor = text.color;
		text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
	}

	public IEnumerator FadeOut()
	{
		text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
		float elapsed = 0f;
		while (elapsed < fadeDuration)
		{
			elapsed += Time.deltaTime;
			float a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
			text.color = new Color(originalColor.r, originalColor.g, originalColor.b, a);
			yield return null;
		}
		text.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
	}
}
