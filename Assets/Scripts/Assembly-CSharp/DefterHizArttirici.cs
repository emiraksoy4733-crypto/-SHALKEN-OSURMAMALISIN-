using UnityEngine;

public class DefterHizArttirici : MonoBehaviour
{
	public PlayerScript ps;

	public BaldiScript baldi;

	public bool arttirabilirmi;

	public void HizArttir()
	{
		if (arttirabilirmi)
		{
			ps.walkSpeed *= 1.09f;
			ps.runSpeed *= 1.095f;
			baldi.speed *= 1.1f;
		}
	}
}
