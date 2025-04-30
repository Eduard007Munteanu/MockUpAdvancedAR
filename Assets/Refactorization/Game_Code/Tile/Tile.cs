using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Tile
{
    
    public (float, float) GetTileCoordinates();

    public float GetTileHeight();

    
}
