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
        if (GameManager.Instance.primaryWeapon != null)
        {
            primaryWeaponObject = GameManager.Instance.primaryWeapon;
            primaryWeapon = Instantiate(primaryWeaponObject, transform).GetComponent<Weapon>();
            primaryIcon.enabled = true;
            primaryIcon.sprite = primaryWeapon.icon;
        }

        if (GameManager.Instance.secondaryWeapon != null)
        {
            secondaryWeaponObject = GameManager.Instance.secondaryWeapon;
            secondaryWeapon = Instantiate(secondaryWeaponObject, transform).GetComponent<Weapon>();
            secondaryIcon.enabled = true;
            secondaryIcon.sprite = secondaryWeapon.icon;
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
    }
}
