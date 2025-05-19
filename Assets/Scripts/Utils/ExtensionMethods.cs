using System.Collections.Generic;
using UnityEngine;

namespace BitBox.Utils
{
    public static class ExtensionMethods
    {
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | (1 << layer));
        }

        public static Vector3 XY(this Vector3 vector)
        {
            return new Vector3(vector.x, vector.y, 0);
        }

        public static Vector3 XZ(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }

        /// <summary>Returns a float in the interval [x, y]</summary>
        public static float ValueInRange(this Vector2 vector)
        {
            return Random.Range(vector.x, vector.y);
        }

        /// <summary>Returns an integer in the interval [x, y]</summary>
        public static int ValueInRange(this Vector2Int vector)
        {
            return Random.Range(vector.x, vector.y + 1);
        }

        public static bool Between(this int value, int min, int max)
        {
            return min <= value && value <= max;
        }

        public static T RandomElement<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static T RandomElement<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static Vector2 ToVector2(this Vector2Int vector)
        {
            return vector;
        }
        
        public static Vector3 ToVector3(this Vector3Int vector)
        {
            return vector;
        }
        
        public static string Stringify<T>(this T[,] array)
        {
            var sb = new System.Text.StringBuilder();
            
            for (int i = array.GetLength(0) - 1; i >= 0; --i)
            {
                for (int j = 0; j < array.GetLength(1); ++j)
                {
                    sb.Append(array[i, j]);
                    if (j != array.GetLength(1) - 1)
                        sb.Append(" ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
