using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Items/Weapon Item")]
public class WeaponItem : Item, IOnBuy
{
    public GameObject weaponPrefab;

    public void OnBuy(ShopManager shopManager)
    {
        shopManager.AddWeapon(weaponPrefab);
    }
}
