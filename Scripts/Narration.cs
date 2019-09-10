using UnityEngine;

[CreateAssetMenu (fileName = "New Narration Asset", menuName = "Narration Asset")]
public class Narration : ScriptableObject
{

	public AudioClip audio;
	public TranscriptLine[] transcript;

}

[System.Serializable]
public struct TranscriptLine
{
	public string text;
	public float startTime;
	public float endTime;
}