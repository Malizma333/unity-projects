using UnityEngine;

public class CellGameObject : MonoBehaviour
{
    public enum State
    {
        Empty = 0,
        Invalid = 1,
        Valid = 2
    }

    [Space(10)]
    [Header("References")]
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private Sprite crossSprite;
    
    private SpriteRenderer spriteRend;
    private Sprite square;
    private State cellState;
    private Vector2 index;

    public State CellState {
        get { return cellState; }
    }

    public Vector2 Index
    {
        get { return index; }
        set { index = value; }
    }

    public void SetCellState(bool state)
    {
        if (cellState == State.Empty)
        {
            if(state)
            {
                cellState = State.Valid;
                spriteRend.sprite = circleSprite;
            }
            else
            {
                cellState = State.Invalid;
                spriteRend.sprite = crossSprite;
            }
        }
    }

    private void Awake()
    {
        cellState = State.Empty;
        spriteRend = GetComponent<SpriteRenderer>();
        square = spriteRend.sprite;
    }

    private void OnMouseOver()
    {
        if(cellState == State.Empty)
        {
            if (Input.GetMouseButtonDown(0))
            {
                SetCellState(true);
            }

            if (Input.GetMouseButtonDown(1))
            {
                SetCellState(false);
            }
        }

        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            cellState = State.Empty;
            spriteRend.sprite = square;
        }
    }
}
