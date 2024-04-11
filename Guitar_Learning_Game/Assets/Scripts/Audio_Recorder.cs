using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class OutputAudioRecorder : MonoBehaviour
{
    internal string FILENAME;
    private int outputRate;
    private int headerSize = 44; // Default for uncompressed wav
    private string fileName;
    private bool recOutput = false;
    private FileStream fileStream;
    private bool isInsideTrigger = false; // Track if inside trigger area

    void Awake()
    {
        outputRate = AudioSettings.outputSampleRate;
    }

    void Update()
    {
        // Toggle recording on 'E' key press if inside trigger area
        if (isInsideTrigger && Input.GetKeyDown(KeyCode.E))
        {
            ToggleRecording();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        isInsideTrigger = true; // Set flag to true when entering trigger area
        Debug.Log("in area");
    }

    void OnTriggerExit(Collider other)
    {
        isInsideTrigger = false; // Reset flag when exiting trigger area
        Debug.Log("out area");
    }

    private void ToggleRecording()
    {
        if (!recOutput)
        {
            FILENAME = "record " + UnityEngine.Random.Range(1, 1000).ToString();
            fileName = Path.GetFileNameWithoutExtension(FILENAME) + ".wav";
            StartWriting(fileName);
            recOutput = true;
            Debug.Log("Start Recording");
        }
        else
        {
            recOutput = false;
            WriteHeader();
            Debug.Log("Stop Recording");
        }
    }

    private void StartWriting(string name)
    {
        fileStream = new FileStream(Application.persistentDataPath + "/" + name, FileMode.Create);
        byte emptyByte = new byte();
        for (int i = 0; i < headerSize; i++) // Preparing the header
        {
            fileStream.WriteByte(emptyByte);
        }
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (recOutput)
        {
            ConvertAndWrite(data); // Audio data is interlaced
        }
    }

    private void WriteHeader()
    {
        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);
        byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);
        byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);
        byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);
        byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);
        ushort two = 2;
        ushort one = 1;
        byte[] audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);
        byte[] numChannels = BitConverter.GetBytes(two);
        fileStream.Write(numChannels, 0, 2);
        byte[] sampleRate = BitConverter.GetBytes(outputRate);
        fileStream.Write(sampleRate, 0, 4);
        byte[] byteRate = BitConverter.GetBytes(outputRate * 4); // SampleRate * BytesPerSample*Channels
        fileStream.Write(byteRate, 0, 4);
        ushort four = 4;
        byte[] blockAlign = BitConverter.GetBytes(four);
        fileStream.Write(blockAlign, 0, 2);
        ushort sixteen = 16;
        byte[] bitsPerSample = BitConverter.GetBytes(sixteen);
        fileStream.Write(bitsPerSample, 0, 2);
        byte[] dataString = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(dataString, 0, 4);
        byte[] subChunk2 = BitConverter.GetBytes(fileStream.Length - headerSize);
        fileStream.Write(subChunk2, 0, 4);
        fileStream.Close();
    }

    private void ConvertAndWrite(float[] dataSource)
    {
        Int16[] intData = new Int16[dataSource.Length];
        Byte[] bytesData = new Byte[dataSource.Length * 2]; // Each Int16 takes 2 bytes
        int rescaleFactor = 32767; // to convert float to Int16
        for (int i = 0; i < dataSource.Length; i++)
        {
            intData[i] = (Int16)(dataSource[i] * rescaleFactor);
            Byte[] byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }
        fileStream.Write(bytesData, 0, bytesData.Length);
    }
}
