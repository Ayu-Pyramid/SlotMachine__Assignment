using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls a single reel (one column) of the slot machine.
/// Handles the spin animation and reports back the final symbols shown.
/// </summary>
public class Reel : MonoBehaviour
{
    [Header("Symbol Slots (top to bottom, visible rows)")]
    [Tooltip("The Image components representing each visible row on this reel.")]
    public Image[] symbolSlots;

    [Header("Spin Settings")]
    [Tooltip("How fast symbols cycle during the spin (seconds per symbol change).")]
    public float spinCycleInterval = 0.06f;

    [Tooltip("Minimum and maximum spin duration for randomized feel.")]
    public float minSpinDuration = 1.2f;
    public float maxSpinDuration = 2.0f;

    [Tooltip("Extra delay before this reel stops (used to stagger reels left-to-right).")]
    public float stopDelay = 0f;

    // The symbol data currently assigned to each row after the spin completes.
    private SymbolData[] currentResult;

    // Reference to the full pool of possible symbols, set by SlotMachine at spin time.
    private SymbolData[] availableSymbols;

    private Coroutine spinRoutine;
    private bool isSpinning;

    public bool IsSpinning => isSpinning;

    /// <summary>
    /// Starts the spin animation. Calls onComplete with the final symbols once stopped.
    /// </summary>
    public void StartSpin(SymbolData[] symbolPool, System.Action<SymbolData[]> onComplete)
    {
        availableSymbols = symbolPool;

        if (spinRoutine != null)
            StopCoroutine(spinRoutine);

        spinRoutine = StartCoroutine(SpinRoutine(onComplete));
    }

    private IEnumerator SpinRoutine(System.Action<SymbolData[]> onComplete)
    {
        isSpinning = true;

        // Wait for the stagger delay so reels stop left-to-right, not all at once.
        if (stopDelay > 0f)
            yield return new WaitForSeconds(0f); // delay handled by total duration below

        float totalDuration = Random.Range(minSpinDuration, maxSpinDuration) + stopDelay;
        float elapsed = 0f;

        // Rapidly cycle through random symbols to create the "spinning" visual effect.
        while (elapsed < totalDuration)
        {
            for (int i = 0; i < symbolSlots.Length; i++)
            {
                SymbolData randomSymbol = availableSymbols[Random.Range(0, availableSymbols.Length)];
                symbolSlots[i].sprite = randomSymbol.symbolSprite;
            }

            yield return new WaitForSeconds(spinCycleInterval);
            elapsed += spinCycleInterval;
        }

        // Lock in the final random result for this reel.
        currentResult = new SymbolData[symbolSlots.Length];
        for (int i = 0; i < symbolSlots.Length; i++)
        {
            SymbolData finalSymbol = availableSymbols[Random.Range(0, availableSymbols.Length)];
            currentResult[i] = finalSymbol;
            symbolSlots[i].sprite = finalSymbol.symbolSprite;
        }

        isSpinning = false;
        onComplete?.Invoke(currentResult);
    }

    /// <summary>
    /// Returns the symbol currently displayed in the middle row (the pay line).
    /// Assumes a 3-row reel where index 1 is the middle.
    /// </summary>
    public SymbolData GetMiddleSymbol()
    {
        if (currentResult == null || currentResult.Length == 0)
            return null;

        int middleIndex = currentResult.Length / 2;
        return currentResult[middleIndex];
    }

    public SymbolData[] GetFullResult() => currentResult;
}