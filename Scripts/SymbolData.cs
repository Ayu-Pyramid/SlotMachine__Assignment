using UnityEngine;

/// <summary>
/// Defines a single slot symbol type as a data asset (ScriptableObject).
/// Create instances via: Assets > Create > SlotGame > Symbol Data
/// </summary>
[CreateAssetMenu(fileName = "NewSymbol", menuName = "SlotGame/Symbol Data")]
public class SymbolData : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Display name, e.g. Cherry, Bell, Seven, Wild.")]
    public string symbolName;

    [Tooltip("The sprite shown on the reel for this symbol.")]
    public Sprite symbolSprite;

    [Header("Payout")]
    [Tooltip("Credits awarded when all 3 reels match this symbol on the pay line.")]
    public int payoutValue = 10;

    [Header("Special Behaviour")]
    [Tooltip("If true, this symbol substitutes for any other symbol when checking wins.")]
    public bool isWild = false;
}