using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class AddSettings : MonoBehaviour
{
	[Range(0f, 1f)]
	public float volumeVariance = .1f;

	[Range(0f, 1f)]
	public float pitchVariance = .1f;

}
