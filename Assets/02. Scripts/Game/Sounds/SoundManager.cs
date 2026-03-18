using UnityEngine;
using UnityEngine.Audio;



namespace Necrogue.Game.Sounds
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("오디오 믹서 (옵션)")]
        public AudioMixer mixer;

        [Header("BGM 클립들/직접할당")]
        public AudioClip[] bgm;

        [Header("SFX 클립들/직접할당")]
        public AudioClip[] sfx;

        // 재사용 가능한 AudioSource들
        private AudioSource bgmSource;      // BGM 전용 (1개만 있으면 됨)
        private AudioSource sfxSource;      // 간단한 SFX 전용 (1개로 충분)

        private void Awake()
        {
            var root = transform.root.gameObject;

            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate {typeof(SoundManager).Name} detected. Destroying root: {root.name}");
                Destroy(root);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(root);

            // AudioSource 자동 할당
            AudioSource[] sources = GetComponents<AudioSource>();
            bgmSource = sources[0];
            if (sources.Length > 1)
                sfxSource = sources[1];
            else
                sfxSource = gameObject.AddComponent<AudioSource>();

            // BGM 기본 설정
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;

            // 믹서 연결 (있으면)
            if (mixer != null)
            {
                bgmSource.outputAudioMixerGroup = mixer.FindMatchingGroups("BGM")[0];
                sfxSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
            }
        }

        // ==================== BGM ====================
        public void PlayBGM(int index)
        {
            if (index < 0 || index >= bgm.Length || bgm[index] == null) return;

            if (bgmSource.clip == bgm[index] && bgmSource.isPlaying) return;

            bgmSource.Stop();
            bgmSource.clip = bgm[index];
            bgmSource.Play();
        }

        public void StopBGM() => bgmSource.Stop();
        public void PauseBGM() => bgmSource.Pause();
        public void ResumeBGM() => bgmSource.UnPause();

        // ==================== SFX ====================
        public void PlaySFX(int index)
        {
            if (index < 0 || index >= sfx.Length || sfx[index] == null) return;
            sfxSource.PlayOneShot(sfx[index]);
        }

        // 이름으로 효과음 호출
        public void Hurt() => PlaySFX(0);   // 예: 맞을 때
    }
}