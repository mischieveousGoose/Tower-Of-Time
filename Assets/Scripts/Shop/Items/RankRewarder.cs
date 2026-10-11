using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Items/RankRewarder")]
public class RankRewarder : Item, IOnRoundEnd
{
    public Rank targetRank;
    public float timeReward;

    public void OnRoundEnd(Player player)
    {
        if (player.rank == targetRank)
            player.AddTime(timeReward);
    }
}
