using TMPro;
using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    [SerializeField] private DamagePopup popupPrefab;
    [SerializeField] private Transform popupParent;

    public void SpawnPopup(Vector3 worldPosition, string text)
    {
        DamagePopup popup = Instantiate(popupPrefab, worldPosition, Quaternion.identity, popupParent);
        popup.SetText(text);
    }

    public void SpawnPopup(Vector3 worldPosition, string text, Color color)
    {
        DamagePopup popup = Instantiate(popupPrefab, worldPosition, Quaternion.identity, popupParent);
        popup.SetText(text);
        popup.SetColor(color);
    }
}