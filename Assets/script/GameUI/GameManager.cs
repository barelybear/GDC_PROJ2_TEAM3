using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField]private Button PauseButton;
    [SerializeField]private Button MenuButton;
    [SerializeField]private Button ResumeButton;
    [SerializeField]private Button RestartButton;
    [SerializeField] private GameObject PausePanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PauseButton.onClick.AddListener(PauseGame);
        MenuButton.onClick.AddListener(ReturnMenu);
        ResumeButton.onClick.AddListener(ResumeGame);
        RestartButton.onClick.AddListener(RestartGame);
        PausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instance
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0; // Pause the game
        PausePanel.SetActive(true); // Show the pause menu
    }

    public void ResumeGame()
    {
        Time.timeScale = 1; // Resume the game
        PausePanel.SetActive(false); // Hide the pause menu
    }
    public void RestartGame()
    {
        Time.timeScale = 1; // Resume the game
        PausePanel.SetActive(false); // Hide the pause menu
        SceneManager.LoadScene("GameScene"); // Restart the game scene
    }
    public void ReturnMenu()
    {
        Time.timeScale = 1; // Resume the game
        PausePanel.SetActive(false); // Hide the pause menu
        SceneManager.LoadScene("Menu"); // Load the main menu scene
    }


}
