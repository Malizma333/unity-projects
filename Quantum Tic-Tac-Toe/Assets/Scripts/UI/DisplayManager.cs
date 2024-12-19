using UnityEngine;
using TMPro;

internal class DisplayManager : MonoBehaviour {
    private static GameManager gameManager;

    [SerializeField]
    private GameObject linePrefab;
    [SerializeField]
    private TextMeshProUGUI infoText;

    private static readonly string[] playerSymbols = new string[] {"X", "O", "<b>△</b>", "<b>□</b>"};
    private static readonly Color[] playerColors = new Color[] {
        Color.red, Color.blue, Color.green, Color.magenta
    };
    
    public static string[] PlayerSymbols { get { return playerSymbols; } }
    public static Color[] PlayerColors { get { return playerColors; } }

    public void Initialize(GameManager gameManager) {
        DisplayManager.gameManager = gameManager;
    }

    public void UpdateVisuals() {
        ShowTurn(gameManager.CurrentPlayer, gameManager.MoveMode);

        Vector2[] winEdge = GameBoardManager.WinningEdge();

        if(winEdge != null) {
            ShowWinner(GameBoardManager.WinningPlayer);
            Vector2[] transformedEdge = GameBoardManager.TransformEdge(winEdge);
            CreateWinLine(transformedEdge, GameBoardManager.WinningPlayer);
        } else if(GameBoardManager.GetPlayableCount() == 0) {
            ShowWinner(-1);
        }
    }

    public void ShowTurn(int currentPlayer, MovementMode moveMode) {
        string color = ColorUtility.ToHtmlStringRGB(playerColors[currentPlayer]);
        string symbol = playerSymbols[currentPlayer];

        infoText.text = $"<color=#{color}>{symbol}'s</color> Move";

        if(moveMode != MovementMode.Classic) {
            infoText.text += $"\n\n{moveMode}";
        }
    }

    public void CreateWinLine(Vector2[] edgePos, int winner) {
        LineRenderer winLine = Instantiate(
            linePrefab, Vector3.zero, Quaternion.identity
        ).GetComponent<LineRenderer>();

        winLine.SetPosition(0, edgePos[0]);
        winLine.SetPosition(1, edgePos[1]);

        winLine.startColor = playerColors[winner];
        winLine.endColor = playerColors[winner];
    }

    public void ShowWinner(int winner) {        
        if(winner == -1) {
            infoText.text = "It's a draw!";
            return;
        }

        string color = ColorUtility.ToHtmlStringRGB(playerColors[winner]);
        string symbol = playerSymbols[winner];
        infoText.text = $"<color=#{color}>{symbol}</color> wins!";
    }
}
