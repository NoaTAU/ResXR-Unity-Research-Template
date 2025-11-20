using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class InstructionsPanel : MonoBehaviour
{
    [Header("references")]
    public GameObject backPanel;
    public TextMeshPro title;
    public TextMeshPro text;

    [Header("settings")]
    public bool hideOnAwake = true;
    public bool useAnimations = true;
    public bool collectEyeGaze = true;


    private Vector3 initialScale;
    private Collider eyeGazeCollider;

    private void Awake()
    {
        initialScale = transform.localScale;

        // setup eye gaze collider
        eyeGazeCollider = backPanel.GetComponent<Collider>();

        if (collectEyeGaze && eyeGazeCollider == null)
        {
            Debug.LogWarning("[InstructionsPanel] No collider found on backPanel for eye gaze collection. Adding a BoxCollider. Please check its dimensions or add your own.");
            eyeGazeCollider = backPanel.AddComponent<BoxCollider>();
        }
        else if (!collectEyeGaze && eyeGazeCollider != null)
        {
            Destroy(eyeGazeCollider);
        }

        if (hideOnAwake)
        {
            HideInstant();
        }

    }

    public async UniTask Show()
    {
        if (useAnimations)
        {
            await AnimateShow();
        }
        else
        {
            ShowInstant();
        }
        Debug.Log($"[InstructionsPanel] {gameObject.name} was shown.");
    }

    public async UniTask Hide()
    {
        if (useAnimations)
        {
            await AnimateHide();
        }
        else
        {
            HideInstant();
        }
        Debug.Log($"[InstructionsPanel] {gameObject.name} was hidden.");
    }


    public async UniTask ShowForSeconds(float seconds)
    {
        await Show();
        await UniTask.Delay(System.TimeSpan.FromSeconds(seconds));
        await Hide();
    }

    private void ShowInstant()
    {
        backPanel.SetActive(true);
        title.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
    }

    private void HideInstant()
    {
        backPanel.SetActive(false);
        title.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
    }

    private async UniTask AnimateShow(float duration = 0.25f)
    {
        // Enable objects first (but make scale zero)
        transform.localScale = Vector3.zero;
        ShowInstant();
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            // smooth easing 
            float ease = 1f - Mathf.Pow(1f - normalized, 3f);

            transform.localScale = initialScale * ease;
            await UniTask.Yield();
        }

        transform.localScale = initialScale;
    }

    private async UniTask AnimateHide(float duration = 0.25f)
    {
        // Make sure it's visible before shrinking
        ShowInstant();

        float t = 0f;
        Vector3 startScale = initialScale;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);

            // ease-in (mirrors the ease-out of AnimateShow)
            float ease = Mathf.Pow(normalized, 3);

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, ease);

            await UniTask.Yield();
        }

        transform.localScale = Vector3.zero;

        // now hide objects
        HideInstant();
    }


}
