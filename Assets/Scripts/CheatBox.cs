using UnityEngine;

public class CheatBox : MonoBehaviour
{
    public GameObject player;
    private BoxCollider boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Cheat box triggered by: " + other.gameObject.name);
        if (other.GetComponent<CharacterController>() != null)
        {
            Debug.Log("Player entered cheat box, teleporting...");
            player.transform.position += new Vector3(350, 0, 0);
        }  
    }

    void OnDrawGizmos()
    {
        // Draw a wire cube to visualize the box collider
        Gizmos.color = Color.green;
        if (boxCollider != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
    }
}
