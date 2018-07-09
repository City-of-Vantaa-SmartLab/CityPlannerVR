using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculate the average amplitude of a voice comment and adjust it if necessary
/// This was used as a reference: http://teropa.info/blog/2016/08/30/amplitude-and-loudness.html
/// </summary>

public static class MonitorCommentVolume
{ 
    //the lowest amplitude that doesn't need to be changed
    private const float minAmp = 0.01f;
    //the highest amplitude that doesn't need to be changed
    private const float maxAmp = 0.1f;

    public static float[] CalculateAmplitudeMultiplier(float[] samples)
    {
        //Sum of every float in samples
        float sum = 0;
        //the average of floats in samples
        float average = 0;

        //Calculate the average amplitude
        for (int i = 0; i < samples.Length; i++)
        {
            sum += Mathf.Abs(samples[i]);
        }

        average = sum / samples.Length;

        //Break the average to a char array and convert to int array, so we can examine single digits of the average separately
        char[] averageChars = average.ToString().ToCharArray();
        
        List<int> digits = new List<int>();

        for (int i = 0; i < averageChars.Length; i++)
        {
            if(averageChars[i] != '.')
            {
                digits.Add((int)char.GetNumericValue(averageChars[i]));
            }
        }

        //Remove the first digit if it's 0 because we don't need that
        if(digits[0] == 0)
        {
            digits.Remove(digits[0]); 
        }


        float multiplier = 1;
        //Amount of 0 between decimal point and the next number other than 0
        int amountOfZeros = 0;

        float[] newSamples;

        //Calculate multiplier
        if(average < minAmp)
        {
            for (int i = 0; i < digits.Count; i++)
            {
                if(digits[i] == 0)
                {
                    amountOfZeros++;
                }
                else
                {
                    multiplier = Mathf.Pow(10, amountOfZeros - 1);

                    newSamples = ChangeAmplitude(samples, multiplier);

                    return newSamples;
                }
            }
        }
        else if(average > maxAmp)
        {
            multiplier = 0.1f;

            newSamples = ChangeAmplitude(samples, multiplier);

            return newSamples;
        }

        //If the amplitude was already good, we can just return the old samples
        return samples;
    }

    private static float[] ChangeAmplitude(float[] samples, float multiplier)
    {
        float[] newSamples = new float[samples.Length];

        for (int i = 0; i < samples.Length; i++)
        {
            newSamples[i] = samples[i] * multiplier;
        }

        return newSamples;
    }
}
