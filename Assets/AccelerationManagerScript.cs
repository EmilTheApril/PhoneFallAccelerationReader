using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class AccelerationManagerScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI startButtonText;
    [SerializeField] private TextMeshProUGUI textOutput;
    [SerializeField] private bool isReading;
    private bool takingSamples;
    public int samplesPerSecond;
    private int numberOfTests;

    private string filename;

    [SerializeField] private List<Vector3> samples;

    private void Start()
    {
        //Sets the filename/path
        filename = "./data.csv";
    }

    private void Update()
    {
        ReadAcceleration();
    }

    public void StartStopReading()
    {
        if (isReading)
        {
            isReading = false;
            takingSamples = false;
            startButtonText.color = Color.green;
            CancelInvoke();
            CleanList();
            WriteCSV();
            samples.Clear();
        }
        else
        {
            isReading = true;
            startButtonText.color = Color.red;
            numberOfTests++;
        }
    }

    private void ReadAcceleration()
    {
        if (!isReading) return;
        textOutput.text = $"Acceleration:\nx: {Input.acceleration.x.ToString("0.000")}\ny: {Input.acceleration.y.ToString("0.000")}\nz: {Input.acceleration.z.ToString("0.000")}";
        if (Input.acceleration.y > -0.95 && !takingSamples)
        {
            InvokeRepeating("TakeSample", 1f / samplesPerSecond, 1f / samplesPerSecond);
        }
    }

    private void TakeSample()
    {
        takingSamples = true;

        samples.Add(Input.acceleration);

        if (Input.acceleration.y > 0)
        {
            StartStopReading();
        }
    }

    private void CleanList()
    {
        for (int i = 1; i < samples.Count; i++)
        {
            if (samples[i - 1] == samples[i])
            {
                samples.RemoveAt(i);
                CleanList();
                return;
            }
        }
        samples.RemoveAt(samples.Count - 1);
    }

    //Writes the data and saves it to the computer.
    public void WriteCSV()
    {
        //We open a TextWriter so we can start writing data that needs to be saved.
        TextWriter tw = new StreamWriter(filename, true);

        //Test number
        tw.WriteLine("Test " + numberOfTests);

        //Headlines
        tw.WriteLine("x, y, z");

        //Results
        foreach (Vector3 vec in samples)
        {
            tw.WriteLine($"{vec.x}, {vec.y}, {vec.z}");
        }

        //Space
        tw.WriteLine("");

        //Closed the TextWriter and saves the data as a file
        tw.Close();
    }
}
