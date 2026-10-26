using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AfterImageEffect : MonoBehaviour
{
	private SpriteRenderer sr;

	private Color startColor;

	private float alpha = 1f;

	private float fadeSpeed;

	public void Init(Sprite targetSprite, Color ghostColor, float duration, Vector3 scale, Quaternion rotation)
	{
		sr = GetComponent<SpriteRenderer>();
		sr.sprite = targetSprite;
		base.transform.localScale = scale;
		base.transform.rotation = rotation;
		startColor = ghostColor;
		sr.color = startColor;
		fadeSpeed = 1f / duration;
	}

	private void Update()
	{
		alpha -= fadeSpeed * Time.deltaTime;
		sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
		if (alpha <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
