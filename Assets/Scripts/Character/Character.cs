using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Custom/Character")]
public class Character : ScriptableObject
{
    public string characterName;

    [TextArea(3, 10)]
    public string description;

    public Sprite sprite;
    public Stats stats;

    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;

}
