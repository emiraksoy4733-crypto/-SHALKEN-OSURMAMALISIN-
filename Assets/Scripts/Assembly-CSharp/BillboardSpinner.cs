using UnityEngine;

public class BillboardSpinner : MonoBehaviour
{
	[Header("Rotation Settings")]
	[Tooltip("Скорость вращения предмета (градусы в секунду)")]
	public float spinSpeed = 120f;

	[Tooltip("Плавное покачивание вверх-вниз (эффект левитации в воздухе)")]
	public bool floatUpDown = true;

	public float floatAmplitude = 0.15f;

	public float floatFrequency = 2f;

	private Vector3 startPosition;

	private void Start()
	{
		startPosition = base.transform.position;
	}

	private void Update()
	{
		base.transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
		if (floatUpDown)
		{
			float y = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
			base.transform.position = new Vector3(base.transform.position.x, y, base.transform.position.z);
		}
	}
}
