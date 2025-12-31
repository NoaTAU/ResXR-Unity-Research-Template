using UnityEngine;

public class FollowPlayerHead : MonoBehaviour
{
    Transform playerHead;
    void Start()
    {
        playerHead = TXRPlayer.Instance.PlayerHead;
    }

    
    void Update()
    {
        transform.position = playerHead.position;
        transform.rotation = playerHead.rotation;
    }
}
