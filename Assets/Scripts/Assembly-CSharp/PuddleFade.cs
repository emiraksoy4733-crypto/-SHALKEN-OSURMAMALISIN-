using UnityEngine;

public class PuddleFade : MonoBehaviour
{
	private SpriteRenderer sr;

	private float alpha = 1f;

	public float lifeTime = 5f;

	private void Start()
	{
		sr = GetComponent<SpriteRenderer>();
		Object.Destroy(base.gameObject, lifeTime + 2f);
	}

	private void Update()
	{
		if (lifeTime > 0f)
		{
			lifeTime -= Time.deltaTime;
		}
		else if (sr != null)
		{
			alpha -= Time.deltaTime * 0.5f;
			sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
		}
	}
}
