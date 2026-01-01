using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class PlayerPositionMark : MonoBehaviour
{
    [Header("references")]
    public GameObject visualMark;
    public InstructionsPanel instructionsPanel;

    [Header("settings")]
    public float fadeInDuration = 1f;


    private PositionMarkTriggerZone triggerZone;
    private bool playerInside = false;
    
    

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



    }

    private void Show()
    {

    }

    private void Hide()
    {
        visualMark.SetActive(false);
        instructionsPanel.Hide();
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