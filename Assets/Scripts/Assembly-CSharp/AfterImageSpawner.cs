using UnityEngine;

public class AfterImageSpawner : MonoBehaviour
{
	[Header("Dependencies")]
	[SerializeField]
	private SpriteRenderer targetSpriteRenderer;

	[Header("Color Settings")]
	public Color afterImageColor = new Color(0f, 1f, 0.2f, 0.6f);

	[Header("Spawn Settings")]
	public float timeBetweenSpawns = 0.1f;

	public float ghostDuration = 0.5f;

	[Tooltip("Спавнить только если персонаж движется?")]
	public bool spawnOnlyWhenMoving = true;

	private float spawnTimer;

	private Vector3 lastPosition;

	private void Start()
	{
		if (targetSpriteRenderer == null)
		{
			targetSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
		}
		lastPosition = base.transform.position;
	}

	private void Update()
	{
		if (targetSpriteRenderer == null || targetSpriteRenderer.sprite == null)
		{
			return;
		}
		bool flag = Vector3.Distance(base.transform.position, lastPosition) > 0.01f;
		lastPosition = base.transform.position;
		if (!spawnOnlyWhenMoving || flag)
		{
			spawnTimer += Time.deltaTime;
			if (spawnTimer >= timeBetweenSpawns)
			{
				spawnTimer = 0f;
				CreateAfterImage();
			}
		}
	}

	private void CreateAfterImage()
	{
		GameObject obj = new GameObject("Playtime_AfterImage");
		obj.transform.position = base.transform.position;
		SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
		spriteRenderer.sortingLayerID = targetSpriteRenderer.sortingLayerID;
		spriteRenderer.sortingOrder = targetSpriteRenderer.sortingOrder - 1;
		spriteRenderer.material = targetSpriteRenderer.material;
		obj.AddComponent<AfterImageEffect>().Init(targetSpriteRenderer.sprite, afterImageColor, ghostDuration, base.transform.lossyScale, targetSpriteRenderer.transform.rotation);
	}
}
