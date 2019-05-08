using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class MineFieldTileContainer
{
    public MineTile empty;
    public MineTile notMine;
    public MineTile flag;
    public MineTile questionMark;

    public MineTile mine;
    public MineTile hitMine;
    public MineTile[] numbers = new MineTile[9];
    string path = "Tiles/";
    public void Init()
    {
        empty = Resources.Load<MineTile>(path+"Empty");
        mine = Resources.Load<MineTile>(path + "Mine");
        hitMine = Resources.Load<MineTile>(path + "hitMine");

        notMine = Resources.Load<MineTile>(path + "notMine");

        flag = Resources.Load<MineTile>(path + "flag");
        questionMark = Resources.Load<MineTile>(path + "questionMark");
        numbers[0] = Resources.Load<MineTile>(path + "zero");
        numbers[1] = Resources.Load<MineTile>(path + "one");
        numbers[2] = Resources.Load<MineTile>(path + "two");
        numbers[3] = Resources.Load<MineTile>(path + "tree");
        numbers[4] = Resources.Load<MineTile>(path + "four");
        numbers[5] = Resources.Load<MineTile>(path + "five");
        numbers[6] = Resources.Load<MineTile>(path + "six");
        numbers[7] = Resources.Load<MineTile>(path + "seven");
        numbers[8] = Resources.Load<MineTile>(path + "eight");
    }
}