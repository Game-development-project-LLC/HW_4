using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Text Elements")]
    [SerializeField] private TextMeshProUGUI pinsText;
    [SerializeField] private TextMeshProUGUI rollText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevelButton;

    private void Awake()
    {
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartClicked);
            restartButton.gameObject.SetActive(false);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            nextLevelButton.gameObject.SetActive(false);
        }
    }

    public void UpdatePins(int pinsStanding)
    {
        if (pinsText != null)
        {
            pinsText.text = $"Pins: {pinsStanding}";
        }
    }

    public void UpdateRoll(int currentRoll, int shotsPerFrame)
    {
        if (rollText != null)
        {
            rollText.text = $"Roll: {currentRoll}/{shotsPerFrame}";
        }
    }

    public void ShowMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
        }
    }

    public void ClearMessage()
    {
        ShowMessage(string.Empty);
    }

    public void ShowRestartButton(bool show)
    {
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(show);
        }
    }

    public void ShowNextLevelButton(bool show)
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(show);
        }
    }

    private void OnRestartClicked()
    {
        GameManager.Instance?.RestartLevel();
    }

    private void OnNextLevelClicked()
    {
        GameManager.Instance?.LoadNextLevel();
    }
}
