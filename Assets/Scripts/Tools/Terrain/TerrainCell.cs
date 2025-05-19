using System.Linq;
using BitBox.Utils;
using UnityEditor;

namespace BitBox.Tools.Terrain
{
    using UnityEngine;
    
    public class TerrainCell
    {
        public class WorldSpaceData
        {
            public Vector3 Top => Bottom + Vector3.up * Height;
            public Vector3 Center => Bottom + Vector3.up * (Height / 2);
            public Vector3 Bottom;
            
            public float Size;
            public float Height;
        }

        public class HeightmapData
        {
            public float Value;
            public Vector3Int Index;
            public int Size;
            public int XMin => Index.x * Size;
            public int ZMin => Index.z * Size;
        }
        
        public WorldSpaceData WorldSpace { get; private set; }
        public HeightmapData Heightmap { get; private set; }
        public Terrain Terrain { get; private set; }

        public TerrainCell(Vector3 hitPoint, Terrain terrain)
        {
            WorldSpace = new WorldSpaceData();
            Heightmap = new HeightmapData();
            Terrain = terrain;
            
            var currentSize = TerrainToolSettings.Instance.CurrentSize;
            
            hitPoint.y = terrain.transform.position.y;
            var terrainOrigin = terrain.transform.position;

            WorldSpace.Size = currentSize * terrain.terrainData.heightmapScale.x;
            Heightmap.Size = currentSize;
            
            var diff = hitPoint - terrainOrigin;
            Vector3Int cellIndex = Vector3Int.FloorToInt(diff / WorldSpace.Size);

            WorldSpace.Bottom = terrainOrigin + (cellIndex.ToVector3() + new Vector3(0.5f, 0f, 0.5f)) * WorldSpace.Size;
            Heightmap.Index = new Vector3Int(cellIndex.x, 0, cellIndex.z);
            Heightmap.Value = EvaluateHeightmapValue();
            WorldSpace.Height = Heightmap.Value * terrain.terrainData.heightmapScale.y;
        }
        
        public float[,] GetHeightmap()
        {
            var size = Heightmap.Size;
            return Terrain.terrainData.GetHeights(Heightmap.Index.x * size, Heightmap.Index.z * size, size, size);
        }

        private float EvaluateHeightmapValue()
        {
            var heights = GetHeightmap();
            return heights.Cast<float>().Max();
        }

        public TerrainCell GetNeighbour(EDirectionType directionType)
        {
            var neighbourIndex = directionType switch
            {
                EDirectionType.Forward => new Vector3Int(Heightmap.Index.x, 0, Heightmap.Index.z + 1),
                EDirectionType.ForwardRight => new Vector3Int(Heightmap.Index.x + 1, 0, Heightmap.Index.z + 1),
                EDirectionType.Right => new Vector3Int(Heightmap.Index.x + 1, 0, Heightmap.Index.z),
                EDirectionType.BackwardRight => new Vector3Int(Heightmap.Index.x + 1, 0, Heightmap.Index.z - 1),
                EDirectionType.Backward => new Vector3Int(Heightmap.Index.x, 0, Heightmap.Index.z - 1),
                EDirectionType.BackwardLeft => new Vector3Int(Heightmap.Index.x - 1, 0, Heightmap.Index.z - 1),
                EDirectionType.Left => new Vector3Int(Heightmap.Index.x - 1, 0, Heightmap.Index.z),
                EDirectionType.ForwardLeft => new Vector3Int(Heightmap.Index.x - 1, 0, Heightmap.Index.z + 1),
                _ => new Vector3Int(Heightmap.Index.x, 0, Heightmap.Index.z)
            };

            if (neighbourIndex.x < 0 || neighbourIndex.x >= Terrain.terrainData.heightmapResolution) return null;
            if (neighbourIndex.z < 0 || neighbourIndex.z >= Terrain.terrainData.heightmapResolution) return null;

            var direction = neighbourIndex - Heightmap.Index;

            return new TerrainCell(WorldSpace.Bottom + (Vector3)direction * WorldSpace.Size, Terrain);
        }

        public void DrawGizmos(Color color)
        {
            Handles.color = color;
            Handles.DrawWireCube(WorldSpace.Bottom + Vector3.up * WorldSpace.Height / 2, new Vector3(WorldSpace.Size, WorldSpace.Height, WorldSpace.Size) * 0.95f);
        }
    }
}
