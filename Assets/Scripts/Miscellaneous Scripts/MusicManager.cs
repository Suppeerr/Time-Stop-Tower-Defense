using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource valleyTrack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        valleyTrack.Play();
    }
}
