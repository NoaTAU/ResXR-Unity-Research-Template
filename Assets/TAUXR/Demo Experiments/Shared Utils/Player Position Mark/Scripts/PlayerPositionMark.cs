using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class PlayerPositionMark : MonoBehaviour
{
    public GameObject visualMark;
    private PositionMarkTriggerZone triggerZone;
    private bool playerInside = false;
    public float fadeInDuration = 1f;

    private void Awake()
    {
        triggerZone = GetComponentInChildren<PositionMarkTriggerZone>();
        if (triggerZone == null)
        {
            Debug.LogError("PositionMarkTriggerZone not found in children.");
        }
    }

    private void OnEnable()
    {
        if (triggerZone != null)
        {
            triggerZone.OnPlayerEnter += HandlePlayerEnter;
        }
    }

    private void OnDisable()
    {
        if (triggerZone != null)
        {
            triggerZone.OnPlayerEnter -= HandlePlayerEnter;
        }
    }

    private void HandlePlayerEnter(GameObject player)
    {
        Debug.Log("[PlayerPositionMark] Player entered position marker");
        playerInside = true;

        visualMark.SetActive(false);
    }

    public async UniTask WaitForPlayerAsync(bool fadeToblack = true, CancellationToken cancellationToken = default)
    {
        if (fadeToblack)
        {
            await TXRPlayer.Instance.FadeViewToColor(Color.black, 0.0f);
        }

        Debug.Log("[PlayerPositionMark] Waiting for player to enter position marker...");

        await UniTask.WaitUntil(() => playerInside, cancellationToken: cancellationToken);

        if (fadeToblack)
        {
            await TXRPlayer.Instance.FadeViewToColor(Color.clear, fadeInDuration);
        }

    }
}