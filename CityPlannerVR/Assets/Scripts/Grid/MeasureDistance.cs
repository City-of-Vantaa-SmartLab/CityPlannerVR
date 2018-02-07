using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeasureDistance {

    public static float CalculateDistance(GridTile tileA, GridTile tileB)
    {
        float distX = Mathf.Abs(tileA.xPos - tileB.xPos);
        float distZ = Mathf.Abs(tileA.zPos - tileB.zPos);

        if (distX > distZ)
        {
            //1.4 is the distance needed to go in diagonal line (if normally distance is 1)
            return 1.4f * distZ + (distX - distZ);
        }
        return 1.4f * distX + (distZ - distX);
    }
}
