using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class SimpleFootIK : MonoBehaviour
{
    [Header("IK Settings")]
    [SerializeField] float raycastDistance = 1.5f;
    [SerializeField] float footOffset = 0.02f;

    [Header("Slope Settings")]
    [SerializeField] float minSlopeForIK = 2f;    // ignore very flat surfaces
    [SerializeField] float maxSlopeForIK = 45f;   // ignore too-steep surfaces
    [SerializeField] float maxIKWeight = 1f;

    Animator animator;
    CharacterController controller;

    void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (!animator) return;

        HandleFootIK(AvatarIKGoal.LeftFoot);
        HandleFootIK(AvatarIKGoal.RightFoot);
    }

    void HandleFootIK(AvatarIKGoal foot)
    {
        Vector3 footPos = animator.GetIKPosition(foot);
        Vector3 rayOrigin = footPos + Vector3.up;

        if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastDistance))
        {
            animator.SetIKPositionWeight(foot, 0f);
            animator.SetIKRotationWeight(foot, 0f);
            return;
        }

        float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

        // Only apply IK on moderate slopes
        if (slopeAngle < minSlopeForIK || slopeAngle > maxSlopeForIK)
        {
            animator.SetIKPositionWeight(foot, 0f);
            animator.SetIKRotationWeight(foot, 0f);
            return;
        }

        float ikWeight = maxIKWeight;

        animator.SetIKPositionWeight(foot, ikWeight);
        animator.SetIKRotationWeight(foot, ikWeight);

        // Position the foot slightly above the ground
        animator.SetIKPosition(foot, hit.point + Vector3.up * footOffset);

        // Rotate foot to match slope
        Quaternion footRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) *
                                  Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        animator.SetIKRotation(foot, footRotation);
    }
}
