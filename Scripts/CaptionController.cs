using TMPro;
using UnityEngine;

public class CaptionController : MonoBehaviour
{

	public TextMeshProUGUI textMesh;

	public void Show (string text)
	{
		textMesh.SetText (text);
	}
}
