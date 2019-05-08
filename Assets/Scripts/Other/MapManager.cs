using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [SerializeField] MapDimentions mapDimentions = new MapDimentions(17, 15);
    [SerializeField] Tilemap markingArea;
    [SerializeField] Tilemap mineField;
    [Range(1, 50)] [SerializeField] int mineCount = 20;   
    [SerializeField] MineFieldTileContainer container;
    [SerializeField] UIStaff uiElements;

    float time = 0;
    bool isMapInitialized = false;
    bool isGameOver = false;
    Camera mainCamera;  
    List<Vector3Int> minePositions = new List<Vector3Int>();
    List<Vector3Int> flagsPositions = new List<Vector3Int>();

    void Start()
    {
        mainCamera = Camera.main;
        uiElements.buttonImage = FindObjectOfType<Button>().GetComponent<Image>();
        container.Init();
        ResetMap();
    }
    void Update()
    {
        UpdateMap();
    }

    void UpdateUI()
    {      
        if (isGameOver)
        {
            uiElements.minesCount.text = "Mines: 0";
            uiElements.timeCount.text = "Time: " + ((int)time).ToString() + " s";
        }
        else
        {
            time += Time.deltaTime;
            uiElements.minesCount.text = "Mines: " + (mineCount - flagsPositions.Count).ToString();            
            uiElements.timeCount.text = "Time: " + ((int)time).ToString() + " s";
        }       
    }

    public void ResetMap()
    {
        GenerateFrontMap(markingArea, container.empty);
        isMapInitialized = false;
        minePositions.Clear();
        flagsPositions.Clear();
        uiElements.buttonImage.sprite = Resources.Load<Sprite>("Textures/NormalFace");
        time = 0;
        isGameOver = false;
    } 

    private void UpdateMap()
    {
        UpdateUI();

        if (!Input.anyKey)
            return;

        Vector3Int cursorPosition = markingArea.GetPositionOfTileOverCursor(mainCamera);

        if (Input.GetMouseButtonDown(0) && markingArea.HasTile(cursorPosition))
        {
            if (minePositions.Contains(cursorPosition))
            {
                SetMapInCaseOfLoss(cursorPosition);
                return;
            }

            if (!isMapInitialized)
            {
                GenerateMines(mineField, cursorPosition, container.mine, container.numbers);
                isMapInitialized = true;
            }

            FloodFill(markingArea, cursorPosition, container.numbers[0]);
            markingArea.RemoveTileAtPosition(cursorPosition);

            SetMapInCaseOfWin();
        }
        else if (Input.GetMouseButtonDown(1) && markingArea.HasTile(cursorPosition))
        {
            PlaceFlagOrQuestionMark(cursorPosition);
        }
    }

    void SetMapInCaseOfWin()
    {
        if (DidWeWon())
        {
            MarkAllMinesWithFlags();
            ShowWholeMineField();
            uiElements.buttonImage.sprite = Resources.Load<Sprite>("Textures/WinFace");
            isGameOver = true;
        }
    }  

    void MarkAllMinesWithFlags()
    {
        foreach (var minePosition in minePositions)
        {
            mineField.SetTile(minePosition, container.flag);
        }
    }

    bool DidWeWon()
    {
        int tileCount = mapDimentions.GetMapArea() - mineCount;

        for (int x = 0; x < mapDimentions.width; x++)
        {
            for (int y = 0; y < mapDimentions.height; y++)
            {
                if (!markingArea.HasTile(new Vector3Int(x, y, 0)))
                {
                    tileCount--;
                }
            }
        }
        return tileCount == 0;
    }

    void SetMapInCaseOfLoss(Vector3Int cursorPosition)
    {
        ShowWholeMineField();
        MarkAllMinesAndMistakes( cursorPosition);

        uiElements.buttonImage.sprite = Resources.Load<Sprite>("Textures/LoseFace");
        isGameOver = true;
    }

    void MarkAllMinesAndMistakes(Vector3Int cursorPosition)
    {
        foreach (var flagPosition in flagsPositions)
        {
            if (!minePositions.Contains(flagPosition))
            {
                mineField.SetTile(flagPosition, container.notMine);
            }
            else
            {
                mineField.SetTile(flagPosition, container.flag);
            }
        }
        mineField.SetTile(cursorPosition, container.hitMine);
    }

    void ShowWholeMineField()
    {
        for (int x = 0; x < mapDimentions.width; x++)
        {
            for (int y = 0; y < mapDimentions.height; y++)
            {
                markingArea.SetTile(new Vector3Int(x, y, 0), null);
            }
        }
    }

    void PlaceFlagOrQuestionMark(Vector3Int cursorPosition)
    {
        MineTile actualTile = markingArea.GetTile<MineTile>(cursorPosition);
        if (actualTile == container.empty)
        {
            markingArea.SetTile(cursorPosition, container.flag);
            flagsPositions.Add(cursorPosition);
        }
        else if (actualTile == container.flag)
        {
            markingArea.SetTile(cursorPosition, container.questionMark);
            flagsPositions.Remove(cursorPosition);
        }
        else if (actualTile == container.questionMark)
        {
            markingArea.SetTile(cursorPosition, container.empty);
        }
    }

    void FloodFill(Tilemap tilemap, Vector3Int position, TileBase zeroTile)
    {
        Queue<Vector3Int> quere = new Queue<Vector3Int>();
        quere.Enqueue(position);
        while (quere.Count > 0)
        {
            Vector3Int newPosition = quere.Dequeue();
            if (markingArea.HasTile(newPosition))
            {
                markingArea.RemoveTileAtPosition(newPosition);

                if (mineField.GetTile<TileBase>(newPosition).Equals(zeroTile))
                {
                    quere.Enqueue(newPosition + Vector3Int.left);
                    quere.Enqueue(newPosition + Vector3Int.right);
                    quere.Enqueue(newPosition + Vector3Int.up);
                    quere.Enqueue(newPosition + Vector3Int.down);

                    quere.Enqueue(newPosition + Vector3Int.left + Vector3Int.up);
                    quere.Enqueue(newPosition + Vector3Int.left + Vector3Int.down);
                    quere.Enqueue(newPosition + Vector3Int.right + Vector3Int.up);
                    quere.Enqueue(newPosition + Vector3Int.right + Vector3Int.down);
                }
            }
        }
    }

    void GenerateMines(Tilemap tilemap, Vector3Int firstClickPosition, TileBase mine, TileBase[] number)
    {
        while (minePositions.Count < mineCount)
        {
            Vector3Int position = new Vector3Int(
                    Random.Range(0, mapDimentions.width),
                    Random.Range(0, mapDimentions.height),
                    0);


            if (firstClickPosition != position && !minePositions.Contains(position) && !minePositions.Contains(firstClickPosition))
            {
                tilemap.SetTile(position, mine);
                minePositions.Add(position);
            }
        }

        for (int i = 0; i < this.mapDimentions.width; i++)
        {
            for (int j = 0; j < this.mapDimentions.height; j++)
            {
                if (!minePositions.Contains(new Vector3Int(i, j, 0)))
                {
                    tilemap.SetTile(new Vector3Int(i, j, 0), number[MinesCount(new Vector3Int(i, j, 0))]);
                }
            }
        }
    }

    int MinesCount(Vector3Int position)
    {
        int z = 0;
        for (int i = position.x - 1; i < position.x + 2; i++)
        {
            for (int j = position.y - 1; j < position.y + 2; j++)
            {
                Vector3Int neighbourPosition = new Vector3Int(i, j, 0);

                if (minePositions.Contains(neighbourPosition))
                {
                    z++;
                }
            }
        }

        return z;
    }

    void GenerateFrontMap(Tilemap tilemap, TileBase empty)
    {
        for (int i = 0; i < this.mapDimentions.width; i++)
        {
            for (int j = 0; j < this.mapDimentions.height; j++)
            {
                tilemap.SetTile(new Vector3Int(i, j, 0), empty);
            }
        }
    }
}


