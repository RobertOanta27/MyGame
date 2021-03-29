using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;

	public AudioMixerGroup mixerGroup;

	[SerializeField] private GameObject[] listSounds;



    void Awake()
	{
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

  //      foreach (Sound s in sounds)
		//{
		//	s.source = gameObject.AddComponent<AudioSource>();
		//	s.source.clip = s.clip;
		//	s.source.loop = s.loop;

		//	s.source.outputAudioMixerGroup = mixerGroup;
		//}
	}

    public void Start()
    {
        
    }

    IEnumerator objDestroy(float seconds,GameObject soundObj)
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp DestorySound : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(seconds);
        Destroy(soundObj);



        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp DestorySound : " + Time.time);
    }

    IEnumerator objFade(string sound,float seconds)
    {
        //Print the time of when the function is first called.
        //Debug.Log("Started Coroutine at timestamp FadeSound : " + Time.time);

        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(seconds);
        
        Play(sound);



        //After we have waited 5 seconds print the time again.
        //Debug.Log("Finished Coroutine at timestamp FadeSound : " + Time.time);
    }


    public GameObject Play(string sound)
	{
        //try
        //{
        //    GameObject soundObj = Instantiate(Array.Find(listSounds, item => item.name == sound));
        //}
        //catch
        //{
        //    Debug.LogError("SoundObject null!");
        //    return;
        //}
        try
        {

            GameObject soundObj = Instantiate(Array.Find(listSounds, item => item.name == sound));
            AddSettings addSettings = soundObj.GetComponent<AddSettings>();
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            float audioLength = audioSource.clip.length;

            audioSource.volume = audioSource.volume * (1f + UnityEngine.Random.Range(-addSettings.volumeVariance / 2f, addSettings.volumeVariance / 2f));
            audioSource.pitch = audioSource.pitch * (1f + UnityEngine.Random.Range(-addSettings.pitchVariance / 2f, addSettings.pitchVariance / 2f));

            if (!audioSource.loop && soundObj != null)
            {
                StartCoroutine(objDestroy(audioLength, soundObj));
            }
            return soundObj;

        }
        catch
        {
            Debug.LogWarning("SoundObj empty or other error related to audio manager");
            GameObject soundObj = null;
            return soundObj;
        }

        
        //Debug.Log("Played Sound");
	}


    public void Play(string sound,float seconds)
    {
        StartCoroutine(objFade(sound,seconds));
    }

    public GameObject Play(string sound,Vector3 position,Quaternion rotation)
    {
        try
        {

            GameObject soundObj = Instantiate(Array.Find(listSounds, item => item.name == sound), position, rotation);
            AddSettings addSettings = soundObj.GetComponent<AddSettings>();
            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            float audioLength = audioSource.clip.length;

            audioSource.volume = audioSource.volume * (1f + UnityEngine.Random.Range(-addSettings.volumeVariance / 2f, addSettings.volumeVariance / 2f));
            audioSource.pitch = audioSource.pitch * (1f + UnityEngine.Random.Range(-addSettings.pitchVariance / 2f, addSettings.pitchVariance / 2f));

            if (!audioSource.loop && soundObj != null)
            {
                StartCoroutine(objDestroy(audioLength, soundObj));
            }
            return soundObj;

        }
        catch
        {
            Debug.LogWarning("SoundObj empty or other error related to audio manager");
            GameObject soundObj = null;
            return soundObj;
        }
    }

    public void Descendo(GameObject soundObj, float howFast)
    {
        //Destroy(soundObj);
        if (soundObj.GetComponent<AudioSource>().volume > 0f)
        {
            soundObj.GetComponent<AudioSource>().volume = soundObj.GetComponent<AudioSource>().volume - howFast * Time.deltaTime;
        }else if (soundObj.GetComponent<AudioSource>().volume <= 0f && soundObj != null)
        {
            Destroy((soundObj));
        }
    }

    


}
