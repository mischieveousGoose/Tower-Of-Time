using UnityEngine;

public class PlayerHealth : MonoBehaviour, Attackable
{
    public float iFrames;
    float iFrameCounter;
    Timer timer;

    DamagePopupSpawner popupSpawner;

    PlayerAttacker attacker;
    SFXManager sfxManager;

    void Start()
    {
        sfxManager = FindFirstObjectByType<SFXManager>();
        timer = GetComponent<Timer>();
        attacker = GetComponent<PlayerAttacker>();
        popupSpawner = FindFirstObjectByType<DamagePopupSpawner>();
    }

    void Update()
    {
        iFrameCounter -= Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        if (attacker.isAttacking) return;
        if (iFrameCounter > 0) return;
        sfxManager.PlaySFX(5);
        iFrameCounter = iFrames;

        popupSpawner.SpawnPopup(transform.position + Vector3.up, "-" + damage.ToString(), Color.red);
        timer.AddTime(-damage);

    }
}
