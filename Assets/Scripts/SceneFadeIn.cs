using UnityEngine;

public class SceneFadeIn : MonoBehaviour
{
    public float transitionDuration = 3f;

    void Start()
    {
        TransitionController.Instance.StartFadeIn(transitionDuration);
    }
}