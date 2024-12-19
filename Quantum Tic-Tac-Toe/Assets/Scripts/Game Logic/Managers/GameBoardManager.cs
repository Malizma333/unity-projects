using UnityEngine;

internal static class GameBoardManager {
    private static GameManager gameManager;

    // Constants for how the board is displayed
    private static readonly float tileSizingConstant = 10f;
    private static readonly float tileSpacingConstant = 0.5f;

    // Board properties
    private static int gameBoardSize;
    private static TileBehaviour[,] gameBoardTiles;
    private static int winningPlayer;

    public static int WinningPlayer { get { return winningPlayer; } }

    public static void Initialize(GameManager gameManager) {
        GameBoardManager.gameManager = gameManager;
        GameBoardManager.gameBoardSize = gameManager.BoardSize;

        winningPlayer = -1;
        gameBoardTiles = new TileBehaviour[gameBoardSize, gameBoardSize];

        CreateBoard();
        ConfigureBoardComponents();
    }

    // Creates a grid of tiles
    private static void CreateBoard() {
        for(int i = 0; i < gameBoardSize; i++) {
            for(int j = 0; j < gameBoardSize; j++) {
                CreateTile(i, j);
            }
        }
    }

    // Creates a new tile given a position
    private static void CreateTile(int i, int j) {
        Vector2 displayPosition = (tileSizingConstant + tileSpacingConstant) * new Vector2(i, j);
        displayPosition += (tileSizingConstant + tileSpacingConstant) * (gameBoardSize - 1) * -.5f * Vector2.one;

        gameBoardTiles[i, j] = GameObject.Instantiate(
            gameManager.TilePrefab, displayPosition, Quaternion.identity
        ).GetComponent<TileBehaviour>();

        gameBoardTiles[i,j].transform.SetParent(gameManager.GameCanvas);
        gameBoardTiles[i,j].GetComponent<RectTransform>().sizeDelta = new Vector2(tileSizingConstant, tileSizingConstant);
        gameBoardTiles[i,j].InitializeProps(gameManager, new int[] {i, j});
    }

    // Configures some miscellaneous UI properties
    private static void ConfigureBoardComponents() {
        Camera.main.orthographicSize = gameBoardSize * (0.5f * tileSizingConstant + tileSpacingConstant);

        RectTransform backgroundTransform = gameManager.BackgroundGrid.GetComponent<RectTransform>();
        backgroundTransform.sizeDelta = ((tileSizingConstant + tileSpacingConstant) * gameBoardSize - tileSpacingConstant) * Vector2.one;
        backgroundTransform.gameObject.SetActive(true);
    }

    // Update visuals of each tile
    public static void UpdateVisuals() {
        for(int i = 0; i < gameBoardSize; i++) {
            for(int j = 0; j < gameBoardSize; j++) {
                gameBoardTiles[i, j].UpdateVisual();
            }
        }
    }

    // Adds entanglement object reference to specific tile
    public static void AddReference(int[] tilePos, Entanglement e) {
        gameBoardTiles[tilePos[0], tilePos[1]].AddEntReference(e);
    }

    // Collapses tiles part of a cycle
    public static void CollapseTile(int[] position, Entanglement currentE) {
        TileBehaviour currentTile = gameBoardTiles[position[0], position[1]];
        currentTile.TileValue = currentE.Value;
        currentTile.TileEntID = currentE.ID;

        foreach(Entanglement newE in currentTile.TileEnts) {
            if(newE.ID == currentE.ID) {
                continue;
            }
            
            foreach(int[] pos in newE.Positions) {
                if(gameBoardTiles[pos[0], pos[1]].Collapsed) {
                    continue;
                }

                CollapseTile(pos, newE);
            }
        }
    }

    // Sets tile value regardless of entanglements
    public static void SetTile(int[] position, int playerValue, int id) {
        TileBehaviour currentTile = gameBoardTiles[position[0], position[1]];
        currentTile.TileValue = playerValue;
        currentTile.TileEntID = id;
    }

    // Returns the number of cells that haven't been collapsed/played
    public static int GetPlayableCount() {
        int count = 0;

        for(int i = 0; i < gameBoardSize; i++) {
            for(int j = 0; j < gameBoardSize; j++) {
                if(!gameBoardTiles[i,j].Collapsed) {
                    count++;
                }
            }
        }

        return count;
    }
    
    // Finds all of the winning positions on the board and returns earliest one
    public static Vector2[] WinningEdge() {
        Vector2[] edge = null;
        int earliestID = -1;

        CheckColumns(ref edge, ref earliestID);
        CheckRows(ref edge, ref earliestID);
        CheckDiagnols(ref edge, ref earliestID);

        return edge;
    }

