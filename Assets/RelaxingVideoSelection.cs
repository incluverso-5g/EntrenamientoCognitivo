using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class RelaxingVideoSelection : MonoBehaviour
{
    private GameObject waveRig;
    VariablesNiveles variablesNiveles;

    public VideoClip playa;
    public VideoClip montana;

    public VideoPlayer videoPlayer;

    void Start()
    {
        waveRig = GameObject.Find("Wave Rig");
        variablesNiveles = waveRig.GetComponent<VariablesNiveles>();

        videoPlayer.SetDirectAudioMute(0, false);

        if (variablesNiveles.videoRelax == "Playa.mp4")
        {
            videoPlayer.clip = playa;
        }
        else if (variablesNiveles.videoRelax == "Montana.mp4")
        {
            videoPlayer.clip = montana;
        }

        videoPlayer.Play();
    }
}
