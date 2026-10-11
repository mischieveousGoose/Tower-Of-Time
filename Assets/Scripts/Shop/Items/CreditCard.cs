using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Items/Credit Card")]
public class CreditCard : Item, IOnBuy, IOnRoundEnd
{
    public float initialReward;
    public float costPerRound;

    public void OnBuy(ShopManager shopManager)
    {
        shopManager.time += initialReward;
    }

    public void OnRoundEnd(Player player)
    {
        player.AddTime(-costPerRound);
    }
}
