using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Tile
{
    
    public (int x_coordinate, int y_coordinate) GetTileCoordinates();

    public float GetTileHeight();

    
}
