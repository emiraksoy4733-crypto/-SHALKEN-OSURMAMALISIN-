using UnityEngine;
using UnityEngine.UI;

public class LipSync : MonoBehaviour
{
	public bool useSprites;

	public bool altSprites;

	public float volumeThreshold = 0.1f;

	public string animName;

	public Sprite[] syncedSprites;

	public Sprite[] syncedAltSprites;

	public AudioSource audioDevice;

	private SpriteRenderer spriteRenderer;

	private Image image;

	private Animator animator;

	private void Start()
	{
		if (useSprites)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
			image = GetComponent<Image>();
		}
		else
		{
			animator = GetComponent<Animator>();
		}
	}

	private void Update()
	{
		if (!audioDevice.isPlaying)
		{
			return;
		}
		int num = 0;
		float num2 = 0f;
		float volumeLevel = GetVolumeLevel();
		if (useSprites)
		{
			num = Mathf.FloorToInt(volumeLevel * (float)(syncedSprites.Length - 1));
			if (volumeLevel < volumeThreshold)
			{
				num = 0;
			}
			num2 += Time.deltaTime;
			if (num2 >= 0.1f)
			{
				num = (num + 1) % syncedSprites.Length;
				num2 = 0f;
			}
			if (spriteRenderer != null)
			{
				if (!altSprites)
				{
					spriteRenderer.sprite = syncedSprites[num];
				}
				else
				{
					spriteRenderer.sprite = syncedAltSprites[num];
				}
			}
			if (image != null)
			{
				if (!altSprites)
				{
					image.sprite = syncedSprites[num];
				}
				else
				{
					image.sprite = syncedAltSprites[num];
				}
			}
		}
		else
		{
			float value = volumeLevel;
			if (volumeLevel < volumeThreshold)
			{
				value = 1f - volumeLevel;
			}
			value = Mathf.Clamp01(value);
			animator.Play(animName, -1, value);
		}
	}

	private float GetVolumeLevel()
	{
		float[] array = new float[256];
		audioDevice.GetOutputData(array, 0);
		float num = 0f;
		for (int i = 0; i < array.Length; i++)
		{
			num += Mathf.Abs(array[i]);
		}
		return Mathf.Clamp01(num / (float)array.Length / volumeThreshold);
	}
}
