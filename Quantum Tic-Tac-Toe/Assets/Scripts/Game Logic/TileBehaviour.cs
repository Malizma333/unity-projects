using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

internal class TileBehaviour : MonoBehaviour {
    private static GameManager gameManager;

    // Display constants for the UI
    private static readonly int smallFontSize = 3, largeFontSize = 5;
    
    // Components
    [SerializeField]
    private TextMeshProUGUI tileText;
    [SerializeField]
    private Image sprite;

    [SerializeField]
    private Color defaultColor;
    [SerializeField]
    private Color errorColor;

    // Tile state information
    private int[] tilePosition;
    private int tileValue;
    private int tileID;
    private bool flashing;
    private List<Entanglement> tileEntanglements;
    
    public int TileValue { get { return tileValue; } set { tileValue = value; } }
    public int TileEntID { get { return tileID; } set { tileID = value; } }
    public bool Collapsed { get { return tileValue != -1; } }
    public List<Entanglement> TileEnts { get { return tileEntanglements; } }

    public void InitializeProps(GameManager gameManager, int[] position) {
        TileBehaviour.gameManager = gameManager;
        this.tilePosition = position;
        this.tileValue = -1;
        this.tileID = -1;
        this.flashing = false;
        this.tileEntanglements = new List<Entanglement>();
        sprite.color = defaultColor;
    }

    // Detects click on tile
    private void OnMouseDown() {
        if(GameBoardManager.WinningPlayer > -1) {
            return;
        }
        
        if(Collapsed) {
            ShowInvalid();
            return;
        }

        switch(gameManager.MoveMode) {
            case MovementMode.Entangle: {
                gameManager.MakeEntanglementMove(tilePosition);
                break;
            }

            case MovementMode.Collapse: {
                gameManager.CollapseTile(tilePosition, EarlyEntInCycle());
                break;
            }

            case MovementMode.Classic: {
                gameManager.MakeClassicMove(tilePosition);
                break;
            }
            default: return;
        }
        
        gameManager.UpdateVisuals();
    }
    
    // Finds the earliest entanglement in the cycle
    private Entanglement EarlyEntInCycle() {
        foreach(Entanglement e in tileEntanglements) {
            if(EntanglementManager.CycleContainsID(e.ID)) {
                return e;
            }
        }

        return null;
    }

    // Updates the tile graphic
    public void UpdateVisual() {
        if(Collapsed) {
            SetTextCollapsed();   
        } else {
            SetTextEntangled();
        }
    }

    // Updates text for collapsed tiles
    private void SetTextCollapsed() {
        tileText.fontSize = largeFontSize;

        string color = ColorUtility.ToHtmlStringRGB(DisplayManager.PlayerColors[tileValue]);
        string symbol = DisplayManager.PlayerSymbols[tileValue];

        tileText.text = $"<color=#{color}>{symbol}";
        if(tileID != -1) {
            tileText.text += $"<sub>{tileID}</sub>";
        }
    }

    // Updates text for entangled tiles
    private void SetTextEntangled() {
        tileText.text = "";
        tileText.fontSize = smallFontSize;
        
        int i = 0;

        foreach(Entanglement e in tileEntanglements) {
            if(i++ > 2) {
                tileText.text += "<color=#000000>...";
                break;
            }
            
            string color = ColorUtility.ToHtmlStringRGB(DisplayManager.PlayerColors[e.Value]);
            string symbol = DisplayManager.PlayerSymbols[e.Value];

            tileText.text += $"<color=#{color}>{symbol}<sub>{e.ID}</sub> ";
        }
    }

    // Adds reference to entanglements related to tile
    public void AddEntReference(Entanglement e) {
        tileEntanglements.Add(e);
    }

    private void ShowInvalid() {        
        if(flashing) {
            StopCoroutine("Flash");
            flashing = false;
        }

        StartCoroutine("Flash");
    }

    private IEnumerator Flash() {
        flashing = false;
        sprite.color = errorColor;
        
        yield return new WaitForSeconds(.15f);

        flashing = true;
        sprite.color = defaultColor;
    }
}