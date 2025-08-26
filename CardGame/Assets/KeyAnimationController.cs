using UnityEngine;

public class KeyAnimationController : MonoBehaviour
{
    private Animation anim;

    [Header("アニメーションクリップ")]
    public AnimationClip IdleClip;
    public AnimationClip FightClip;
    public AnimationClip WeaponClip;
    public AnimationClip MagicClip;
    public AnimationClip DamageClip;

    [Header("エフェクト")]
    public GameObject FightEffect;
    public GameObject WeaponEffect;
    public GameObject MagicEffect;
    public GameObject DamageEffect;

    [Header("エフェクトスポーンポイント")]
    public Transform FightEffectSpawnPoint;
    public Transform WeaponEffectSpawnPoint;
    public Transform MagicEffectSpawnPoint;
    public Transform DamageEffectSpawnPoint;

    [Header("サウンドエフェクト (SE)")]
    public AudioClip FightSE;
    public AudioClip WeaponSE;
    public AudioClip MagicSE;
    public AudioClip DamageSE;

    public float WaitFightSE;
    public float WaitWeaponSE;
    public float WaitMagicSE;
    public float WaitDamageSE;

    private GameObject currentEffectInstance = null;
    private float currentAnimEndTime = 0f;
    private AudioSource audioSource;

    // SE再生制御用
    private float sePlayTime = -1f;
    private AudioClip pendingSE = null;

    void Awake()
    {
        anim = GetComponent<Animation>();
        if (anim == null)
        {
            anim = gameObject.AddComponent<Animation>();
            Debug.LogWarning("Animation コンポーネントがなかったので追加しました。");
        }

        // AudioSource がなければ追加
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        AddClipIfNeeded(IdleClip, WrapMode.Loop);
        AddClipIfNeeded(FightClip, WrapMode.Once);
        AddClipIfNeeded(WeaponClip, WrapMode.Once);
        AddClipIfNeeded(MagicClip, WrapMode.Once);
        AddClipIfNeeded(DamageClip, WrapMode.Once);

        if (IdleClip != null)
        {
            anim.Play(IdleClip.name);
        }
    }

    void Update()
    {
        // アニメーション終了後にIdleに戻す
        if (Time.time > currentAnimEndTime && !anim.IsPlaying(IdleClip.name))
        {
            anim.Play(IdleClip.name);
            StopCurrentEffect();
        }

        // SEの再生タイミングを監視
        if (pendingSE != null && Time.time >= sePlayTime)
        {
            PlaySE(pendingSE);
            pendingSE = null;
        }

        // キー入力でテスト
        if (Input.GetKeyDown(KeyCode.Alpha1) && FightClip != null)
        {
            PlayAnimationWithSE(FightClip, FightEffect, FightEffectSpawnPoint, FightSE, WaitFightSE); // 0.4秒後にSE
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && WeaponClip != null)
        {
            PlayAnimationWithSE(WeaponClip, WeaponEffect, WeaponEffectSpawnPoint, WeaponSE, WaitWeaponSE);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && MagicClip != null)
        {
            PlayAnimationWithSE(MagicClip, MagicEffect, MagicEffectSpawnPoint, MagicSE, WaitMagicSE);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && DamageClip != null)
        {
            PlayAnimationWithSE(DamageClip, DamageEffect, DamageEffectSpawnPoint, DamageSE, WaitDamageSE);
        }
    }

    // 🔑 アニメーションとSE再生タイミングをまとめて制御
    private void PlayAnimationWithSE(AnimationClip clip, GameObject effect, Transform spawnPoint, AudioClip seClip, float seDelay)
    {
        anim.Play(clip.name);
        currentAnimEndTime = Time.time + clip.length;

        PlayEffect(effect, spawnPoint);

        // SE再生タイミングをセット
        if (seClip != null)
        {
            sePlayTime = Time.time + seDelay;
            pendingSE = seClip;
        }
    }

    private void PlayEffect(GameObject effectPrefab, Transform spawnPoint)
    {
        StopCurrentEffect();

        if (effectPrefab != null && spawnPoint != null)
        {
            currentEffectInstance = Instantiate(effectPrefab, spawnPoint.position, Quaternion.identity);

            // キャラに追従
            currentEffectInstance.transform.SetParent(spawnPoint);
            currentEffectInstance.transform.localPosition = Vector3.zero;

            // スケール調整
            float baseScale = 1.0f;
            Vector3 parentScale = spawnPoint.lossyScale;
            float averageScale = (parentScale.x + parentScale.y + parentScale.z) / 3f;

            currentEffectInstance.transform.localScale = Vector3.one * baseScale * averageScale;
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

    public void PlayCardAnimation(CardType type)
    {
        switch (type)
        {
            case CardType.Rock:
                PlayAnimationWithSE(FightClip, FightEffect, FightEffectSpawnPoint, FightSE, WaitFightSE);
                break;
            case CardType.Paper:
                PlayAnimationWithSE(WeaponClip, WeaponEffect, WeaponEffectSpawnPoint, WeaponSE, WaitWeaponSE);
                break;
            case CardType.Scissors:
                PlayAnimationWithSE(MagicClip, MagicEffect, MagicEffectSpawnPoint, MagicSE, WaitMagicSE);
                break;
            default:
                break;
        }
    }

    public void PlayDamageAnimation()
    {
        if (DamageClip != null)
        {
            PlayAnimationWithSE(DamageClip, DamageEffect, DamageEffectSpawnPoint, DamageSE, WaitDamageSE);
        }
    }

}

