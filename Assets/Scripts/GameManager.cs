using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    // Singleton instance so any script can access it easily (GameManager.Instance)
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int currentLoop = 0;
    public bool isCurrentPuzzleSolved = false;

    // Events that other scripts (like the Clock) can listen to
    public event Action OnLoopAdvanced;
    public event Action OnLoopReset;

    private void Awake()
    {
        // Setup Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void AdvanceLoop()
    {
        currentLoop++;
        isCurrentPuzzleSolved = false; // Reset the puzzle state for the next floor
        Debug.Log("Loop Advanced! Welcome to Loop " + currentLoop);

        // Tell the clock to update!
        OnLoopAdvanced?.Invoke();
    }

    public void ResetLoop()
    {
        currentLoop = 0;
        isCurrentPuzzleSolved = false;
        Debug.Log("Punishment! Reset back to Loop 0");

        // Tell the clock to go back to 1:01!
        OnLoopReset?.Invoke();
    }
}