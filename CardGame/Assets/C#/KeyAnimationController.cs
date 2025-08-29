using UnityEngine;

[System.Serializable]
public class EffectTiming
{
    public GameObject effectPrefab;
    public Transform spawnPoint;
    public float startTime = 0f; // エフェクト開始までの遅延(秒)
    public float endTime = 1f;   // 開始から何秒後に消すか
}

public class KeyAnimationController : MonoBehaviour
{
    private Animation anim;
    private AudioSource audioSource;

    private float currentAnimEndTime = 0f;
    private float sePlayTime = -1f;
    private AudioClip pendingSE = null;

    private GameObject currentEffectInstance = null;
    private float effectStartTime = -1f;
    private float effectEndTime = -1f;
    private EffectTiming pendingEffect = null;

    [Header("アニメーションクリップ")]
    public AnimationClip IdleClip;
    public AnimationClip FightClip;
    public AnimationClip WeaponClip;
    public AnimationClip MagicClip;
    public AnimationClip DamageClip;
    public AnimationClip ShieldClip;

    [Header("SE設定")]
    public AudioClip FightSE;
    public float WaitFightSE;
    public AudioClip WeaponSE;
    public float WaitWeaponSE;
    public AudioClip MagicSE;
    public float WaitMagicSE;
    public AudioClip DamageSE;
    public float WaitDamageSE;
    public AudioClip ShieldSE;
    public float WaitShieldSE;

    [Header("エフェクト設定")]
    public EffectTiming FightEffect;
    public EffectTiming WeaponEffect;
    public EffectTiming MagicEffect;
    public EffectTiming DamageEffect;
    public EffectTiming ShieldEffect;

    void Awake()
    {
        anim = GetComponent<Animation>();
        if (anim == null) anim = gameObject.AddComponent<Animation>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        AddClipIfNeeded(IdleClip, WrapMode.Loop);
        AddClipIfNeeded(FightClip, WrapMode.Once);
        AddClipIfNeeded(WeaponClip, WrapMode.Once);
        AddClipIfNeeded(MagicClip, WrapMode.Once);
        AddClipIfNeeded(DamageClip, WrapMode.Once);
        AddClipIfNeeded (ShieldClip, WrapMode.Once);

        if (IdleClip != null) anim.Play(IdleClip.name);
    }

    void Update()
    {
        // アニメーション終了後にIdleに戻す
        if (Time.time > currentAnimEndTime && !anim.IsPlaying(IdleClip.name))
        {
            anim.Play(IdleClip.name);
            StopCurrentEffect();
        }

        // SE再生タイミング
        if (pendingSE != null && Time.time >= sePlayTime)
        {
            PlaySE(pendingSE);
            pendingSE = null;
        }

        // エフェクト開始
        if (pendingEffect != null && Time.time >= effectStartTime && currentEffectInstance == null)
        {
            PlayEffect(pendingEffect);
        }

        // エフェクト終了
        if (currentEffectInstance != null && Time.time >= effectEndTime)
        {
            StopCurrentEffect();
        }
    }

    private void PlayAnimationWithSE(AnimationClip clip, EffectTiming effectTiming, AudioClip seClip, float seDelay)
    {
        anim.Play(clip.name);
        currentAnimEndTime = Time.time + clip.length;

        // エフェクトのスケジュール
        if (effectTiming != null && effectTiming.effectPrefab != null)
        {
            pendingEffect = effectTiming;
            effectStartTime = Time.time + effectTiming.startTime;
            effectEndTime = Time.time + effectTiming.endTime;
        }

        // SEスケジュール
        if (seClip != null)
        {
            sePlayTime = Time.time + seDelay;
            pendingSE = seClip;
        }
    }

    private void PlayEffect(EffectTiming effectTiming)
    {
        StopCurrentEffect();

        if (effectTiming.effectPrefab != null && effectTiming.spawnPoint != null)
        {
            currentEffectInstance = Instantiate(effectTiming.effectPrefab, effectTiming.spawnPoint.position, Quaternion.identity);
            currentEffectInstance.transform.SetParent(effectTiming.spawnPoint);
            currentEffectInstance.transform.localPosition = Vector3.zero;
            currentEffectInstance.transform.localScale = Vector3.one;
        }
    }

    private void StopCurrentEffect()
    {
        if (currentEffectInstance != null)
        {
            Destroy(currentEffectInstance);
            currentEffectInstance = null;
        }
    }

    private void AddClipIfNeeded(AnimationClip clip, WrapMode mode)
    {
        if (clip != null)
        {
            clip.wrapMode = mode;
            if (anim.GetClip(clip.name) == null)
            {
                anim.AddClip(clip, clip.name);
            }
        }
    }

    private void PlaySE(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // 呼び出し例
    public void PlayCardAnimation(CardType type)
    {
        switch (type)
        {
            case CardType.Rock:
                PlayAnimationWithSE(FightClip, FightEffect, FightSE, WaitFightSE);
                break;
            case CardType.Paper:
                PlayAnimationWithSE(WeaponClip, WeaponEffect, WeaponSE, WaitWeaponSE);
                break;
            case CardType.Scissors:
                PlayAnimationWithSE(MagicClip, MagicEffect, MagicSE, WaitMagicSE);
                break;
        }
    }

    public void PlayDamageAnimation()
    {
        PlayAnimationWithSE(DamageClip, DamageEffect, DamageSE, WaitDamageSE);
    }
}
