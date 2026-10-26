using UnityEngine;

public class PulsingScript : MonoBehaviour
{
	public Camera playerCamera;

	public BaldiScript baldi;

	public bool isPulsing;

	[SerializeField]
	private float maxCooldown = 0.54f;

	private float cooldown;

	public float targetFOV = 120f;

	public float defaultFOV = 120f;

	public float pulseFOV = 110f;

	public ZoomBlurEffect zoomBlur;

	private void Start()
	{
		if (playerCamera != null)
		{
			playerCamera.fieldOfView = defaultFOV;
		}
		cooldown = maxCooldown;
	}

	private void Update()
	{
		if (isPulsing)
		{
			zoomBlur.enabled = true;
		}
		else
		{
			zoomBlur.enabled = false;
		}
		if (baldi == null || playerCamera == null)
		{
			return;
		}
		if (baldi.canChase)
		{
			isPulsing = true;
		}
		else
		{
			isPulsing = false;
			targetFOV = defaultFOV;
		}
		if (cooldown > 0f)
		{
			cooldown -= Time.deltaTime;
		}
		if (isPulsing && cooldown <= 0f)
		{
			playerCamera.fieldOfView = pulseFOV;
			cooldown = maxCooldown;
		}
		if (playerCamera.fieldOfView < defaultFOV)
		{
			playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, defaultFOV, 10f * Time.deltaTime);
			if (defaultFOV - playerCamera.fieldOfView < 0.05f)
			{
				playerCamera.fieldOfView = defaultFOV;
			}
		}
	}
}
