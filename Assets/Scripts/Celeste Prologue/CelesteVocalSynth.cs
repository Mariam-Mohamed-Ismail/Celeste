using UnityEngine;

public class CelesteVocalSynth : MonoBehaviour
{
    public static AudioClip GenerateBeep()
    {
        int samplerate = 44100;
        float frequency = 440;
        float duration = 0.05f;
        int samplesCount = (int)(samplerate * duration);
        float[] samples = new float[samplesCount];

        for (int i = 0; i < samplesCount; i++)
        {
            float t = (float)i / samplerate;
            // Sine wave with a quick fade out to prevent clicks
            float fade = 1.0f - ((float)i / samplesCount);
            samples[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * 0.5f * fade;
        }

        AudioClip clip = AudioClip.Create("VocalBlip", samplesCount, 1, samplerate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
