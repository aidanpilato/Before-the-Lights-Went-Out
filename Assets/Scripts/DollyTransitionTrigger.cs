using Cinemachine;
using UnityEngine;

public class DollyTransitionTrigger : MonoBehaviour
{
    public CinemachineVirtualCamera gameplayCam;
    public CinemachineVirtualCamera dollyCam;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        gameplayCam.Priority = 5;
        dollyCam.Priority = 20;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        gameplayCam.Priority = 20;
        dollyCam.Priority = 5;
    }
}
