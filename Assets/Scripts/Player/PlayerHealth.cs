using UnityEngine;

public class PlayerHealth : MonoBehaviour, Attackable
{
    public float iFrames;
    float iFrameCounter;
    Player player;

    DamagePopupSpawner popupSpawner;

    PlayerCombat attacker;
    SFXManager sfxManager;

    void Start()
    {
        sfxManager = FindFirstObjectByType<SFXManager>();
        player = GetComponent<Player>();
        attacker = GetComponent<PlayerCombat>();
        popupSpawner = FindFirstObjectByType<DamagePopupSpawner>();
    }

    void Update()
    {
        iFrameCounter -= Time.deltaTime;
    }

    public void SetIFrames(float time)
    {
        iFrameCounter = time;
    }

    public void TakeDamage(float damage)
    {
        if (iFrameCounter > 0) return;
        sfxManager.PlaySFX(5);
        SetIFrames(iFrames);

        popupSpawner.SpawnPopup(transform.position + Vector3.up, "-" + damage.ToString(), Color.red);
        player.AddTime(-damage);

    }
}
