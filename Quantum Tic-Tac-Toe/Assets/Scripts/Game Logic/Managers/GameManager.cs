using UnityEngine;

internal enum MovementMode {
    Entangle = 0,
    Collapse = 1,
    Classic = 2
}

internal class GameManager : MonoBehaviour {
    // User-defined settings
    private int gameBoardSize;
    private int numberOfPlayers;
    [SerializeField]
    private int entangleCap = 2;

    // Component references
    [Header("Components")]
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private Transform gameCanvas;
    [SerializeField]
    private GameObject backgroundGrid;
    [SerializeField]
    private ButtonManager buttonManager;
    [SerializeField]
    private DisplayManager displayManager;

    // Current game information
    private int currentPlayer;
    private int currentTurn;
    private MovementMode moveMode;

    public int CurrentPlayer { get { return currentPlayer; } }
    public int BoardSize { get { return gameBoardSize; } }
    public int EntangleCap { get { return entangleCap; } }
    public MovementMode MoveMode { get { return moveMode; } }

    public GameObject TilePrefab { get { return tilePrefab; } }
    public Transform GameCanvas { get { return gameCanvas; } }
    public GameObject BackgroundGrid { get { return backgroundGrid; } }

    private void Start() {
        currentPlayer = 0;
        currentTurn = 1;

        gameBoardSize = PlayerPrefs.GetInt("SizeSlider", 3);
        numberOfPlayers = PlayerPrefs.GetInt("PlayerSlider", 2);

        buttonManager.Initialize(this);
        displayManager.Initialize(this);

        GameBoardManager.Initialize(this);
        EntanglementManager.Initialize(this);
        
        if(PlayerPrefs.GetInt("ModeSlider", 0) == 0) {
            moveMode = MovementMode.Classic;
        } else {
            moveMode = MovementMode.Entangle;
        }
        
        UpdateVisuals();
    }

    // Switches the current player
    private void SwitchPlayers() {
        currentPlayer = (currentPlayer + 1) % numberOfPlayers;
    }

    // Validates entanglement creation/deletion process
    public void MakeEntanglementMove(int[] tilePosition) {
        if(!EntanglementManager.CurrentEntOpen) {
            EntanglementManager.InitializeEntanglement(currentPlayer, currentTurn);
        }

        EntanglementManager.AddToEntanglement(tilePosition);

        if(EntanglementManager.CurrentEntLength == entangleCap) {
            EntanglementManager.CloseEntanglement();

            if(EntanglementManager.ContainsCycle) {
                moveMode = MovementMode.Collapse;
            }

            SwitchPlayers();
            currentTurn++;
        }

        if(EntanglementManager.ContainsCycle) {
            moveMode = MovementMode.Collapse;
        }
    }

    public void CollapseTile(int[] tilePosition, Entanglement e) {
        if(e is null) {
            return;
        }

        if(!EntanglementManager.CycleContainsID(e.ID)) {
            return;
        }

        EntanglementManager.ClearCycle();

        GameBoardManager.CollapseTile(tilePosition, e);
        
        if(GameBoardManager.GetPlayableCount() == 1) {
            moveMode = MovementMode.Classic;
        } else {
            moveMode = MovementMode.Entangle;
        }
    }

    public void MakeClassicMove(int[] tilePosition) {
        GameBoardManager.SetTile(tilePosition, currentPlayer, currentTurn);
        SwitchPlayers();
        currentTurn++;
    }

    // Adds reference to an entanglement for each relevant tiles
    public void AddEntReferences(int[] pos, Entanglement e) {
        GameBoardManager.AddReference(pos, e);
    }

    // Updates the board UI
    public void UpdateVisuals() {
        displayManager.UpdateVisuals();
        GameBoardManager.UpdateVisuals();
    }
}