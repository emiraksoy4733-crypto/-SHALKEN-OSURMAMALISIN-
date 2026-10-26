using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemEffect : MonoBehaviour
{
	private RawImage rawImage;

	private RectTransform rectTransform;

	[Header("Animation Settings")]
	[SerializeField]
	private float duration = 1f;

	[SerializeField]
	private float moveSpeed = 60f;

	[Header("Position Adjustment")]
	[Tooltip("Adjust X and Y offsets in the Inspector to perfectly match your layout manually.")]
	[SerializeField]
	private Vector3 visualOffset = new Vector3(0f, 0f, 0f);

	private Color greenColor = new Color(0.2f, 1f, 0.2f, 1f);

	private void EnsureComponents()
	{
		if (rawImage == null)
		{
			rawImage = GetComponent<RawImage>();
		}
		if (rectTransform == null)
		{
			rectTransform = GetComponent<RectTransform>();
		}
	}

	private void Awake()
	{
		EnsureComponents();
	}

	public void StartEffect(Texture itemTexture, Vector3 slotPosition)
	{
		EnsureComponents();
		if (!(rawImage == null) && !(rectTransform == null))
		{
			StopAllCoroutines();
			base.gameObject.SetActive(value: true);
			rawImage.texture = itemTexture;
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.position = slotPosition + visualOffset;
			rawImage.color = greenColor;
			StartCoroutine(AnimatePickup());
		}
	}

	private IEnumerator AnimatePickup()
	{
		float elapsed = 0f;
		Color startColor = greenColor;
		Color endColor = new Color(greenColor.r, greenColor.g, greenColor.b, 0f);
		while (elapsed < duration)
		{
			if (rawImage == null || rectTransform == null)
			{
				yield break;
			}
			elapsed += Time.deltaTime;
			float t = elapsed / duration;
			rectTransform.anchoredPosition += Vector2.up * (moveSpeed * Time.deltaTime);
			rawImage.color = Color.Lerp(startColor, endColor, t);
			yield return null;
		}
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(value: false);
		}
	}
}
