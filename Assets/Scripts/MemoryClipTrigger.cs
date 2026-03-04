using UnityEngine;

public class MemoryClipTrigger : MonoBehaviour
{
    public AudioClip clip;
    [HideInInspector] public bool hasPlayed = false;

    private MemoryManager manager;

    void Start()
    {
        manager = FindAnyObjectByType<MemoryManager>();
        manager.RegisterClip(this);
    }

    public void TryTrigger(float playerX)
    {
        if (!hasPlayed && playerX >= transform.position.x)
        {
            manager.RequestPlay(this);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, 
            transform.position + Vector3.up * 2);
    }
}