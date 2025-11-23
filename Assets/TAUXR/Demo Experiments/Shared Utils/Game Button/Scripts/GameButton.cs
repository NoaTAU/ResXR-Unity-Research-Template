using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class GameButton : MonoBehaviour
{


    [Header("References")]
    [SerializeField]
    private GameObject press;
    [SerializeField]
    private GameButtonCollider buttonCollider;

    [Header("Settings and public events")]
    public UnityEvent onPress;
    public UnityEvent onRelease;
    public bool showDebugLogs = false;
    public bool useSound = true;

    private Vector3 _origPosition;
    private AudioSource clickSound;
    private UniTaskCompletionSource press_tcs;

    private void Start()
    {
        _origPosition = press.transform.localPosition;
        if (useSound)
        {
            clickSound = gameObject.GetComponent<AudioSource>();
            if (clickSound == null)
            {
                Debug.LogWarning($"[GameButton] No AudioSource found on {gameObject.name} but useSound is true. Disabling sound.");
                useSound = false;
            }
        }

    }



    public virtual void whenPressed()
    {
        press_tcs?.TrySetResult();
        onPress.Invoke();

        press.transform.localPosition = new Vector3(0, 0.003f, 0);
        clickSound?.Play();

        if (showDebugLogs)
        {
            Debug.Log($"[GameButton] {gameObject.name} button was pressed");
        }

    }


    public virtual void whenReleased()
    {
        press.transform.localPosition = _origPosition;
        onRelease.Invoke();

        if (showDebugLogs)
        {
            Debug.Log($"[GameButton] {gameObject.name} button was released");
        }
    }

    public async UniTask WaitForButtonPress()
    {
        press_tcs = new UniTaskCompletionSource();

        buttonCollider.onPress += whenPressed;

        await press_tcs.Task;

        buttonCollider.onPress -= whenPressed;
    }

}
