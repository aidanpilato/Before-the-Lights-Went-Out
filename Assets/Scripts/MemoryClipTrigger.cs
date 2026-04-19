using UnityEngine;

public class MemoryClipTrigger : MonoBehaviour
{
    public AudioClip clip;
    public float length;
    public float playerWalkSpeed = 2f;
    [HideInInspector] public bool hasPlayed = false;

    private MemoryManager manager;

    void Start()
    {
        manager = FindAnyObjectByType<MemoryManager>();
        manager.RegisterClip(this);
    }

    private void OnValidate()
    {
        if (clip != null)
            length = clip.length;
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
        float distance = length * playerWalkSpeed;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, 
            transform.position + Vector3.up * 2);
        Gizmos.DrawCube(transform.position + Vector3.right * distance / 2, new Vector3(distance, 1, 1));
    }
}