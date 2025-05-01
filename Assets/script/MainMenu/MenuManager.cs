using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Button startButton;
    [SerializeField]  private Button guideButton;
    [SerializeField] private GameObject GuidePanel;
    [SerializeField] private Button continueButton;
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        guideButton.onClick.AddListener(OpenGuidePanel);
        continueButton.onClick.AddListener(ContinueGame);
        GuidePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenGuidePanel()
    {
        bool isActive = GuidePanel.activeSelf;
        GuidePanel.SetActive(!isActive);
    }

    public void ContinueGame()
    {
        Debug.Log("Continue Game button clicked. Implement your logic here.");
    }
    
}
