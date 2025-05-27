using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RectangleData
{
    public int X;
    public int Y;
    public int Width;
    public int Height;
    public HashSet<EDiagonalType> Diagonals;
    
    public RectangleData(RectInt rect, int[,] grid)
    {
        X = rect.x;
        Y = rect.y;
        Width = rect.width;
        Height = rect.height;
        Diagonals = new HashSet<EDiagonalType>();
        EvaluateBorders(grid);
    }

    private void EvaluateBorders(int[,] grid)
    {
        if (HasBottomLeftVertex(grid)) Diagonals.Add(EDiagonalType.BottomLeft);
        if (HasBottomRightVertex(grid)) Diagonals.Add(EDiagonalType.BottomRight);
        if (HasTopLeftVertex(grid)) Diagonals.Add(EDiagonalType.TopLeft);
        if (HasTopRightVertex(grid)) Diagonals.Add(EDiagonalType.TopRight);
    }
    
    private bool HasBottomLeftVertex(int[,] grid)
    {
        // Left neighbor
        if (X - 1 < 0) return true;
        if (grid[Y, X - 1] is 0) return true;
        if (grid[Y, X - 1] is < 0 and not(int)EDiagonalType.BottomRight) return true;
        
        // Bottom neighbor
        if (Y - 1 < 0) return true;
        if (grid[Y - 1, X] is 0) return true;
        if (grid[Y - 1, X] is < 0 and not (int)EDiagonalType.TopLeft) return true;
        
        // Bottom left neighbor
        if (X - 1 < 0 || Y - 1 < 0) return true;
        if (grid[Y - 1, X - 1] is 0) return true;
        if (grid[Y - 1, X - 1] is < 0 and not (int)EDiagonalType.TopRight) return true;

        return false;
    }
    
    private bool HasBottomRightVertex(int[,] grid)
    {
        // Right neighbor
        if (X + Width >= grid.GetLength(1)) return true;
        if (grid[Y, X + Width] is 0) return true;
        if (grid[Y, X + Width] is < 0 and not (int)EDiagonalType.BottomLeft) return true;
        
        // Bottom neighbor
        if (Y - 1 < 0) return true;
        if (grid[Y - 1, X] is 0) return true;
        if (grid[Y - 1, X] is < 0 and not (int)EDiagonalType.TopRight) return true;
        
        // Bottom right neighbor
        if (X + Width >= grid.GetLength(1) || Y - 1 < 0) return true;
        if (grid[Y - 1, X + Width] is 0) return true;
        if (grid[Y - 1, X + Width] is < 0 and not (int)EDiagonalType.TopLeft) return true;

        return false;
    }
    
    private bool HasTopLeftVertex(int[,] grid)
    {
        // Left neighbor
        if (X - 1 < 0) return true;
        if (grid[Y, X - 1] is 0) return true;
        if (grid[Y, X - 1] is < 0 and not (int)EDiagonalType.TopRight) return true;
        
        // Top neighbor
        if (Y + Height >= grid.GetLength(0)) return true;
        if (grid[Y + Height, X] is 0) return true;
        if (grid[Y + Height, X] is < 0 and not (int)EDiagonalType.BottomLeft) return true;
        
        // Top left neighbor
        if (X - 1 < 0 || Y + Height >= grid.GetLength(0)) return true;
        if (grid[Y + Height, X] is 0) return true;
        if (grid[Y + Height, X] is < 0 and not (int)EDiagonalType.BottomRight) return true;

        return false;
    }
    
    private bool HasTopRightVertex(int[,] grid)
    {
        // Right neighbor
        if (X + Width >= grid.GetLength(1)) return true;
        if (grid[Y, X + Width] is 0) return true;
        if (grid[Y, X + Width] is < 0 and not (int)EDiagonalType.TopLeft) return true;
        
        // Top neighbor
        if (Y + Height >= grid.GetLength(0)) return true;
        if (grid[Y + Height, X] is 0) return true;
        if (grid[Y + Height, X] is < 0 and not (int)EDiagonalType.BottomRight) return true;
        
        // Top right neighbor
        if (X + Width >= grid.GetLength(1) || Y + Height >= grid.GetLength(0)) return true;
        if (grid[Y + Height, X + Width] is 0) return true;
        if (grid[Y + Height, X + Width] is < 0 and not (int)EDiagonalType.BottomLeft) return true;

        return false;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append($"X: {X}, Y: {Y}, Width: {Width}, Height: {Height}");
        if (Diagonals.Count == 0)
        {
            return sb.ToString();
        }       
        sb.Append(" ------ Diagonals: {");
        if (Diagonals.Contains(EDiagonalType.BottomLeft)) sb.Append(" ↙");
        if (Diagonals.Contains(EDiagonalType.BottomRight)) sb.Append(" ↘"); 
        if (Diagonals.Contains(EDiagonalType.TopLeft)) sb.Append(" ↖");
        if (Diagonals.Contains(EDiagonalType.TopRight)) sb.Append(" ↗");
        sb.Append(" }");
        return sb.ToString();
    }
}
