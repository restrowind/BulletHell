using UnityEngine;
using System.Collections.Generic;

public class GlobalAudioPlayer : MonoBehaviour
{
    public static GlobalAudioPlayer Instance;

    [System.Serializable]
    public class NamedAudioClip
    {
        public string name;
        public AudioClip clip;
    }

    [Header("��Ƶ�����б�")]
    public List<NamedAudioClip> audioClips = new List<NamedAudioClip>();

    private Dictionary<string, AudioClip> clipDict = new Dictionary<string, AudioClip>();
    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();
    private int poolSize = 10;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var entry in audioClips)
            {
                if (!clipDict.ContainsKey(entry.name))
                {
                    clipDict.Add(entry.name, entry.clip);
                }
            }

            // ��ʼ�� AudioSource ��
            for (int i = 0; i < poolSize; i++)
            {
                AudioSource src = gameObject.AddComponent<AudioSource>();
                src.playOnAwake = false;
                audioSourcePool.Enqueue(src);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ��ȡһ�����е� AudioSource����̬��չ��
    /// </summary>
    private AudioSource GetAudioSource()
    {
        foreach (var source in audioSourcePool)
        {
            if (!source.isPlaying)
                return source;
        }

        // ���ݣ���ѡ��
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        audioSourcePool.Enqueue(newSource);
        return newSource;
    }

    /// <summary>
    /// ������Ƶ���ɵ��Ӳ��ţ��Զ�����
    /// </summary>
    public void Play(string name, bool loop = false)
    {
        if (!clipDict.ContainsKey(name)) return;

        AudioSource src = GetAudioSource();
        src.clip = clipDict[name];
        src.loop = loop;
        src.Play();

        if (!loop)
        {
            StartCoroutine(RecycleAfter(src, src.clip.length));
        }
    }

    private System.Collections.IEnumerator RecycleAfter(AudioSource src, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!src.loop)
        {
            src.Stop();
            src.clip = null;
        }
    }

    /// <summary>
    /// ���Ų����ӵ���Ƶ������ͬ����β��ţ�
    /// </summary>
    public void PlaySingle(string name, bool loop = false)
    {
        if (!clipDict.ContainsKey(name)) return;

        foreach (var source in audioSourcePool)
        {
            if (source.clip == clipDict[name] && source.isPlaying)
                return;
        }

        Play(name, loop);
    }
}
