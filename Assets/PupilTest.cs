using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PupilTest : MonoBehaviour
{
    public float duration = 20f;
    public float colorSweepTime = 5f;
    public float wait = 10f;
    public Image img;
    public float elapsedTime;
    public float t;
    public float f;
    public Color newColor;
    public Color maxColor = new Color(1, 1, 1, 1);
    private float tim = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tim >= 0)
        {
            tim += Time.deltaTime;
            if (tim > wait)
            {
                tim = -1;
                StartCoroutine(ChangeColor());
            }
        }
    }

    IEnumerator ChangeColor()
    {
        elapsedTime = 0f;

        // Loop until the duration is reached
        while (elapsedTime < duration)
        {
            // Calculate the t value (normalized time) between 0 and 1
            t = elapsedTime / colorSweepTime;
            f = Mathf.PingPong(t, 1);

            // Interpolate the color from black (0, 0, 0) to white (1, 1, 1)
            newColor = Color.Lerp(Color.black, maxColor, f); // Color.white, f);
            img.color = newColor;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the final color is set to white
        //img.color = Color.white;
    }
}