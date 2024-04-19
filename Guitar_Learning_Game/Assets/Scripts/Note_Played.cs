using System.IO;
using UnityEngine;

public class Note_Played : MonoBehaviour
{
    private string filePath = "Assets/Scripts/closest_note.txt"; // Adjust the path to your closest_note.txt file
    private bool isPlayerInRange = false; // Flag to check if the player is in the trigger area

    void Update()
    {
        // Check if the 'E' key is pressed and the player is within the trigger area
        if (Input.GetKeyDown(KeyCode.E) && isPlayerInRange)
        {
            ReadAndPrintNote();
        }
    }

    // Method to read the note from the file and print it
    void ReadAndPrintNote()
    {
        try
        {
            // Read the first line from the text file which contains the note
            string readNote = File.ReadAllText(filePath).Trim();

            // Log the read note to the Unity Console
            Debug.Log("Read note: " + readNote);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error reading the note file: " + ex.Message);
        }
    }

    // This method is called when any other collider enters the trigger collider attached to this GameObject
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  // Make sure the Player GameObject has the tag "Player"
        {
            isPlayerInRange = true;  // Set isPlayerInRange to true when player enters the trigger
        }
    }

    // This method is called when the collider exits the trigger collider
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false; // Set isPlayerInRange to false when player exits the trigger
        }
    }
}
