
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LotteryWheel : MonoBehaviour
{
    [Header("輪盤設定")]
    public Transform wheel;
    public Button spinButton;
    public int fullRotations = 5;
    public float spinDuration = 5f;

    [Header("獎項設定（由正上方開始，順時針排列）")]
    public string[] prizes =
    {
        "一等獎", "二等獎", "三等獎", "四等獎", "五等獎",
        "六等獎", "七等獎", "八等獎", "九等獎", "十等獎"
    };

    [Header("UI 設定")]
    public TMP_Text resultText;

    [Header("音效設定")]
    public AudioSource audioSource;
    public AudioClip spinSound;
    public AudioClip stopSound;

    private bool isSpinning;
    private const float SectorAngle = 36f;

    private void Start()
    {
        if (spinButton != null)
            spinButton.onClick.AddListener(Spin);

        if (resultText != null)
            resultText.text = "點擊抽獎";

        if (prizes == null || prizes.Length != 10)
            Debug.LogWarning("請設定十個獎項！");
    }

    public void Spin()
    {
        if (isSpinning || wheel == null)
            return;

        if (prizes == null || prizes.Length != 10)
        {
            Debug.LogError("獎項必須剛好有十個！");
            return;
        }

        StartCoroutine(SpinRoutine());
    }

    private IEnumerator SpinRoutine()
    {
        isSpinning = true;

        if (spinButton != null)
            spinButton.interactable = false;

        if (resultText != null)
            resultText.text = "抽獎中...";

        // 播放旋轉音效
        if (audioSource != null && spinSound != null)
        {
            audioSource.clip = spinSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        // 隨機決定最終停下的獎項
        int prizeIndex = Random.Range(0, prizes.Length);

        float startAngle = wheel.eulerAngles.z;
        float normalizedAngle = Mathf.Repeat(startAngle, 360f);

        // 讓指定獎項的中心對準正上方指針
        float targetCenter = prizeIndex * SectorAngle;
        float extraAngle = Mathf.Repeat(
            targetCenter - normalizedAngle, 360f);

        float totalRotation =
            fullRotations * 360f + extraAngle;

        float elapsed = 0f;

        // EaseInOut：先加速，再減速至停止
        while (elapsed < spinDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / spinDuration);
            float easedT = Mathf.SmoothStep(0f, 1f, t);

            float angle = startAngle + totalRotation * easedT;

            wheel.rotation = Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        wheel.rotation = Quaternion.Euler(
            0f, 0f, startAngle + totalRotation);

        // 停止旋轉音效並播放結果音效
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.loop = false;

            if (stopSound != null)
                audioSource.PlayOneShot(stopSound);
        }

        // 依照輪盤實際角度判斷正上方的獎項
        float finalAngle = Mathf.Repeat(
            wheel.eulerAngles.z, 360f);

        int actualIndex = Mathf.FloorToInt(
            Mathf.Repeat(finalAngle + SectorAngle / 2f, 360f)
            / SectorAngle);

        actualIndex = Mathf.Clamp(actualIndex, 0, prizes.Length - 1);

        if (resultText != null)
            resultText.text = "恭喜抽中：" + prizes[actualIndex];

        Debug.Log("抽獎結果：" + prizes[actualIndex]);

        isSpinning = false;

        if (spinButton != null)
            spinButton.interactable = true;
    }
}
