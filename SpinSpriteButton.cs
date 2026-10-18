
using UnityEngine;

public class SpinSpriteButton : MonoBehaviour
{
    public LotteryWheel lotteryWheel;

    private void OnMouseDown()
    {
        if (lotteryWheel != null)
        {
            lotteryWheel.Spin();
        }
        else
        {
            Debug.LogWarning("請指定 Lottery Wheel！");
        }
    }
}
