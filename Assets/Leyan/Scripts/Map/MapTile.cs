using UnityEngine;

public class MapTile : MonoBehaviour
{
    [Header("震动参数")]
    public float positionShakeAmount = 0.1f;  // 位置震动幅度
    public float scaleShakeAmount = 0.05f;    // 缩放震动幅度
    public float shakeFrequency = 5f;        // 震动频率 (Hz)
    public bool enableDynamicShake = true;   // 是否启用灵动模式（更随机）

    [Header("弹性 Scale 参数")]
    public float bounceScale = 1.2f;         // 弹性 Scale 的放大倍数
    public float bounceDuration = 0.2f;      // 弹性 Scale 的持续时间
    public int bounceOvershoot = 10;         // 弹性 overshoot 参数（增加弹性效果）

    [Header("Bright Mask 参数")]
    public GameObject brightMask;           // Bright Mask 的 GameObject（需包含 SpriteRenderer）
    public float brightMaskFadeDuration = 0.1f; // Bright Mask 淡入淡出持续时间

    private bool isShaking = false;
    private float shakeTime = 0f;
    private float shakeDuration = 0f;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private SpriteRenderer brightMaskRenderer; // Bright Mask 的 SpriteRenderer

    void Start()
    {
        // 保存初始位置和缩放
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;

        // 初始化 Bright Mask
        if (brightMask != null)
        {
            // 获取 Bright Mask 的 SpriteRenderer
            brightMaskRenderer = brightMask.GetComponent<SpriteRenderer>();
            if (brightMaskRenderer == null)
            {
                Debug.LogError("Bright Mask GameObject 必须包含 SpriteRenderer 组件！");
                return;
            }

            // 设置初始透明度为 0
            brightMaskRenderer.color = new Color(1, 1, 1, 0);
        }
    }

    void Update()
    {
        if (isShaking)
        {
            shakeTime += Time.deltaTime;

            // 计算当前角度（基于时间的正弦波，保证稳定频率）
            float angle = shakeTime * shakeFrequency * 2f * Mathf.PI;

            // 位置震动
            float xShake = Mathf.Sin(angle) * positionShakeAmount;
            float yShake = Mathf.Cos(angle * 1.1f) * positionShakeAmount; // 让 Y 轴稍微不同步，显得更自然

            // 缩放震动
            float scaleFactor = Mathf.Sin(angle * 1.2f) * scaleShakeAmount;

            // 灵动模式（随机扰动，但影响较小）
            if (enableDynamicShake)
            {
                xShake += Random.Range(-0.005f, 0.005f);
                yShake += Random.Range(-0.005f, 0.005f);
                scaleFactor += Random.Range(-0.005f, 0.005f);
            }

            // 应用震动
            transform.localPosition = originalPosition + new Vector3(xShake, yShake, 0);
            transform.localScale = originalScale * (1 + scaleFactor);

            // 持续时间控制
            if (shakeDuration > 0 && shakeTime >= shakeDuration)
            {
                StopShake();
            }
        }
    }

    /// <summary>
    /// 开始震动
    /// </summary>
    /// <param name="duration">震动持续时间（-1 表示无限震动）</param>
    public void StartShake(float duration = -1f)
    {
        if (!isShaking)
        {
            isShaking = true;
            shakeTime = 0f;
            shakeDuration = duration;
        }
    }

    /// <summary>
    /// 停止震动
    /// </summary>
    public void StopShake()
    {
        isShaking = false;
        transform.localPosition = originalPosition;
        transform.localScale = originalScale;
    }

    /// <summary>
    /// 播放弹性 Scale 动效
    /// </summary>
    public void PlayBounceAnimation()
    {
        // 使用 LeanTween 实现弹性动画
        LeanTween.scale(gameObject, originalScale * bounceScale, bounceDuration) // 放大到 bounceScale 倍
            .setEase(LeanTweenType.easeOutQuad) // 缓动类型
            .setOnComplete(() =>
            {
                // 缩小回原始大小，并增加弹性 overshoot 效果
                LeanTween.scale(gameObject, originalScale, bounceDuration)
                    .setEase(LeanTweenType.easeOutBack) // 使用 easeOutBack 增强弹性
                    .setOvershoot(bounceOvershoot); // 设置 overshoot 参数
            });

        // 播放 Bright Mask 的淡入淡出效果
        if (brightMaskRenderer != null)
        {
            // 淡入
            LeanTween.alpha(brightMaskRenderer.gameObject, 1f, brightMaskFadeDuration)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() =>
                {
                    // 淡出
                    LeanTween.alpha(brightMaskRenderer.gameObject, 0f, brightMaskFadeDuration)
                        .setEase(LeanTweenType.easeInQuad);
                });
        }
    }
}