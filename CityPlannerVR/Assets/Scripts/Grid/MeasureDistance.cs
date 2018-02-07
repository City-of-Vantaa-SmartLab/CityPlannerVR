using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Is used with and without pathfinding to determine distance between two tiles
/// </summary>
public static class MeasureDistance {

    public static float CalculateDistance(GridTile tileA, GridTile tileB)
    {
        float distX = Mathf.Abs(tileA.xPos - tileB.xPos);
        float distZ = Mathf.Abs(tileA.zPos - tileB.zPos);

        if (distX > distZ)
        {
            //1.4 is the diagonal distance
            return 1.4f * distZ + (distX - distZ);
        }
        return 1.4f * distX + (distZ - distX);
    }
}
