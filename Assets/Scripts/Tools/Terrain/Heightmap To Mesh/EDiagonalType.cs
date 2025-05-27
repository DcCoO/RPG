using UnityEngine;

public enum EDiagonalType 
{
    TopRight = -1,
    BottomRight = -2,
    BottomLeft = -3,
    TopLeft = -4,
}

public static class EDiagonalTypeExtensions
{
    public static EDiagonalType Opposite(this EDiagonalType type)
    {
        return type switch
        {
            EDiagonalType.TopRight => EDiagonalType.BottomLeft,
            EDiagonalType.BottomRight => EDiagonalType.TopLeft,
            EDiagonalType.BottomLeft => EDiagonalType.TopRight,
            EDiagonalType.TopLeft => EDiagonalType.BottomRight,
            _ => throw new System.ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}
