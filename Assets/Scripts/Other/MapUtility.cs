using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class MapUtility
{
//
//    /// <summary>
//    /// Removes tile at position (sets it null)
//    /// </summary>
//    /// <param name="positionOfCell"></param>
    public static void RemoveTileAtPosition(this Tilemap t, Vector3Int positionOfCell)
    {
        t.SetTile(positionOfCell, null);
    }

    public static Vector3Int ScreenToCellPoint(this Tilemap t, Camera camera, Vector3 screenPoint)
    {
        Vector3 worldPoint = camera.ScreenToWorldPoint(screenPoint);
        Vector3Int cellPosition = t.layoutGrid.WorldToCell(worldPoint);
        return cellPosition;
    }

    public static Vector3Int GetPositionOfTileOverCursor(this Tilemap tilemap, Camera camera)
    {
        Vector3Int position = camera.ScreenToCellPoint(tilemap, Input.mousePosition);
        return new Vector3Int(position.x,position.y,0);
    }
}
