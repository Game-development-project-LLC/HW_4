using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// High-level game state for a single bowling frame.
/// </summary>
public enum GameState
{
    /// <summary>Waiting for the player to roll the ball.</summary>
    WaitingForRoll,

    /// <summary>The ball has been thrown and is still moving.</summary>
    BallRolling,

    /// <summary>We are currently counting fallen pins after the ball stopped.</summary>
    EvaluatingRoll,

    /// <summary>The frame has ended (either success or failure).</summary>
    FrameFinished,
}

/// <summary>
/// Central controller that manages rolls, pin evaluation and UI updates.
/// Implemented as a simple singleton.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Frame Settings")]
    [Tooltip("How many rolls the player has per frame.")]
    [SerializeField] private int shotsPerFrame = 2;

    public GameState CurrentState { get; private set; } = GameState.WaitingForRoll;

    public int CurrentRoll { get; private set; } = 1;
    public int TotalPins { get; private set; }
    public int PinsDownTotal { get; private set; } = 0;

    private EnemyBase[] pins;
    private UIManager uiManager;

    private void Awake()
    {
        // Simple singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();

        // Find all pins in the scene (all EnemyBase components)
        pins = FindObjectsOfType<EnemyBase>();
        TotalPins = pins.Length;

        ResetFrame();
    }

    /// <summary>
    /// Reset the state of the current frame and all pins.
    /// </summary>
    private void ResetFrame()
    {
        CurrentRoll = 1;
        PinsDownTotal = 0;

        // Reset all pins to their original pose
        foreach (var pin in pins)
        {
            pin.ResetPin();
        }

        CurrentState = GameState.WaitingForRoll;
        UpdateUI();
        uiManager?.ShowMessage("Aim and press SPACE to roll.");
    }

    /// <summary>
    /// Update UI texts for pin count and roll number.
    /// </summary>
    private void UpdateUI()
    {
        int pinsStanding = TotalPins - PinsDownTotal;
        uiManager?.UpdatePins(pinsStanding);
        uiManager?.UpdateRoll(CurrentRoll, shotsPerFrame);
    }

    /// <summary>
    /// Returns true if the player is currently allowed to shoot.
    /// </summary>
    public bool CanShoot()
    {
        return CurrentState == GameState.WaitingForRoll && CurrentRoll <= shotsPerFrame;
    }

    /// <summary>
    /// Called by PlayerController right after the ball has been thrown.
    /// </summary>
    public void OnPlayerShot()
    {
        if (!CanShoot()) return;

        CurrentState = GameState.BallRolling;
        uiManager?.ClearMessage();
    }

    /// <summary>
    /// Called when the ball reaches the end trigger or otherwise stops moving.
    /// Starts a coroutine to evaluate pins after a short delay.
    /// </summary>
    public void OnBallStopped()
    {
        if (CurrentState != GameState.BallRolling) return;

        StartCoroutine(EvaluatePinsAfterDelay());
    }

    /// <summary>
    /// Wait a bit for physics to settle before counting which pins have fallen.
    /// </summary>
    private IEnumerator EvaluatePinsAfterDelay()
    {
        CurrentState = GameState.EvaluatingRoll;

        // Wait for the pins to finish moving (tweak time as needed)
        yield return new WaitForSeconds(10f);

        EvaluatePins();
    }

    /// <summary>
    /// Count fallen pins, update totals, and decide whether the frame is won,
    /// lost, or if the player gets another roll.
    /// </summary>
    private void EvaluatePins()
    {
        // Count pins that are down in this roll and not yet removed
        int downNow = pins.Count(p => p.IsDown && !p.RemovedThisFrame);

        // Mark these pins as removed from the lane
        foreach (var pin in pins)
        {
            if (pin.IsDown && !pin.RemovedThisFrame)
            {
                pin.RemoveFromLane();
            }
        }

        PinsDownTotal += downNow;
        int pinsStanding = TotalPins - PinsDownTotal;

        UpdateUI();

        if (pinsStanding == 0)
        {
            // All pins are down -> frame success (strike/spare)
            OnFrameWin();
        }
        else
        {
            // Not all pins are down
            if (CurrentRoll < shotsPerFrame)
            {
                // Prepare for next roll in the same frame
                CurrentRoll++;
                CurrentState = GameState.WaitingForRoll;
                uiManager?.ShowMessage("Roll again!");
                UpdateUI();
            }
            else
            {
                // No rolls left -> frame failure
                OnFrameFail();
            }
        }
    }

    /// <summary>
    /// Called when the player has knocked down all pins in this frame.
    /// </summary>
    private void OnFrameWin()
    {
        CurrentState = GameState.FrameFinished;
        uiManager?.ShowMessage("All pins down! Level complete.");
        uiManager?.ShowNextLevelButton(true);
    }

    /// <summary>
    /// Called when the player has no rolls left and some pins are still standing.
    /// </summary>
    private void OnFrameFail()
    {
        CurrentState = GameState.FrameFinished;
        uiManager?.ShowMessage("Try again! Not all pins were knocked down.");
        uiManager?.ShowRestartButton(true);
    }

    /// <summary>
    /// Reset the current level/frame without reloading the scene.
    /// </summary>
    public void RestartLevel()
    {
        uiManager?.ShowRestartButton(false);
        ResetFrame();
    }

    /// <summary>
    /// Load the next level or, in this simple project, just reset the frame again.
    /// </summary>
    public void LoadNextLevel()
    {
        // For now we treat this as a new frame on the same lane.
        uiManager?.ShowNextLevelButton(false);
        ResetFrame();
    }
}
