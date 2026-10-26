using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageHoverGlow : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Image image;

	[Header("Color")]
	public Color normalColor = Color.white;

	public Color hoverColor = new Color(1f, 0.35f, 0f);

	[Header("Zoom")]
	public float hoverScale = 1.05f;

	public float scaleSpeed = 8f;

	[Header("Color Speed")]
	public float fadeSpeed = 8f;

	private bool hovering;

	private Vector3 normalScale;

	private void Awake()
	{
		if (image == null)
		{
			image = GetComponent<Image>();
		}
		normalScale = base.transform.localScale;
		ResetButton();
	}

	private void OnEnable()
	{
		ResetButton();
	}

	private void ResetButton()
	{
		hovering = false;
		if (image != null)
		{
			image.color = normalColor;
		}
		base.transform.localScale = normalScale;
	}

	private void Update()
	{
		Color b = (hovering ? hoverColor : normalColor);
		image.color = Color.Lerp(image.color, b, Time.deltaTime * fadeSpeed);
		Vector3 b2 = (hovering ? (normalScale * hoverScale) : normalScale);
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, b2, Time.deltaTime * scaleSpeed);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		hovering = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hovering = false;
	}
}
