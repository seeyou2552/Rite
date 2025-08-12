using System;
using UnityEngine;

public class SoundSource : MonoBehaviour, IPoolable
{
    private AudioSource _audioSource;
    public AudioSource AudioSource => _audioSource;
    private Action<SoundSource> returnToPool;

    private void Awake()
    {
        // AudioSource 컴포넌트 초기화
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    public void Play(AudioClip audioClip, float soundEffectVolume)
    {
        // null 체크 추가
        if (audioClip == null)
        {
            Debug.LogError("SoundSource: audioClip이 null입니다.");
            Disable();
            return;
        }

        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                Debug.LogError("SoundSource: AudioSource 컴포넌트를 찾을 수 없습니다.");
                Disable();
                return;
            }
        }

        CancelInvoke();
        _audioSource.clip = audioClip;
        _audioSource.volume = Mathf.Clamp01(soundEffectVolume);
        _audioSource.Play();

        // 오디오 길이 + 여유시간 후 비활성화
        Invoke(nameof(Disable), audioClip.length + 0.1f);
    }

    public void Disable()
    {
        // AudioSource 정지
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }

        // 모든 Invoke 취소
        CancelInvoke();

        OnDespawn();
    }

    public void Initialize(Action<SoundSource> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        // 스폰 시 초기화
        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource.clip = null;
        }
        CancelInvoke();
    }

    public void OnDespawn()
    {
        returnToPool?.Invoke(this);
    }

    // 수동으로 정지하고 풀에 반환
    public void Stop()
    {
        Disable();
    }

    // 현재 재생 중인지 확인
    public bool IsPlaying()
    {
        return _audioSource != null && _audioSource.isPlaying;
    }

    // 남은 재생 시간 확인
    public float GetRemainingTime()
    {
        if (_audioSource != null && _audioSource.clip != null && _audioSource.isPlaying)
        {
            return _audioSource.clip.length - _audioSource.time;
        }
        return 0f;
    }
}