using System.Collections.Generic;
using UnityEngine;

public partial class HeightmapToMesh : MonoBehaviour
{
    public class Node
    {
        public int I;
        public int J;
        public List<(int, int)> Neighbors;
        public bool IsBorder;
    }
    
    private void BuildBorders(int[,] grid)
    {
        for (int i = 0; i < Size; ++i)
        {
            for (int j = 0; j < Size; ++j)
            {
                if (grid[j, i] == 0) continue;
                
                var rect = new RectangleData(new RectInt(i, j, 1, 1), grid);
                if (rect.Diagonals.Count > 0)
                {
                    //DiagonalData diagonalData = new DiagonalData(i, j, rect.Diagonals.RandomElement());
                    //DiagonalList.Add(diagonalData);
                }
            }
        }
    }

    private Node BuildNode(int[,] grid, int row, int col, EDiagonalType vertex)
    {
        switch (vertex)
        {
            case EDiagonalType.BottomLeft:
                for (int i = -1; i <= 1; ++i)
                {
                    for (int j = -1; j <= 1; ++j)
                    {
                        if (grid[j, i] == 0) continue;
                
                        
                    }
                }

                break;
        }

        return null;
    }
}
