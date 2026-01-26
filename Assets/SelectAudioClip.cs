using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAudioClip : MonoBehaviour
{
    public AudioSource audioSource;

    public bool playAudio;

    public AudioClip Instrucciones_caf_entrenamiento;
    public AudioClip Instrucciones_caf_tarea_1;
    public AudioClip Instrucciones_sup_entrenamiento;
    public AudioClip andaHaciaAdelante;
    public AudioClip avanza;
    public AudioClip miraArriba;
    public AudioClip miraAbajo;
    public AudioClip miraIzquierda;
    public AudioClip miraDerecha;
    public AudioClip miraDetras;
    public AudioClip Instrucciones_caf_tarea_2_largas;
    public AudioClip Instrucciones_caf_tarea_2_cortas;
    public AudioClip Instrucciones_sup_tarea_2_largas;
    public AudioClip Instrucciones_sup_tarea_2_cortas;

    public bool lanzaInstruccionesCafT1 = false;

    void Start()
    {
        playAudio = false;
    }

    public void Select_And_Play_Audio(string audioName)
    {
        if (playAudio)
        {
            AudioClipSelect(audioName);
            Debug.Log("AudioClip" + audioSource.clip.name);
            audioSource.Play();
            Debug.Log("Playing");

            StartCoroutine(StopAudioWhenFinished(audioSource, audioSource.clip.length));
            //Debug.Log("AudioClip is: " + audioSource.clip.name);
        }
    }

    private void AudioClipSelect(string clip)
    {
        if (clip.Contains("arriba"))
        {
            audioSource.clip = miraArriba;
        }
        else if (clip.Contains("abajo"))
        {
            audioSource.clip = miraAbajo;
        }
        else if (clip.Contains("derecha"))
        {
            audioSource.clip = miraDerecha;
        }
        else if (clip.Contains("izquierda"))
        {
            audioSource.clip = miraIzquierda;
        }
        else if (clip.Contains("detras"))
        {
            audioSource.clip = miraDetras;
        }
        else if (clip.Contains("cafeteriaentrenamiento"))
        {
            audioSource.clip = Instrucciones_caf_entrenamiento;
        }
        else if (clip.Contains("cafeteriatarea1"))
        {
            audioSource.clip = Instrucciones_caf_tarea_1;
        }
        else if (clip.Contains("cafeteriatarea2larga"))
        {
            audioSource.clip = Instrucciones_caf_tarea_2_largas;
        }
        else if (clip.Contains("cafeteriatarea2corta"))
        {
            audioSource.clip = Instrucciones_caf_tarea_2_cortas;
        }
        else if (clip.Contains("supermercadoentrenamiento"))
        {
            audioSource.clip = Instrucciones_sup_entrenamiento;
        }
        else if (clip.Contains("supermercadotarea2larga"))
        {
            audioSource.clip = Instrucciones_sup_tarea_2_largas;
        }
        else if (clip.Contains("supermercadotarea2corta"))
        {
            audioSource.clip = Instrucciones_sup_tarea_2_cortas;
        }
        else if (clip.Contains("andahaciadelante"))
        {
            audioSource.clip = andaHaciaAdelante;
        }
        else if (clip.Contains("avanza"))
        {
            audioSource.clip = avanza;
        }
    }

    private IEnumerator StopAudioWhenFinished(AudioSource audioSource, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);

        if (audioSource.clip.name.Contains("Instrucciones_Entrenamiento_Cafeteria"))
        {
            lanzaInstruccionesCafT1 = true;
        }

        audioSource.Stop();
    }
}
