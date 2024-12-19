using System.Collections.Generic;

// Class to store data about an entanglement
internal class Entanglement {
    // Coordinates that are entangled
    private List<int[]> positions;

    // Value and time index of the entanglement
    private int value;
    private int id;

    public List<int[]> Positions { get { return positions; } }
    public int Value { get { return value; } }
    public int ID { get { return id; } }

    public Entanglement(int value, int id) {
        this.value = value;
        this.id = id;
        this.positions = new List<int[]>();
    }

    //Adds position to entanglement coordinates
    public void AddPosition(int[] pos) {
        positions.Add(pos);
    }

    // Returns string representation of current object
    public override string ToString()
    {
        string output = "Entanglement {";
        output += $"ID: {this.id}, ";
        output += $"Value: {this.value}, ";
        output += $"Positions: {{";
        for(int i = 0; i < this.positions.Count; i++) {
            output += $"({this.positions[i][0]}, {this.positions[i][1]})";
            if(i < this.positions.Count - 1) output += ", ";
        }
        output += "}}";
        return output;
    }
}