    // Function to check each column of the grid to see if its winning
    private static void CheckColumns(ref Vector2[] edge, ref int earliestID) {
        // Iterate through each column
        for(int colI = 0; colI < gameBoardSize; colI++) {
            bool fullLine = true;
            int latestLineID = -1;
            
            for(int cellI = 0; true; cellI++) {
                // Don't want incomplete/entangled cells
                if(!gameBoardTiles[colI, cellI].Collapsed) {
                    fullLine = false;
                    break;
                }
                
                // Get the last-most placed tile
                if(latestLineID < gameBoardTiles[colI, cellI].TileEntID || latestLineID == -1) {
                    latestLineID = gameBoardTiles[colI, cellI].TileEntID;
                }
                
                // Conditional for-loop break
                if(cellI == gameBoardSize - 1) break;

                // Don't want cells that aren't the same value
                if(gameBoardTiles[colI, cellI].TileValue !=
                    gameBoardTiles[colI, cellI + 1].TileValue) {
                    fullLine = false;
                    break;
                }
            }

            // Check for earliest placed cell and update winning player
            if(fullLine && (earliestID > latestLineID || earliestID == -1)) {
                earliestID = latestLineID;
                edge = new Vector2[] {
                    new Vector2(colI, 0),
                    new Vector2(colI, gameBoardSize - 1)
                };
                winningPlayer = gameBoardTiles[colI, 0].TileValue;
            }
        }
    }

    // Function to check each row of the grid to see if its winning
    private static void CheckRows(ref Vector2[] edge, ref int earliestID) {        
        for(int rowI = 0; rowI < gameBoardSize; rowI++) {
            bool fullLine = true;
            int latestLineID = -1;
            
            for(int cellI = 0; true; cellI++) {
                if(!gameBoardTiles[cellI, rowI].Collapsed) {
                    fullLine = false;
                    break;
                }
                
                if(latestLineID < gameBoardTiles[cellI, rowI].TileEntID || latestLineID == -1) {
                    latestLineID = gameBoardTiles[cellI, rowI].TileEntID;
                }
                
                if(cellI == gameBoardSize - 1) break;

                if(gameBoardTiles[cellI, rowI].TileValue !=
                    gameBoardTiles[cellI + 1, rowI].TileValue) {
                    fullLine = false;
                    break;
                }
            }

            if(fullLine && (earliestID > latestLineID || earliestID == -1)) {
                earliestID = latestLineID;
                edge = new Vector2[] {
                    new Vector2(0, rowI),
                    new Vector2(gameBoardSize - 1, rowI)
                };
                winningPlayer = gameBoardTiles[0, rowI].TileValue;
            }
        }
    }

    // Function to check each diagnol of the grid to see if its winning
    private static void CheckDiagnols(ref Vector2[] edge, ref int earliestID) {        
        bool fullLine = true;
        int latestLineID = -1;
        
        for(int cellI = 0; true; cellI++) {
            if(!gameBoardTiles[cellI, cellI].Collapsed) {
                fullLine = false;
                break;
            }
            
            if(latestLineID < gameBoardTiles[cellI, cellI].TileEntID || latestLineID == -1) {
                latestLineID = gameBoardTiles[cellI, cellI].TileEntID;
            }
            
            if(cellI == gameBoardSize - 1) break;

            if(gameBoardTiles[cellI, cellI].TileValue !=
                gameBoardTiles[cellI + 1, cellI + 1].TileValue) {
                fullLine = false;
                break;
            }
        }

        if(fullLine && (earliestID > latestLineID || earliestID == -1)) {
            earliestID = latestLineID;
            edge = new Vector2[] {
                new Vector2(0, 0),
                new Vector2(gameBoardSize - 1, gameBoardSize - 1)
            };
            winningPlayer = gameBoardTiles[0, 0].TileValue;
        }

        fullLine = true;
        latestLineID = -1;
        
        for(int cellI = 0; true; cellI++) {
            if(!gameBoardTiles[gameBoardSize - cellI - 1, cellI].Collapsed) {
                fullLine = false;
                break;
            }
            
            if(latestLineID < gameBoardTiles[gameBoardSize - cellI - 1, cellI].TileEntID || latestLineID == -1) {
                latestLineID = gameBoardTiles[gameBoardSize - cellI - 1, cellI].TileEntID;
            }
            
            if(cellI == gameBoardSize - 1) break;

            if(gameBoardTiles[gameBoardSize - cellI - 1, cellI].TileValue !=
                gameBoardTiles[gameBoardSize - cellI - 2, cellI + 1].TileValue) {
                fullLine = false;
                break;
            }
        }

        if(fullLine && (earliestID > latestLineID || earliestID == -1)) {
            earliestID = latestLineID;
            edge = new Vector2[] {
                new Vector2(0, gameBoardSize - 1),
                new Vector2(gameBoardSize - 1, 0)
            };
            winningPlayer = gameBoardTiles[gameBoardSize - 1, 0].TileValue;
        }
    }

    // Transforms edge from grid position to display position
    public static Vector2[] TransformEdge(Vector2[] edge) {
        if(edge[0].x == edge[1].x) {
            edge[0].x += .5f;
            edge[1].x += .5f;
            edge[1].y += 1f;
        } else if(edge[0].y == edge[1].y) {
            edge[0].y += .5f;
            edge[1].y += .5f;
            edge[1].x += 1f;
        } else if(edge[0].y < edge[1].y) {
            edge[1].x += 1f;
            edge[1].y += 1f;
        } else {
            edge[1].x += 1f;
            edge[0].y += 1f;
        }

        edge[0].x = (tileSizingConstant + tileSpacingConstant) * (edge[0].x - gameBoardSize/2f);
        edge[0].y = (tileSizingConstant + tileSpacingConstant) * (edge[0].y - gameBoardSize/2f);
        edge[1].x = (tileSizingConstant + tileSpacingConstant) * (edge[1].x - gameBoardSize/2f);
        edge[1].y = (tileSizingConstant + tileSpacingConstant) * (edge[1].y - gameBoardSize/2f);

        return edge;
    }
}
