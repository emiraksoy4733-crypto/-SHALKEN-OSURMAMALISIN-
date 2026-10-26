using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ZoomBlurEffect : MonoBehaviour
{
	public Material blurMaterial;

	public PulsingScript pulse;

	[Range(0f, 0.1f)]
	public float blurStrength = 0.02f;

	[Range(2f, 30f)]
	public int samples = 10;

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		blurMaterial.SetFloat("_BlurStrength", blurStrength);
		blurMaterial.SetInt("_Samples", samples);
		Graphics.Blit(src, dest, blurMaterial);
	}
}
