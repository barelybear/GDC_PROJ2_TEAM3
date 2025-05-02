using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

public class WinTrigger : MonoBehaviour
{
    [SerializeField] private GameObject winPanel; // UI to show when player wins
    [SerializeField] private Button restartButton; // Button to restart the game
    [SerializeField] private Button menuButton; // Button to return to the main menu
    private bool isGameWon = false; // Flag to check if the game is won

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winPanel.SetActive(false); // Hide the win panel at the start
        restartButton.onClick.AddListener(RestartGame); // Add listener to restart button
        menuButton.onClick.AddListener(ReturnMenu); // Add listener to menu button
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return; // Check if the collider is the player

        Entity target = other.GetComponent<Entity>();
        if (target != null && !isGameWon) // Check if the target is valid and game is not already won
        {
            Debug.Log("VICTORY"); // Log victory message
            isGameWon = true; // Set game won flag to true
            winPanel.SetActive(true); // Show the win panel
            Time.timeScale = 0; // Pause the game
        }
    }
    public void RestartGame()
    {
        Time.timeScale = 1; // Resume the game
        winPanel.SetActive(false); // Hide the pause menu
        SceneManager.LoadScene("GameScene"); // Restart the game scene
    }
    public void ReturnMenu()
    {
        Time.timeScale = 1; // Resume the game
        winPanel.SetActive(false); // Hide the pause menu
        SceneManager.LoadScene("Menu"); // Load the main menu scene
    }
}
