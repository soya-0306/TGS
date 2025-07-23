using UnityEngine;

public class KeyAnimationController : MonoBehaviour
{
    private Animation anim;

    [Header("アニメーションクリップ")]
    public AnimationClip IdleClip;
    public AnimationClip FightClip;
    public AnimationClip WeaponClip;
    public AnimationClip MagicClip;

    [Header("エフェクト")]
    public GameObject FightEffect;
    public GameObject WeaponEffect;
    public GameObject MagicEffect;

    [Header("エフェクトスポーンポイント")]
    public Transform FightEffectSpawnPoint;
    public Transform WeaponEffectSpawnPoint;
    public Transform MagicEffectSpawnPoint;

    private GameObject currentEffectInstance = null;
    private float currentAnimEndTime = 0f;

    void Awake()
    {
        anim = GetComponent<Animation>();
        if (anim == null)
        {
            anim = gameObject.AddComponent<Animation>();
            Debug.LogWarning("Animation コンポーネントがなかったので追加しました。");
        }

        AddClipIfNeeded(IdleClip, WrapMode.Loop);
        AddClipIfNeeded(FightClip, WrapMode.Once);
        AddClipIfNeeded(WeaponClip, WrapMode.Once);
        AddClipIfNeeded(MagicClip, WrapMode.Once);

        if (IdleClip != null)
        {
            anim.Play(IdleClip.name);
        }
    }

    void Update()
    {
        // アニメーション終了後にidleに戻す
        if (Time.time > currentAnimEndTime && !anim.IsPlaying(IdleClip.name))
        {
            anim.Play(IdleClip.name);
            StopCurrentEffect();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && FightClip != null)
        {
            PlayAnimation(FightClip.name, FightClip.length);
            PlayEffect(FightEffect, FightEffectSpawnPoint);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && WeaponClip != null)
        {
            PlayAnimation(WeaponClip.name, WeaponClip.length);
            PlayEffect(WeaponEffect, WeaponEffectSpawnPoint);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && MagicClip != null)
        {
            PlayAnimation(MagicClip.name, MagicClip.length);
            PlayEffect(MagicEffect, MagicEffectSpawnPoint);
        }
    }

    private void PlayAnimation(string name, float duration)
    {
        anim.Play(name);
        currentAnimEndTime = Time.time + duration;
    }

    private void PlayEffect(GameObject effect, Transform spawnPoint)
    {
        StopCurrentEffect();

        if (effect != null && spawnPoint != null)
        {
            currentEffectInstance = Instantiate(effect, spawnPoint.position, Quaternion.identity);
            currentEffectInstance.transform.SetParent(spawnPoint);
            currentEffectInstance.transform.localPosition = Vector3.zero;
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
}
