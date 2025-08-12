using System.Collections.Generic;
using UnityEngine;

/* 사운드 타입 : BGM은 Loop 적용, SFX는 단발적인 효과음적용 */
public enum SoundType
{
    BGM,
    SFX
}

public class SoundManager : SingleTon<SoundManager>
{
    /* 사운드 조절 기능 */
    [SerializeField][Range(0, 1)] private float _soundEffectVolume = 1f;
    [SerializeField][Range(0, 1)] private float _soundEffectPitchVariance = 0.1f;
    [SerializeField][Range(0, 1)] private float _musicVolume = 1f;

    /* 모든 사운드 저장 */
    [SerializeField] private AudioClip[] _musicClips;
    /* 저장된 사운드를 꺼내쓰기 쉽도록 Dictionary에 저장 */
    private Dictionary<string, AudioClip> _audioDictionary;
    private AudioSource _musicAudioSource;
    public SoundPoolManager musicAudioSourcePool;

    public override void Awake()
    {
        base.Awake();
        InitializeAudioSource();
        InitAudioDictionary();
    }

    private void InitializeAudioSource()
    {
        _musicAudioSource = GetComponent<AudioSource>();
        if (_musicAudioSource == null)
        {
            _musicAudioSource = gameObject.AddComponent<AudioSource>();
        }

        _musicAudioSource.volume = _musicVolume;
        _musicAudioSource.loop = true;
    }

    /* Init과정 : 모든 오디오 클립을 Dictionary형태로 재가공 */
    private void InitAudioDictionary()
    {
        _audioDictionary = new Dictionary<string, AudioClip>();

        if (_musicClips != null)
        {
            foreach (var clip in _musicClips)
            {
                if (clip != null && !string.IsNullOrEmpty(clip.name))
                {
                    if (!_audioDictionary.ContainsKey(clip.name))
                    {
                        _audioDictionary.Add(clip.name, clip);
                    }
                    else
                    {
                        Debug.LogWarning($"중복된 오디오 클립 이름: {clip.name}");
                    }
                }
            }
        }
    }

    /* 볼륨 조절 기능. 나중에 옵션으로 사운드를 BGM,SFX 따로 조절할 수 있도록 만든 형태 */
    public void SetVolume(SoundType type, float volume)
    {
        volume = Mathf.Clamp01(volume);

        if (type == SoundType.BGM)
        {
            _musicVolume = volume;
            if (_musicAudioSource != null)
            {
                _musicAudioSource.volume = _musicVolume;
            }
        }
        else if (type == SoundType.SFX)
        {
            _soundEffectVolume = volume;
        }
    }

    /* BGM은 Loop를 돌며 계속해서 반복 재생 */
    public void PlayBGM(string clipName, bool isLoop = true)
    {
        if (_musicAudioSource == null || _audioDictionary == null)
        {
            Debug.LogError("SoundManager: BGM 재생 실패 - AudioSource 또는 AudioDictionary가 null입니다.");
            return;
        }

        if (_audioDictionary.ContainsKey(clipName))
        {
            _musicAudioSource.Stop();
            _musicAudioSource.clip = _audioDictionary[clipName];
            _musicAudioSource.loop = isLoop;
            _musicAudioSource.Play();
        }
        else
        {
            Debug.LogError($"SoundManager: PlayBGM - {clipName}은 존재하지 않는 오디오 클립입니다.");
        }
    }

    /* BGM을 정지할 때 쓰는 메서드 */
    public void StopBGM()
    {
        if (_musicAudioSource != null)
        {
            _musicAudioSource.Stop();
        }
    }

    /* 효과음 재생용 메서드 */
    public void PlaySFX(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            Debug.LogError("SoundManager: PlaySFX - clipName이 null 또는 빈 문자열입니다.");
            return;
        }

        if (musicAudioSourcePool == null)
        {
            musicAudioSourcePool = SoundPoolManager.Instance;
            if (musicAudioSourcePool == null)
            {
                Debug.LogError("SoundManager: SoundPoolManager를 찾을 수 없습니다.");
                return;
            }
        }

