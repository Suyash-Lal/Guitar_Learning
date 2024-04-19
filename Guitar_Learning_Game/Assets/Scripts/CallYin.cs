using System.Diagnostics;
using UnityEngine;
using System.IO;

public class CallYin : MonoBehaviour
{
    private bool isPlayerInTrigger = false; // Flag to check if player is within the trigger area

    // Method to run the Python script
    public void RunPythonScript(string filePath)
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = "python"; // Use "python3" on macOS or Linux if needed
        start.Arguments = $"\"C:\\GitHub Repos\\Thesis-Guitar_Project\\Guitar_Learning\\Guitar_Learning_Game\\Assets\\Scripts\\YIN_Suyash.py\" \"{filePath}\""; // Adjust the path and pass arguments
        start.UseShellExecute = false; // Do not use the system shell to start the process
        start.RedirectStandardOutput = true; // Allows you to read output
        start.RedirectStandardError = true; // Redirects error output

        using(Process process = Process.Start(start))
        {
            using(StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd(); // Reads the output to a string
                UnityEngine.Debug.Log(result); // Logs output in Unity console
            }

            using(StreamReader reader = process.StandardError)
            {
                string error = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(error))
                    UnityEngine.Debug.LogError(error);
            }
        }
    }

    void Update()
    {
        if (isPlayerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            // You may need to adjust this file path or how it's passed.
            RunPythonScript("YIN_Suyash.py");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Ensure the collider has the tag "Player"
        {
            isPlayerInTrigger = true;
            UnityEngine.Debug.Log("Player entered trigger");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            UnityEngine.Debug.Log("Player left trigger");
        }
    }
}
