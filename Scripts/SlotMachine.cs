using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Core game manager for the slot machine.
/// Orchestrates spinning all reels, determines win/loss, and triggers payouts.
/// </summary>
public class SlotMachine : MonoBehaviour
{
    [Header("Reel References (left to right)")]
    public Reel[] reels;

    [Header("Symbol Pool")]
    [Tooltip("All possible symbols that can appear on the reels.")]
    public SymbolData[] symbolPool;

    [Header("UI References")]
    public Button spinButton;
    public TMP_Text balanceText;
    public TMP_Text resultText;

    [Header("Economy Settings")]
    [Tooltip("Credits the player starts with.")]
    public int startingBalance = 1000;

    [Tooltip("Cost to play one spin.")]
    public int betAmount = 10;

    private int currentBalance;
    private int reelsFinished;
    private bool isSpinning;

    // Stores each reel's middle-row result once all reels finish spinning.
    private SymbolData[] finalMiddleRow;

    private void Start()
    {
        currentBalance = startingBalance;
        UpdateBalanceUI();

        if (spinButton != null)
            spinButton.onClick.AddListener(OnSpinButtonPressed);
    }

    /// <summary>
    /// Called when the player presses the Spin button.
    /// Validates balance, deducts the bet, and starts all reels spinning.
    /// </summary>
    public void OnSpinButtonPressed()
    {
        if (isSpinning)
            return; // Prevent double-spin while reels are already in motion.

        if (currentBalance < betAmount)
        {
            resultText.text = "Not enough credits!";
            return;
        }

        currentBalance -= betAmount;
        UpdateBalanceUI();

        StartSpin();
    }

    private void StartSpin()
    {
        isSpinning = true;
        reelsFinished = 0;
        finalMiddleRow = new SymbolData[reels.Length];
        resultText.text = "";

        if (spinButton != null)
            spinButton.interactable = false; // Block input until this spin resolves.

        // Start every reel spinning. Each reel calls back independently when it stops,
        // because each has its own randomized duration + stagger delay (set in Step 4).
        for (int i = 0; i < reels.Length; i++)
        {
            int reelIndex = i; // Local copy to avoid closure capture issues in the lambda.
            reels[i].StartSpin(symbolPool, (result) => OnReelStopped(reelIndex, result));
        }
    }

    /// <summary>
    /// Called by each Reel when it finishes spinning.
    /// Once ALL reels have reported in, evaluate the win condition.
    /// </summary>
    private void OnReelStopped(int reelIndex, SymbolData[] result)
    {
        finalMiddleRow[reelIndex] = reels[reelIndex].GetMiddleSymbol();

        // Guard: if a reel's symbolSlots weren't wired up in the Inspector, GetMiddleSymbol()
        // returns null. Fail loudly here with a clear message instead of crashing later.
        if (finalMiddleRow[reelIndex] == null)
        {
            Debug.LogError($"Reel index {reelIndex} ({reels[reelIndex].name}) returned a null symbol. " +
                           "Check that its 'Symbol Slots' array is assigned in the Inspector.");
        }

        reelsFinished++;

        if (reelsFinished >= reels.Length)
        {
            EvaluateResult();
        }
    }

    /// <summary>
    /// Checks the pay line (middle row across all reels) for a win and applies payout.
    /// Win condition: all reels show the same symbol, OR are covered by Wild symbols.
    /// </summary>
    private void EvaluateResult()
    {
        isSpinning = false;
        if (spinButton != null)
            spinButton.interactable = true;

        bool isWin = CheckWinCondition(finalMiddleRow, out SymbolData matchedSymbol);

        if (isWin)
        {
            int payout = matchedSymbol.payoutValue * (betAmount / 10 > 0 ? betAmount / 10 : 1);
            currentBalance += payout;
            resultText.text = $"WIN! {matchedSymbol.symbolName} x3 — +{payout} credits!";
        }
        else
        {
            resultText.text = "No match. Try again!";
        }

        UpdateBalanceUI();
    }

    /// <summary>
    /// Determines whether the given pay-line symbols count as a win.
    /// All non-wild symbols must match; Wild symbols substitute for any symbol.
    /// </summary>
    private bool CheckWinCondition(SymbolData[] payLine, out SymbolData matchedSymbol)
    {
        matchedSymbol = null;

        // Safety check: if any reel failed to report a symbol (e.g. misconfigured slots),
        // treat the spin as a non-win instead of crashing.
        foreach (var symbol in payLine)
        {
            if (symbol == null)
            {
                Debug.LogWarning("SlotMachine: a reel returned a null middle symbol. " +
                                 "Check that every Reel's 'symbolSlots' array is fully assigned in the Inspector.");
                return false;
            }
        }

        // Find the first non-wild symbol to use as the "target" everything else must match.
        SymbolData target = null;
        foreach (var symbol in payLine)
        {
            if (!symbol.isWild)
            {
                target = symbol;
                break;
            }
        }

        // Edge case: all three symbols are Wild — treat as a win on the highest-value symbol.
        if (target == null)
        {
            matchedSymbol = GetHighestPayoutSymbol();
            return true;
        }

        // Every symbol on the line must either equal the target or be Wild.
        foreach (var symbol in payLine)
        {
            if (symbol != target && !symbol.isWild)
                return false;
        }

        matchedSymbol = target;
        return true;
    }

    private SymbolData GetHighestPayoutSymbol()
    {
        SymbolData best = symbolPool[0];
        foreach (var s in symbolPool)
        {
            if (s.payoutValue > best.payoutValue)
                best = s;
        }
        return best;
    }

    private void UpdateBalanceUI()
    {
        if (balanceText != null)
            balanceText.text = $"Balance: {currentBalance}";
    }
}