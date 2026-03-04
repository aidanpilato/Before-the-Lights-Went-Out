using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MemoryManager : MonoBehaviour
{
    private List<MemoryClipTrigger> clips = new List<MemoryClipTrigger>();

    private AudioSource audioSource;
    private bool isPlaying = false;
    private int currentIndex = 0;

    public Transform player;

    void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    public void RegisterClip(MemoryClipTrigger clip)
    {
        clips.Add(clip);

        // Sort by X position automatically
        clips.Sort((a, b) => 
            a.transform.position.x.CompareTo(b.transform.position.x));
    }

    void Update()
    {
        if (currentIndex >= clips.Count)
            return;

        clips[currentIndex].TryTrigger(player.position.x);
    }

    public void RequestPlay(MemoryClipTrigger clip)
    {
        if (isPlaying) return;

        StartCoroutine(PlayClipCoroutine(clip));
    }

    private IEnumerator PlayClipCoroutine(MemoryClipTrigger clip)
    {
        isPlaying = true;
        clip.hasPlayed = true;

        audioSource.clip = clip.clip;
        audioSource.Play();

        yield return new WaitUntil(() => !audioSource.isPlaying);

        isPlaying = false;
        currentIndex++;
    }
}