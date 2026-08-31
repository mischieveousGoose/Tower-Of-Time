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

    public float GetMouseAngle()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 playerPos = Camera.main.WorldToScreenPoint(transform.position);
        Vector2 direction = (mousePos - playerPos);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return angle;
    }

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
        if (InputManager.Instance.PrimaryWeaponPerformed && primaryWeapon != null)
        {
            primaryWeapon.OnUse();
        }
        else if (InputManager.Instance.PrimaryWeaponReleased && primaryWeapon != null)
        {
            primaryWeapon.ReleaseAttack();
        }

        if (InputManager.Instance.SecondaryWeaponPerformed && secondaryWeapon != null)
        {
            secondaryWeapon.OnUse();
        }
        else if (InputManager.Instance.SecondaryWeaponReleased && secondaryWeapon != null)
        {
            secondaryWeapon.ReleaseAttack();
        }
    }
}
