using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameState
{
    WaitingForRoll,   // לפני זריקה
    BallRolling,      // הכדור עדיין נע
    EvaluatingRoll,   // סופרים פינים
    FrameFinished,    // מחכים להחלטה / מעבר רמה
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Frame Settings")]
    [SerializeField] private int shotsPerFrame = 2;

    public GameState CurrentState { get; private set; } = GameState.WaitingForRoll;

    public int CurrentRoll { get; private set; } = 1;
    public int TotalPins { get; private set; }
    public int PinsDownTotal { get; private set; } = 0;

    private EnemyBase[] pins;
    private UIManager uiManager;

    private void Awake()
    {
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

        // Find all pins in the scene
        pins = FindObjectsOfType<EnemyBase>();
        TotalPins = pins.Length;

        ResetFrame();
    }

    private void ResetFrame()
    {
        CurrentRoll = 1;
        PinsDownTotal = 0;

        // Reset all pins to original pose
        foreach (var pin in pins)
        {
            pin.ResetPin();
        }

        CurrentState = GameState.WaitingForRoll;
        UpdateUI();
        uiManager?.ShowMessage("Aim and press SPACE to roll.");
    }

    private void UpdateUI()
    {
        int pinsStanding = TotalPins - PinsDownTotal;
        uiManager?.UpdatePins(pinsStanding);
        uiManager?.UpdateRoll(CurrentRoll, shotsPerFrame);
    }

    public bool CanShoot()
    {
        return CurrentState == GameState.WaitingForRoll && CurrentRoll <= shotsPerFrame;
    }

    public void OnPlayerShot()
    {
        if (!CanShoot()) return;

        CurrentState = GameState.BallRolling;
        uiManager?.ClearMessage();
    }

    // Called from trigger at end of lane or from ball script when it stops moving
    public void OnBallStopped()
    {
        if (CurrentState != GameState.BallRolling) return;

        // נתחיל קורוטינה במקום לספור מיד
        StartCoroutine(EvaluatePinsAfterDelay());
    }

    private IEnumerator EvaluatePinsAfterDelay()
    {
        CurrentState = GameState.EvaluatingRoll;

        // מחכים שהפיזיקה תתייצב (תוכל לשחק עם הזמן)
        yield return new WaitForSeconds(10f);

        EvaluatePins();
    }

    private void EvaluatePins()
    {
        // Count pins that are down in this roll
        int downNow = pins.Count(p => p.IsDown && !p.RemovedThisFrame);

        // Mark them as removed from lane
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
            // Strike or spare -> win frame
            OnFrameWin();
        }
        else
        {
            // Not all pins down
            if (CurrentRoll < shotsPerFrame)
            {
                // Prepare for next roll in same frame
                CurrentRoll++;
                CurrentState = GameState.WaitingForRoll;
                uiManager?.ShowMessage("Roll again!");
                UpdateUI();
            }
            else
            {
                // No rolls left -> fail frame
                OnFrameFail();
            }
        }
    }

    private void OnFrameWin()
    {
        CurrentState = GameState.FrameFinished;
        uiManager?.ShowMessage("All pins down! Level complete.");
        uiManager?.ShowNextLevelButton(true);
    }

    private void OnFrameFail()
    {
        CurrentState = GameState.FrameFinished;
        uiManager?.ShowMessage("Try again! Not all pins were knocked down.");
        uiManager?.ShowRestartButton(true);
    }

    public void RestartLevel()
    {
        // Option A: פשוט מאפסים את הפריים באותה סצנה
        uiManager?.ShowRestartButton(false);
        ResetFrame();
    }

    public void LoadNextLevel()
    {
        // Option B: כאן אתה יכול לטעון סצנה אחרת
        // בינתיים פשוט מאפסים פריים (כאילו רמה חדשה באותו מסלול)
        uiManager?.ShowNextLevelButton(false);
        ResetFrame();
    }
}