        if (_audioDictionary != null && _audioDictionary.ContainsKey(clipName))
        {
            SoundSource soundSource = musicAudioSourcePool.GetObject(0, Vector3.zero, Quaternion.identity);
            if (soundSource != null)
            {
                soundSource.Play(_audioDictionary[clipName], _soundEffectVolume);
            }
            else
            {
                Debug.LogError("SoundManager: SoundSource 객체를 가져올 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"SoundManager: PlaySFX - {clipName}은 존재하지 않는 오디오 클립입니다.");
        }
    }

    /* 효과음 재생 후 해당 효과음 제어를 위해 만들어진 효과음 Prefab을 Return받는데 사용되는 메서드 */
    public SoundSource PlaySfxReturnSoundSource(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            Debug.LogError("SoundManager: PlaySfxReturnSoundSource - clipName이 null 또는 빈 문자열입니다.");
            return null;
        }

        if (musicAudioSourcePool == null)
        {
            musicAudioSourcePool = SoundPoolManager.Instance;
            if (musicAudioSourcePool == null)
            {
                Debug.LogError("SoundManager: SoundPoolManager를 찾을 수 없습니다.");
                return null;
            }
        }

        if (_audioDictionary != null && _audioDictionary.ContainsKey(clipName))
        {
            SoundSource soundSource = musicAudioSourcePool.GetObject(0, Vector3.zero, Quaternion.identity);
            if (soundSource != null)
            {
                soundSource.Play(_audioDictionary[clipName], _soundEffectVolume);
                return soundSource;
            }
            else
            {
                Debug.LogError("SoundManager: SoundSource 객체를 가져올 수 없습니다.");
                return null;
            }
        }
        else
        {
            Debug.LogError($"SoundManager: PlaySfxReturnSoundSource - {clipName}은 존재하지 않는 오디오 클립입니다.");
            return null;
        }
    }

    /* 효과음을 특정 위치에 재생시켜주기 위한 메서드. 매개변수 Vector3 위치에 생성, Transform으로 생성될 부모 오브젝트를 정함 */
    public void PlaySFX(string clipName, Vector3 position, Transform parent)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            Debug.LogError("SoundManager: PlaySFX - clipName이 null 또는 빈 문자열입니다.");
            return;
        }

        if (musicAudioSourcePool == null)
        {
            musicAudioSourcePool = SoundPoolManager.Instance;
            if (musicAudioSourcePool == null)
            {
                Debug.LogError("SoundManager: SoundPoolManager를 찾을 수 없습니다.");
                return;
            }
        }

        if (_audioDictionary != null && _audioDictionary.ContainsKey(clipName))
        {
            SoundSource soundSource = musicAudioSourcePool.GetObject(1, position, Quaternion.identity);
            if (soundSource != null)
            {
                if (parent != null)
                {
                    soundSource.transform.SetParent(parent);
                }
                soundSource.Play(_audioDictionary[clipName], _soundEffectVolume);
            }
            else
            {
                Debug.LogError("SoundManager: SoundSource 객체를 가져올 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"SoundManager: PlaySFX - {clipName}은 존재하지 않는 오디오 클립입니다.");
        }
    }
    
    
    public SoundSource PlaySFXReturnSoundSource(string clipName, Vector3 position, Transform parent)
    {
        if (string.IsNullOrEmpty(clipName))
        {
            Debug.LogError("SoundManager: PlaySFX - clipName이 null 또는 빈 문자열입니다.");
            return null;
        }

        if (musicAudioSourcePool == null)
        {
            musicAudioSourcePool = SoundPoolManager.Instance;
            if (musicAudioSourcePool == null)
            {
                Debug.LogError("SoundManager: SoundPoolManager를 찾을 수 없습니다.");
                return null;
            }
        }

        if (_audioDictionary != null && _audioDictionary.ContainsKey(clipName))
        {
            SoundSource soundSource = musicAudioSourcePool.GetObject(1, position, Quaternion.identity);
            if (soundSource != null)
            {
                if (parent != null)
                {
                    soundSource.transform.SetParent(parent);
                }
                soundSource.Play(_audioDictionary[clipName], _soundEffectVolume);
                return soundSource;
            }
            else
            {
                Debug.LogError("SoundManager: SoundSource 객체를 가져올 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError($"SoundManager: PlaySFX - {clipName}은 존재하지 않는 오디오 클립입니다.");
        }

        return null;
    }
}