using UnityEngine;

public class MapTile : MonoBehaviour
{
    [Header("�𶯲���")]
    public float positionShakeAmount = 0.1f;  // λ���𶯷���
    public float scaleShakeAmount = 0.05f;    // �����𶯷���
    public float shakeFrequency = 5f;        // ��Ƶ�� (Hz)
    public bool enableDynamicShake = true;   // �Ƿ������鶯ģʽ���������

    [Header("���� Scale ����")]
    public float bounceScale = 1.2f;         // ���� Scale �ķŴ���
    public float bounceDuration = 0.2f;      // ���� Scale �ĳ���ʱ��
    public int bounceOvershoot = 10;         // ���� overshoot ���������ӵ���Ч����

    [Header("Bright Mask ����")]
    public GameObject brightMask;           // Bright Mask �� GameObject������� SpriteRenderer��
    public float brightMaskFadeDuration = 0.1f; // Bright Mask ���뵭������ʱ��

    private bool isShaking = false;
    private float shakeTime = 0f;
    private float shakeDuration = 0f;
    private Vector3 originalPosition;
    private Vector3 originalScale;
    private SpriteRenderer brightMaskRenderer; // Bright Mask �� SpriteRenderer

    void Start()
    {
        // �����ʼλ�ú�����
        originalPosition = transform.localPosition;
        originalScale = transform.localScale;

        // ��ʼ�� Bright Mask
        if (brightMask != null)
        {
            // ��ȡ Bright Mask �� SpriteRenderer
            brightMaskRenderer = brightMask.GetComponent<SpriteRenderer>();
            if (brightMaskRenderer == null)
            {
                Debug.LogError("Bright Mask GameObject ������� SpriteRenderer �����");
                return;
            }

            // ���ó�ʼ͸����Ϊ 0
            brightMaskRenderer.color = new Color(1, 1, 1, 0);
        }
    }

    void Update()
    {
        if (isShaking)
        {
            shakeTime += Time.deltaTime;

            // ���㵱ǰ�Ƕȣ�����ʱ������Ҳ�����֤�ȶ�Ƶ�ʣ�
            float angle = shakeTime * shakeFrequency * 2f * Mathf.PI;

            // λ����
            float xShake = Mathf.Sin(angle) * positionShakeAmount;
            float yShake = Mathf.Cos(angle * 1.1f) * positionShakeAmount; // �� Y ����΢��ͬ�����Եø���Ȼ

            // ������
            float scaleFactor = Mathf.Sin(angle * 1.2f) * scaleShakeAmount;

            // �鶯ģʽ������Ŷ�����Ӱ���С��
            if (enableDynamicShake)
            {
                xShake += Random.Range(-0.005f, 0.005f);
                yShake += Random.Range(-0.005f, 0.005f);
                scaleFactor += Random.Range(-0.005f, 0.005f);
            }

            // Ӧ����
            transform.localPosition = originalPosition + new Vector3(xShake, yShake, 0);
            transform.localScale = originalScale * (1 + scaleFactor);

            // ����ʱ�����
            if (shakeDuration > 0 && shakeTime >= shakeDuration)
            {
                StopShake();
            }
        }
    }

    /// <summary>
    /// ��ʼ��
    /// </summary>
    /// <param name="duration">�𶯳���ʱ�䣨-1 ��ʾ�����𶯣�</param>
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
    /// ֹͣ��
    /// </summary>
    public void StopShake()
    {
        isShaking = false;
        transform.localPosition = originalPosition;
        transform.localScale = originalScale;
    }

    /// <summary>
    /// ���ŵ��� Scale ��Ч
    /// </summary>
    public void PlayBounceAnimation()
    {
        // ʹ�� LeanTween ʵ�ֵ��Զ���
        LeanTween.scale(gameObject, originalScale * bounceScale, bounceDuration) // �Ŵ� bounceScale ��
            .setEase(LeanTweenType.easeOutQuad) // ��������
            .setOnComplete(() =>
            {
                // ��С��ԭʼ��С�������ӵ��� overshoot Ч��
                LeanTween.scale(gameObject, originalScale, bounceDuration)
                    .setEase(LeanTweenType.easeOutBack) // ʹ�� easeOutBack ��ǿ����
                    .setOvershoot(bounceOvershoot); // ���� overshoot ����
            });

        // ���� Bright Mask �ĵ��뵭��Ч��
        if (brightMaskRenderer != null)
        {
            // ����
            LeanTween.alpha(brightMaskRenderer.gameObject, 1f, brightMaskFadeDuration)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() =>
                {
                    // ����
                    LeanTween.alpha(brightMaskRenderer.gameObject, 0f, brightMaskFadeDuration)
                        .setEase(LeanTweenType.easeInQuad);
                });
        }
    }
}