using UnityEngine;

public class AudioSceneController : MonoBehaviour
{
    public AudioClip clip;
    public bool isLoop = true;
    public int repeatCount = 1;

    private AudioSource audioSource;

    void Start()
    {

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        audioSource = GetComponent<AudioSource>();

        if (isLoop)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            StartCoroutine(PlayClipMultipleTimes(clip, repeatCount));
        }
    }

    System.Collections.IEnumerator PlayClipMultipleTimes(AudioClip clip, int times)
    {
        for (int i = 0; i < times; i++)
        {
            audioSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length + 1f);
        }
    }
}
