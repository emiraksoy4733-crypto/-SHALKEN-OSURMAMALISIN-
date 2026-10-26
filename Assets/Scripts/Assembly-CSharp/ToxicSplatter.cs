using System.Collections.Generic;
using UnityEngine;

public class ToxicSplatter : MonoBehaviour
{
	private ParticleSystem partSystem;

	private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

	[Header("Prefabs & Audio")]
	[Tooltip("Префаб 2D-спрайта лужи ToxicPuddle")]
	public GameObject splatterPrefab;

	[Tooltip("Звуковой эффект удара жижи о стену (например, splat.wav)")]
	public AudioClip splatSound;

	[Tooltip("Источник звука (AudioSource) на префабе БСОДЫ")]
	public AudioSource audioSource;

	[Header("Performance Settings")]
	[Tooltip("Каждая какая по счету капля создает лужу (чтобы не спамить объекты на стенах)")]
	[Range(1f, 10f)]
	public int spawnRateDivider = 4;

	private void Start()
	{
		partSystem = GetComponent<ParticleSystem>();
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
		}
		if (audioSource == null)
		{
			audioSource = GetComponentInParent<AudioSource>();
		}
	}

	private void OnParticleCollision(GameObject other)
	{
		if (other.CompareTag("Player") || other.name.Contains("Player") || splatterPrefab == null || partSystem == null)
		{
			return;
		}
		int num = partSystem.GetCollisionEvents(other, collisionEvents);
		bool flag = false;
		for (int i = 0; i < num; i++)
		{
			if (collisionEvents[i].intersection == Vector3.zero)
			{
				continue;
			}
			if (!flag && audioSource != null && splatSound != null)
			{
				audioSource.pitch = Random.Range(0.85f, 1.25f);
				audioSource.PlayOneShot(splatSound);
				flag = true;
			}
			if (Random.Range(0, spawnRateDivider) == 0)
			{
				Vector3 intersection = collisionEvents[i].intersection;
				Vector3 vector = collisionEvents[i].normal;
				if (vector.sqrMagnitude < 0.01f)
				{
					vector = Vector3.up;
				}
				Quaternion rotation = Quaternion.LookRotation(vector);
				GameObject obj = Object.Instantiate(splatterPrefab, intersection + vector * 0.04f, rotation);
				float num2 = 4.5f * Random.Range(0.8f, 1.3f);
				obj.transform.localScale = new Vector3(num2, num2, 1f);
				obj.transform.Rotate(Vector3.forward, Random.Range(0f, 360f));
			}
		}
	}
}
