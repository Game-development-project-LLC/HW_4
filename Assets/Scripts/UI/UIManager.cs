using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Handles all UI elements for the bowling game:
/// pins counter, roll counter, instruction messages and buttons.
/// </summary>
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
        // Initialize buttons and hide them by default
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

    /// <summary>
    /// Update the text that shows how many pins are still standing.
    /// </summary>
    public void UpdatePins(int pinsStanding)
    {
        if (pinsText != null)
        {
            pinsText.text = $"Pins: {pinsStanding}";
        }
    }

    /// <summary>
    /// Update the text that shows the current roll out of the total allowed shots.
    /// </summary>
    public void UpdateRoll(int currentRoll, int shotsPerFrame)
    {
        if (rollText != null)
        {
            rollText.text = $"Roll: {currentRoll}/{shotsPerFrame}";
        }
    }

    /// <summary>
    /// Display a status/instruction message to the player.
    /// </summary>
    public void ShowMessage(string msg)
    {
        if (messageText != null)
        {
            messageText.text = msg;
        }
    }

    /// <summary>
    /// Clear any currently shown message.
    /// </summary>
    public void ClearMessage()
    {
        ShowMessage(string.Empty);
    }

    /// <summary>
    /// Show or hide the restart button.
    /// </summary>
    public void ShowRestartButton(bool show)
    {
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(show);
        }
    }

    /// <summary>
    /// Show or hide the next-level button.
    /// </summary>
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
