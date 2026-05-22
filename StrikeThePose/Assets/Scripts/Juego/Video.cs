using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine;

public class Video : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage rawImage;

    private void Start()
    {
        videoPlayer.isLooping = true;
        videoPlayer.Play();
    }

    private void Update()
    {
        if (videoPlayer.texture != null)
            rawImage.texture = videoPlayer.texture;
    }
}
