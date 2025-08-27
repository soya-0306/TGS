using UnityEngine;

public class BattleSEPlayer : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip roundStartSE;
    public AudioClip drawSE;
    public AudioClip rerollSE;
    public AudioClip selectCardSE;
    public AudioClip moveCardSE;
    public AudioClip revealCardSE;
    public AudioClip damageSE;
    public AudioClip tieOrShieldSE;

    private AudioSource audioSource;

    private void Awake()
    {
        // AudioSource ‚ª‚È‚¯‚ê‚ÎŽ©“®’Ç‰Á
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    public void PlayRoundStartSE() => PlaySE(roundStartSE);
    public void PlayDrawSE() => PlaySE(drawSE);
    public void PlayRerollSE() => PlaySE(rerollSE);
    public void PlaySelectCardSE() => PlaySE(selectCardSE);
    public void PlayMoveCardSE() => PlaySE(moveCardSE);
    public void PlayRevealCardSE() => PlaySE(revealCardSE);
    public void PlayDamageSE() => PlaySE(damageSE);
    public void PlayTieOrShieldSE() => PlaySE(tieOrShieldSE);

    private void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SE Clip is not assigned in BattleSEPlayer.");
        }
    }
}
