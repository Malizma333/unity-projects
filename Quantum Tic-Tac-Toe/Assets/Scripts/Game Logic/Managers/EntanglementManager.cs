using System.Collections.Generic;

internal static class EntanglementManager {
    private static GameManager gameManager;

    private static Dictionary<Entanglement, List<Entanglement>[]> entGraph;
    private static Entanglement currentEnt;
    private static List<int> cycleIDs;
    private static bool containsCycle;

    public static bool ContainsCycle { get { return containsCycle; } }
    public static bool CurrentEntOpen { get { return currentEnt is not null; } }

    public static int CurrentEntLength { get {
        return CurrentEntOpen ? currentEnt.Positions.Count : 0;
    } }

    public static void Initialize(GameManager gameManager) {
        EntanglementManager.gameManager = gameManager;

        containsCycle = false;
        currentEnt = null;
        cycleIDs = new List<int>();
        entGraph = new Dictionary<Entanglement, List<Entanglement>[]>();
    }

    // Initializes a new entanglement
    public static void InitializeEntanglement(int value, int id) {
        currentEnt = new Entanglement(value, id);
    }

    // Adds to the currently open entanglement
    public static void AddToEntanglement(int[] pos) {
        foreach(int[] positions in currentEnt.Positions) {
            if(pos[0] == positions[0] && pos[1] == positions[1]) {
                return;
            }
        }

        currentEnt.AddPosition(pos);
        gameManager.AddEntReferences(pos, currentEnt);
    }

    // Stores current entanglement and removes object reference
    public static void CloseEntanglement() {
        if(currentEnt is null) {
            return;
        }

        StoreEnt(currentEnt);
        
        containsCycle = CheckCycle(currentEnt);
        
        currentEnt = null;
    }

    // Adds entanglement to entanglement graph
    private static void StoreEnt(Entanglement ent) {
        if(!entGraph.ContainsKey(ent)) {
            entGraph.Add(ent, new List<Entanglement>[gameManager.EntangleCap]);
        }

        foreach(KeyValuePair<Entanglement, List<Entanglement>[]> ePair in entGraph) {
            Entanglement newEnt = ePair.Key;

            if(newEnt.ID == ent.ID) {
                continue;
            }

            for(int i = 0; i < ent.Positions.Count; i++) {
                for(int j = 0; j < newEnt.Positions.Count; j++) {
                    int[] posA = ent.Positions[i];
                    int[] posB = newEnt.Positions[j];
                    if(posA[0] == posB[0] && posA[1] == posB[1]) {
                        if(entGraph[ent][i] is null) {
                            entGraph[ent][i] = new List<Entanglement>();
                        }

                        entGraph[ent][i].Add(newEnt);

                        if(entGraph[newEnt][j] is null) {
                            entGraph[newEnt][j] = new List<Entanglement>();
                        }

                        entGraph[newEnt][j].Add(ent);
                    }
                }
            }
        }
    }

    // Check for a cycle in the entanglements in case it needs to be collapsed
    private static bool CheckCycle(Entanglement currentE, int[] currentPos = null, 
                                   List<int[]> visited = null, Entanglement parentE = null) {
        
        if(visited is null) {
            visited = new List<int[]>();
        }

        if(currentPos is not null) {
            visited.Add(currentPos);
        }

        bool parentSeen = false;
        
        for(int i = 0; i < currentE.Positions.Count; i++) {
            if(currentPos == currentE.Positions[i]) {
                continue;
            }
            
            if(!visited.Contains(currentE.Positions[i])) {
                List<Entanglement> adjEnts = entGraph[currentE][i];
                if(adjEnts is null) {
                    continue;
                }

                foreach(Entanglement adjE in adjEnts) {
                    if(CheckCycle(adjE, currentE.Positions[i], visited, currentE)) {
                        cycleIDs.Add(currentE.ID);
                        return true;
                    }
                }

                continue;
            }
            
            if(parentE is null) continue;

            if(parentE.ID != currentE.ID) {
                return true;
            } else {
                if(parentSeen) {
                    return true;
                }
                parentSeen = true;
            }
        }

        return false;
    }

    // Clears the currently stored entanglement cycle
    public static void ClearCycle() {
        containsCycle = false;
        cycleIDs = new List<int>();
    }

    public static bool CycleContainsID(int id) {
        return cycleIDs.Contains(id);
    }
    
    // Return string representation of entanglement graph
    public static string GraphString() {
        string output = "";
        foreach(KeyValuePair<Entanglement, List<Entanglement>[]> ePair in entGraph) {
            output += $"{ePair.Key} : {{";
            for(int i = 0; i < ePair.Value.Length; i++) {
                if(ePair.Value[i] is null) {
                    continue;
                }
                output += $" ({ePair.Key.Positions[i][0]}, {ePair.Key.Positions[i][1]}): {{";
                for(int j = 0; j < ePair.Value[i].Count; j++) {
                    output += $"{ePair.Value[i][j]}";
                    if(j < ePair.Value[i].Count - 1) output += ", ";
                }
                output += "}";
            }
            output += "}\n";
        }
        return output;
    }

    // Return string representation of cycles
    public static string CycleString() {
        string output = "{";
        for(int i = 0; i < cycleIDs.Count; i++) {
            output += $"{cycleIDs[i]}";
            if(i < cycleIDs.Count - 1) output += ", ";
        }
        output += "}";
        return output;
    }
}