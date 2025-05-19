using BitBox.Utils;
using UnityEditor;

namespace BitBox.Tools.Terrain
{
    using UnityEngine;
    
    public static partial class TerrainTool
    {
        
        private static EConnectionState _connectionState = EConnectionState.WaitingForFirstCell;
        private static TerrainCell _startCell;
        
        private static bool HandleConnection(Event e, TerrainCell cell)
        {
            if (!e.alt)
            {
                _connectionState = EConnectionState.WaitingForFirstCell;
                return false;
            }
            
            switch (_connectionState)
            {
                case EConnectionState.WaitingForFirstCell:
                    if (e.type == EventType.MouseDown && e.button == 0)
                    {
                        _startCell = cell;
                        _connectionState = EConnectionState.WaitingForSecondCell;
                    }
                    break;
                
                
                case EConnectionState.WaitingForSecondCell:
                    
                    _startCell.DrawGizmos(Color.white);
                    cell.DrawGizmos(Color.white);
                    
                    if (e.type == EventType.MouseUp && e.button == 0)
                    {
                        _connectionState = EConnectionState.WaitingForFirstCell;
                        SetTerrainRamp(_startCell, cell);
                    }
                    
                    break;
            }

            return true;
        }
        
        private static void SetTerrainRamp(TerrainCell startCell, TerrainCell endCell)
        {
            var distance = endCell.Heightmap.Index - startCell.Heightmap.Index;
            if (distance.x == 0 && distance.z == 0) return;
            if (distance.x == 0 && Mathf.Abs(distance.z) == 0) return;
            if (Mathf.Abs(distance.x) == 0 && distance.z == 0) return;

            bool isHorizontal = startCell.Heightmap.XMin != endCell.Heightmap.XMin;
            
            int xmin = Mathf.Min(startCell.Heightmap.XMin, endCell.Heightmap.XMin);
            int xmax = Mathf.Max(startCell.Heightmap.XMin + startCell.Heightmap.Size, endCell.Heightmap.XMin + endCell.Heightmap.Size);
            int zmin = Mathf.Min(startCell.Heightmap.ZMin, endCell.Heightmap.ZMin);
            int zmax = Mathf.Max(startCell.Heightmap.ZMin + startCell.Heightmap.Size, endCell.Heightmap.ZMin + endCell.Heightmap.Size);
            
            Debug.Log($"XMin: {xmin}, ZMin: {zmin}, XMax: {xmax}, ZMax: {zmax}, Horizontal: {isHorizontal}");

            int width = xmax - xmin;
            int length = zmax - zmin;

            var terrainData = _terrain.terrainData;

            float[,] heights = terrainData.GetHeights(xmin, zmin, width, length);
            
            var startHeight = heights[0, 0];
            var endHeight = heights[length - 1, width - 1];

            if (isHorizontal)   // Horizontal case
            {
                var left = startCell.Heightmap.Size - 1;
                var right = width - startCell.Heightmap.Size + 1;
                for (int z = 0; z < length; z++)
                {
                    for (int x = left; x < right; x++)
                    {
                        heights[z, x] = Mathf.Lerp(startHeight, endHeight, (x - left) / (float)(right - left - 1));
                    }
                }
            }
            else   // Vertical case
            {
                var bottom = startCell.Heightmap.Size;
                var top = length - startCell.Heightmap.Size;
                
                for (int z = bottom; z < top; z++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        heights[z, x] = Mathf.Lerp(startHeight, endHeight, (z - bottom) / (float)(top - bottom - 1));
                    }
                }
            }

            Undo.RegisterCompleteObjectUndo(terrainData, "Set Terrain Ramp");

            terrainData.SetHeights(xmin, zmin, heights);
        }
    }
}
