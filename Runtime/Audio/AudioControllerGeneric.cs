using System;
using System.Collections.Generic;
using CustomUtils.Runtime.Audio.Data;
using CustomUtils.Runtime.Pools.Objects;
using CustomUtils.Unsafe;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace CustomUtils.Runtime.Audio
{
    [PublicAPI]
    public abstract class AudioControllerGeneric<TMusicType, TSfxType> : MonoBehaviour
        where TMusicType : unmanaged, Enum
        where TSfxType : unmanaged, Enum
    {
        [SerializeField] private AudioDatabaseGeneric<TMusicType, TSfxType> _audioDatabaseGeneric;

        [SerializeField] private PoolConfig<AudioSource> _soundPoolConfig;
        [SerializeField] private AudioSource _clipSource;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _oneShotSource;

        private readonly Dictionary<int, float> _lastPlayedTimes = new();
        private readonly List<AliveAudioData<TSfxType>> _aliveAudios = new();
        private readonly List<AliveAudioData<TSfxType>> _audiosToRemove = new();

        private Pool<AudioSource> _soundPool;

        public virtual void Initialize()
        {
            _soundPool = new ComponentPool<AudioSource>(_soundPoolConfig);
        }

        public virtual AudioSource PlaySfx(TSfxType soundType, float volumeModifier = 1, float pitchModifier = 1)
        {
            var soundData = _audioDatabaseGeneric.SoundContainers[soundType];
            if (!ShouldPlaySound(soundType, soundData))
                return null;

            var soundTypeValue = UnsafeEnumConverter<TSfxType>.ToInt32(soundType);
            _lastPlayedTimes[soundTypeValue] = Time.unscaledTime;

            var soundSource = _soundPool.Get();
            soundSource.clip = soundData.AudioData.AudioClip;
            soundSource.pitch = pitchModifier * soundData.AudioData.RandomPitch.Value;
            soundSource.volume = volumeModifier * soundData.AudioData.RandomVolume.Value;

            soundSource.Play();

            var aliveData = new AliveAudioData<TSfxType>(soundType, soundSource);
            _aliveAudios.Add(aliveData);

            PlaySoundInternal(aliveData).Forget();

            return soundSource;
        }

        public virtual AudioSource PlayClip(AudioClip soundType, float volumeModifier = 1, float pitchModifier = 1)
        {
            _clipSource.clip = soundType;
            _clipSource.pitch = pitchModifier;
            _clipSource.volume = volumeModifier;

            _clipSource.Play();

            return _clipSource;
        }

        public virtual void StopClip()
        {
            _clipSource.Stop();
        }

        public virtual void PlayOneShotSfx(TSfxType soundType, float volumeModifier = 1, float pitchModifier = 1)
        {
            var soundData = _audioDatabaseGeneric.SoundContainers[soundType];

            if (soundData?.AudioData == null || !soundData.AudioData?.AudioClip)
                return;

            _oneShotSource.pitch = pitchModifier * soundData.AudioData.RandomPitch.Value;
            _oneShotSource.volume = volumeModifier * soundData.AudioData.RandomVolume.Value;
            _oneShotSource.PlayOneShot(soundData.AudioData.AudioClip);
        }

        public virtual AudioSource PlayMusic(TMusicType musicType)
        {
            var musicData = _audioDatabaseGeneric.MusicContainers[musicType];
            return musicData == null ? null : PlayMusic(musicData);
        }

        public virtual AudioSource PlayMusic(AudioData data)
        {
            if (data == null || !data.AudioClip)
                return null;

            _musicSource.clip = data.AudioClip;
            _musicSource.pitch = data.RandomPitch.Value;
            _musicSource.volume = data.RandomVolume.Value;
            _musicSource.Play();

            return _musicSource;
        }

        public virtual void StopMusic()
        {
            _musicSource.Stop();
        }

        public virtual void StopSound(TSfxType soundType)
        {
            var soundTypeValueToRemove = UnsafeEnumConverter<TSfxType>.ToInt32(soundType);

            _audiosToRemove.Clear();
            foreach (var audioData in _aliveAudios)
            {
                var soundTypeValue = UnsafeEnumConverter<TSfxType>.ToInt32(audioData.SoundType);

                if (soundTypeValue != soundTypeValueToRemove)
                    continue;

                audioData.AudioSource.Stop();
                _soundPool.Release(audioData.AudioSource);
                _audiosToRemove.Add(audioData);
            }

            foreach (var audioData in _audiosToRemove)
                _aliveAudios.Remove(audioData);
        }

        private bool ShouldPlaySound(TSfxType soundType, SoundContainer soundData)
        {
            if (soundData?.AudioData == null || !soundData.AudioData?.AudioClip)
                return false;

            var soundValue = UnsafeEnumConverter<TSfxType>.ToInt32(soundType);
            return soundData.Cooldown == 0 ||
                   !_lastPlayedTimes.TryGetValue(soundValue, out var lastTime) ||
                   !(Time.unscaledTime < lastTime + soundData.Cooldown);
        }

        private async UniTask PlaySoundInternal(AliveAudioData<TSfxType> aliveData)
        {
            await UniTask.WaitForSeconds(aliveData.AudioSource.clip.length);

            _soundPool.Release(aliveData.AudioSource);
            _aliveAudios.Remove(aliveData);
        }
    }
}