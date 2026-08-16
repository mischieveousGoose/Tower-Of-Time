using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    GameObject primaryWeaponObject;
    GameObject secondaryWeaponObject;

    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;

    [Header("UI")]
    public Image primaryIcon;
    public Image secondaryIcon;

    void Start()
    {
        primaryIcon.enabled = false;
        secondaryIcon.enabled = false;
        if (GameManager.Instance.runData.primaryWeapon != null)
        {
            primaryWeaponObject = GameManager.Instance.runData.primaryWeapon;
            primaryWeapon = Instantiate(primaryWeaponObject, transform).GetComponent<Weapon>();
            primaryIcon.enabled = true;
            primaryIcon.sprite = primaryWeapon.item.icon;
        }

        if (GameManager.Instance.runData.secondaryWeapon != null)
        {
            secondaryWeaponObject = GameManager.Instance.runData.secondaryWeapon;
            secondaryWeapon = Instantiate(secondaryWeaponObject, transform).GetComponent<Weapon>();
            secondaryIcon.enabled = true;
            secondaryIcon.sprite = secondaryWeapon.item.icon;
        }
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && primaryWeapon != null)
        {
            primaryWeapon.OnUse();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame && secondaryWeapon != null)
        {
            secondaryWeapon.OnUse();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && primaryWeapon != null)
        {
            primaryWeapon.ReleaseAttack();
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame && secondaryWeapon != null)
        {
            secondaryWeapon.ReleaseAttack();
        }
    }
}
