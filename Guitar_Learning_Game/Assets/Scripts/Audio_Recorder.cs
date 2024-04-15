using System;
using System.IO;
using UnityEngine;

public class OutputAudioRecorder : MonoBehaviour
{
    public string saveFolderPath = "Assets/Scripts/Player_Recordings"; // Default save path
    private AudioClip recordingClip;
    private bool isRecording = false; // Flag to track recording state
    private string microphoneDevice; // Name of the microphone device
    private int headerSize = 44; // Size of the WAV header
    
    void Start()
    {
        // Set the microphone to the default device
        microphoneDevice = Microphone.devices.Length > 0 ? Microphone.devices[0] : null;
        if (microphoneDevice == null)
        {
            Debug.LogError("No microphone devices found.");
            return;
        }

        // Ensure the save folder exists
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }
    }

    void Update()
    {
        // Toggle recording on 'E' key press if inside trigger area
        if (isInsideTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (!isRecording)
            {
                StartRecording();
            }
            else
            {
                StopRecording();
            }
        }
    }

    private void StartRecording()
    {
        if (microphoneDevice == null) return;
        
        if (!isRecording) // Only start recording if not already recording
        {
            isRecording = true;
            recordingClip = Microphone.Start(microphoneDevice, false, 5, 44100); // Adjust length and sample rate as needed
            Debug.Log("Recording started...");
        }
    }

    private void StopRecording()
    {
        if (microphoneDevice == null || !isRecording) return; // Only stop if currently recording
        
        Microphone.End(microphoneDevice); // Stops the recording
        isRecording = false;
        Debug.Log("Recording stopped.");
        
        SaveRecordingToFile();
    }

    private void SaveRecordingToFile()
    {
        if (recordingClip == null) return;

        // Path to save the recording with DateTime format including milliseconds
        string fileName = $"recording0.wav";
        string path = Path.Combine(saveFolderPath, fileName);
        
        // Create the file
        using (var fileStream = CreateEmptyWAV(path))
        {
            ConvertAndWrite(fileStream, recordingClip);
            WriteHeader(fileStream, recordingClip);
        }

        Debug.Log($"Recording saved to: {path}");
    }
    private FileStream CreateEmptyWAV(string filepath)
    {
        var fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < headerSize; i++) // prepare the header
        {
            fileStream.WriteByte(emptyByte);
        }

        return fileStream;
    }
    private void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);
        Int16[] intData = new Int16[samples.Length];
        // converting in 2 steps : float[] to Int16[], //then Int16[] to Byte[]
        Byte[] bytesData = new Byte[samples.Length * 2];
        // bytesData array is twice the size of
        // dataArray array because a float converted in Int16 is 2 bytes.
        int rescaleFactor = 32767; // to convert float to Int16

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        fileStream.Write(bytesData, 0, bytesData.Length);
    }
    private void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;

        fileStream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);

        // UInt16 two = 1;
        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);

        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * bytesPerSample*channels
        fileStream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(datastring, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(subChunk2, 0, 4);

        // File is now saved and can be closed
        fileStream.Close();
    }

    // Ensure to include OnTriggerEnter2D and OnTriggerExit2D if you wish to use isInsideTrigger
    private bool isInsideTrigger = false; // Example flag for trigger state
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        { 
            isInsideTrigger = true; // Set flag to true when entering trigger area
            Debug.Log("Player entered trigger area.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isInsideTrigger = false; // Set flag to false when exiting trigger area
            Debug.Log("Player left trigger area.");
        }
    }
}
