using System.Text;
using UnityEditor;

namespace BitBox.Tools.Terrain
{
    using UnityEngine;
    
    public static partial class TerrainTool
    {
        private static void SetTerrainCellHeight(Terrain terrain, TerrainCell cell, bool erase)
        {
            var terrainData = terrain.terrainData;
            
            float[,] heights = cell.GetHeightmap();

            for (int z = 0; z < cell.Heightmap.Size; z++)
            {
                for (int x = 0; x < cell.Heightmap.Size; x++)
                {
                    heights[z, x] = erase ? 0f : _currentHeightMapHeight;
                }
            }

            Undo.RegisterCompleteObjectUndo(terrainData, "Set Terrain Cell Height");
            
            terrainData.SetHeights(cell.Heightmap.Index.x * cell.Heightmap.Size, cell.Heightmap.Index.z * cell.Heightmap.Size, heights);
        }
        
        private static void SetTerrainCellDiagonalHeight(Terrain terrain, TerrainCell cell, EDirectionType directionType, float heightmapValue)
        {
            TerrainData terrainData = terrain.terrainData;
            var cellOriginalHeightmapValue = cell.Heightmap.Value;

            float[,] heights = cell.GetHeightmap();

            for (int z = 0; z < cell.Heightmap.Size; z++)
            {
                for (int x = 0; x < cell.Heightmap.Size; x++)
                {
                    bool shouldModify = false;

                    switch (directionType)
                    {
                        case EDirectionType.BackwardLeft:
                            shouldModify = (x <= (cell.Heightmap.Size - 1 - z));
                            break;
                        case EDirectionType.BackwardRight:
                            shouldModify = (x >= z);
                            break;
                        case EDirectionType.ForwardLeft:
                            shouldModify = (x <= z);
                            break;
                        case EDirectionType.ForwardRight:
                            shouldModify = (x >= (cell.Heightmap.Size - 1 - z));
                            break;
                    }

                    if (shouldModify)
                        heights[z, x] = heightmapValue;

                }
            }

            /*if (directionType is EDirectionType.BackwardLeft or EDirectionType.ForwardRight)
            {
                var average = (heightmapValue + cellOriginalHeightmapValue) / 2f;
                Debug.Log($"{directionType} at height {heightmapValue} and average {average}");
                int dx = directionType is EDirectionType.BackwardLeft ? 1 : -1;
                for (int z = 0; z < cell.Heightmap.Size; z++)
                {
                    for (int x = 0; x < cell.Heightmap.Size; x++)
                    {
                        if (!Mathf.Approximately(heights[z, x], heightmapValue)) continue;
                        if (x + dx < 0 || x + dx >= cell.Heightmap.Size) continue;
                        if (Mathf.Approximately(heights[z, x + dx], heightmapValue)) continue;
                        heights[z, x + dx] = Mathf.Lerp(cellOriginalHeightmapValue, heightmapValue, 0.8f);
                        break;
                    }
                }
            }*/

            StringBuilder sb = new();
            for (int z = cell.Heightmap.Size - 1; z >= 0; --z)
            {
                for (int x = 0; x < cell.Heightmap.Size; x++)
                {
                    sb.Append(heights[z, x]);
                    sb.Append(" ");
                }
                sb.AppendLine();
            }
            
            Debug.Log(sb.ToString());

            Undo.RegisterCompleteObjectUndo(terrainData, "Set Terrain Cell Diagonal Height");
            terrainData.SetHeights(cell.Heightmap.Index.x * cell.Heightmap.Size, cell.Heightmap.Index.z * cell.Heightmap.Size, heights);
        }

        private static float GetHeightmapValueForDiagonal(Vector3 center, EDirectionType directionType) => directionType switch
        {
            EDirectionType.BackwardLeft or EDirectionType.BackwardRight => _terrain.SampleHeight(center + Vector3.back * TerrainToolSettings.Instance.CurrentSize),
            EDirectionType.ForwardLeft or EDirectionType.ForwardRight => _terrain.SampleHeight(center + Vector3.forward * TerrainToolSettings.Instance.CurrentSize),
            _ => 0
        };
    }
}