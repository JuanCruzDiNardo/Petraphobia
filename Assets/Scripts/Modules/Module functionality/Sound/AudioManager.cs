using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource motorSource;
    [SerializeField] private AudioSource drillSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource sonarSource;
    [SerializeField] private AudioSource switchSource;

    [Header("Clips")]
    [SerializeField] private AudioClip motorClip;
    [SerializeField] private AudioClip drillClip;
    [SerializeField] private AudioClip overloadClip;
    [SerializeField] private AudioClip powerRestoreClip;
    [SerializeField] private AudioClip sonarBeepClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip switchClip;

    [Header("Volumes")]
    [Range(0f, 1f)][SerializeField] private float motorVolume = 0.3f;
    [Range(0f, 1f)][SerializeField] private float drillVolume = 0.25f;
    [Range(0f, 1f)][SerializeField] private float sfxVolume = 0.8f;
    [Range(0f, 1f)][SerializeField] private float deathVolume = 0.2f;
    [Range(0f, 1f)][SerializeField] private float sonarVolume = 0.6f;

    private void Awake()
    {
        SetupLoopSource(motorSource, motorClip, motorVolume);
        SetupLoopSource(drillSource, drillClip, drillVolume);

        EnergyManager.OnOverload += StopMotor;
        EnergyManager.OnOverload += StopDrill;
        EnergyManager.OnOverload += PlayEnergyOverload;

        EnergyManager.OnRestored += PlayDrill;
        EnergyManager.OnRestored += PlayMotor;
        EnergyManager.OnRestored += PlayEnergyRestored;

        SonarModule.OnSonarPing += PlaySonarBeep;

        EnemyManager.OnPlayerDeath += PlayPLayerDeath;
        EnemyManager.OnPlayerDeath += StopDrill;
        EnemyManager.OnPlayerDeath += StopMotor;

        SwitchModule.OnClick += PlaySwitchClick;

        PlayDrill();
        PlayMotor();
    }

    private void SetupLoopSource(AudioSource source, AudioClip clip, float volume)
    {
        if (source == null || clip == null) return;

        source.clip = clip;
        source.loop = true;
        source.volume = volume;
        source.playOnAwake = false;
    }

    // ======================
    // MOTOR
    // ======================
    public void PlayMotor()
    {
        if (!motorSource.isPlaying)
            motorSource.Play();
    }

    public void StopMotor()
    {
        motorSource.Stop();
    }

    // ======================
    // TALADRO
    // ======================
    public void PlayDrill()
    {
        if (!drillSource.isPlaying)
            drillSource.Play();
    }

    public void StopDrill()
    {
        drillSource.Stop();
    }

    // ======================
    // ENERGÍA
    // ======================
    public void PlayEnergyOverload()
    {
        sfxSource.PlayOneShot(overloadClip, sfxVolume);
    }

    public void PlayEnergyRestored()
    {
        sfxSource.PlayOneShot(powerRestoreClip, sfxVolume);
    }

    // ======================
    // SONAR
    // ======================
    public void PlaySonarBeep()
    {
        sonarSource.PlayOneShot(sonarBeepClip, sonarVolume);
    }

    public void PlayPLayerDeath()
    {
        sfxSource.PlayOneShot(deathClip, deathVolume);
    }

    // ======================
    // SONAR
    // ======================
    public void PlaySwitchClick()
    {
        sonarSource.PlayOneShot(switchClip, sfxVolume);
    }
}
