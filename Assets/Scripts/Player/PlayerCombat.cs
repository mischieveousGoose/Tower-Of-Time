using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    public GameObject primaryWeaponObject;
    public GameObject secondaryWeaponObject;

    private Weapon primaryWeapon;
    private Weapon secondaryWeapon;

    void Start()
    {
        primaryWeapon = Instantiate(primaryWeaponObject, transform.parent).GetComponent<Weapon>();
        secondaryWeapon = Instantiate(secondaryWeaponObject, transform.parent).GetComponent<Weapon>();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && primaryWeapon != null)
        {
            primaryWeapon.OnUse();
        }

        if (Mouse.current.rightButton.wasPressedThisFrame && primaryWeapon != null)
        {
            secondaryWeapon.OnUse();
        }
    }
}
