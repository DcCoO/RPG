using BitBox.Utils;

namespace BitBox.Tools.Terrain
{
    using System.Collections.Generic;
    using System.Text;
    using UnityEditor;
    using UnityEngine;
    
    [InitializeOnLoad]
    public static partial class TerrainTool
    {
        static TerrainTool()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }
        
        private static float _currentHeightMapHeight => TerrainToolSettings.Instance.CurrentHeight / 20f;
        

        private static float _currentWorldSpaceHeight => _terrain?.terrainData.heightmapScale.y * (TerrainToolSettings.Instance.CurrentHeight / 20f) ?? 0f;

        private static Terrain _terrain;
        

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (!TerrainToolSettings.Instance.IsActive) return;
            
            Event e = Event.current;

            if (e == null) return;

            TerrainCell cell;

            if (e.control || e.shift || e.alt)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (!hit.collider.gameObject.TryGetComponent(out _terrain)) return;
                    cell = new TerrainCell(hit.point, _terrain);
                }
                else return;
            }
            else return;
            
            //Handles.color = Color.red;
            //Handles.DrawWireCube(cellPosition, new Vector3(currentSize, _tileHeight, currentSize));

            if (e.shift) // Draw diagonals
            {
                var rayOrigin = cell.WorldSpace.Top + Vector3.up * 0.1f;
                var (diagonalType, heightmapValue) = CheckSurroundings(cell);
                if (e.type is not EventType.MouseDown) return; 
                if (diagonalType is EDirectionType.None) return;
                if (e.button is not 0) return;
                SetTerrainCellDiagonalHeight(_terrain, cell, diagonalType, heightmapValue);
            }

            else if (e.control) // Set height
            {
                Handles.color = Color.red;
                Handles.DrawWireCube(cell.WorldSpace.Bottom + Vector3.up * _currentWorldSpaceHeight / 2, new Vector3(cell.WorldSpace.Size, _currentWorldSpaceHeight, cell.WorldSpace.Size));

                for (int i = 1; i <= 8; ++i)
                {
                    var neighbour = cell.GetNeighbour((EDirectionType)i);
                    neighbour?.DrawGizmos(new Color(0, 1, 0, 0.4f));
                }
                
                if (e.type != EventType.MouseDown && e.type != EventType.MouseDrag) return;
                if (e.button is not 0 and not 1) return;
                SetTerrainCellHeight(_terrain, cell, e.button is 1);
                
                var heights = _terrain.terrainData.GetHeights(0, 0, 20, 20);
                StringBuilder sb = new();
                for (int z = 19; z >= 0; --z)
                {
                    for (int x = 0; x < 20; x++)
                    {
                        sb.Append($"{(heights[z, x] == 0 ? "o" : "x")}");
                    }
                    sb.AppendLine();
                }
                Debug.Log(sb.ToString());
                
            }
            
            else if (HandleConnection(e, cell)) {}

            if (e.type == EventType.MouseDown) e.Use();
        }
        
        private static (EDirectionType, float) CheckSurroundings(TerrainCell cell)
        {
            var forward = cell.GetNeighbour(EDirectionType.Forward);
            var back = cell.GetNeighbour(EDirectionType.Backward);
            var left = cell.GetNeighbour(EDirectionType.Left);
            var right = cell.GetNeighbour(EDirectionType.Right);

            var halfSize = cell.WorldSpace.Size / 2;

            Handles.color = Color.cyan;
            var availablePositions = new List<(Vector3, float)>();

            if (forward is not null && left is not null && cell.Heightmap.Value < forward.Heightmap.Value && cell.Heightmap.Value < left.Heightmap.Value)
            {
                var heightmapValue = Mathf.Min(forward.Heightmap.Value, left.Heightmap.Value);
                availablePositions.Add((cell.WorldSpace.Top + new Vector3(-halfSize, 0, halfSize), heightmapValue));
            }
            if (forward is not null && right is not null && cell.Heightmap.Value < forward.Heightmap.Value && cell.Heightmap.Value < right.Heightmap.Value)
            {
                var heightmapValue = Mathf.Min(forward.Heightmap.Value, right.Heightmap.Value);
                availablePositions.Add((cell.WorldSpace.Top + new Vector3(halfSize, 0, halfSize), heightmapValue));
            }
            if (back is not null && left is not null && cell.Heightmap.Value < back.Heightmap.Value && cell.Heightmap.Value < left.Heightmap.Value)
            {
                var heightmapValue = Mathf.Min(back.Heightmap.Value, left.Heightmap.Value);
                availablePositions.Add((cell.WorldSpace.Top + new Vector3(-halfSize, 0, -halfSize), heightmapValue));
            }
            if (back is not null && right is not null && cell.Heightmap.Value < back.Heightmap.Value && cell.Heightmap.Value < right.Heightmap.Value)
            {
                var heightmapValue = Mathf.Min(back.Heightmap.Value, right.Heightmap.Value);
                availablePositions.Add((cell.WorldSpace.Top + new Vector3(halfSize, 0, -halfSize), heightmapValue));
            }

            if (availablePositions.Count == 0) return (EDirectionType.None, 0f);

            var closestIndex = GetIndexOfClosestPoint(availablePositions);

            for (int i = 0; i < availablePositions.Count; ++i)
            {
                if (i == closestIndex) Handles.DrawWireCube(availablePositions[i].Item1, new Vector3(0.4f, 0.4f, 0.4f));
                else Handles.DrawWireCube(availablePositions[i].Item1, new Vector3(0.2f, 0.2f, 0.2f));
            }

            //find diagonal type and return by comparing the position of closest point and origin
            var closestPoint = availablePositions[closestIndex];
            if (closestPoint.Item1.x > cell.WorldSpace.Bottom.x && closestPoint.Item1.z > cell.WorldSpace.Bottom.z) return (EDirectionType.ForwardRight, closestPoint.Item2);
            if (closestPoint.Item1.x < cell.WorldSpace.Bottom.x && closestPoint.Item1.z > cell.WorldSpace.Bottom.z) return (EDirectionType.ForwardLeft, closestPoint.Item2);
            if (closestPoint.Item1.x < cell.WorldSpace.Bottom.x && closestPoint.Item1.z < cell.WorldSpace.Bottom.z) return (EDirectionType.BackwardLeft, closestPoint.Item2);
            return (EDirectionType.BackwardRight, closestPoint.Item2);
        }

        public static int GetIndexOfClosestPoint(List<(Vector3, float)> points)
        {
            int closestIndex = -1;
            float closestDistance = float.MaxValue;
            var mousePosition = Event.current.mousePosition;

            for (int i = 0; i < points.Count; i++)
            {
                var screenPosition = HandleUtility.WorldToGUIPoint(points[i].Item1);
                var distance = Vector2.Distance(mousePosition, screenPosition);
                if (distance < closestDistance)
                {
                    closestDistance = Vector2.Distance(mousePosition, screenPosition);
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        private static void LogHeightMap(Vector3 worldP1, Vector3 worldP2, Terrain terrain)
        {
            TerrainData terrainData = terrain.terrainData;

            Vector3 terrainSize = terrainData.size;
            Vector3 terrainPosition = terrain.transform.position;

            int heightmapWidth = terrainData.heightmapResolution;
            int heightmapHeight = terrainData.heightmapResolution;

            Vector2 normalizedP1 = new Vector2(
                Mathf.InverseLerp(terrainPosition.x, terrainPosition.x + terrainSize.x, worldP1.x),
                Mathf.InverseLerp(terrainPosition.z, terrainPosition.z + terrainSize.z, worldP1.z)
            );
            Vector2 normalizedP2 = new Vector2(
                Mathf.InverseLerp(terrainPosition.x, terrainPosition.x + terrainSize.x, worldP2.x),
                Mathf.InverseLerp(terrainPosition.z, terrainPosition.z + terrainSize.z, worldP2.z)
            );

            int x1 = Mathf.CeilToInt(normalizedP1.x * heightmapWidth);
            int z1 = Mathf.CeilToInt(normalizedP1.y * heightmapHeight);
            int x2 = Mathf.FloorToInt(normalizedP2.x * heightmapWidth);
            int z2 = Mathf.FloorToInt(normalizedP2.y * heightmapHeight);
            
            int xmin = Mathf.Min(x1, x2);
            int xmax = Mathf.Max(x1, x2);
            int zmin = Mathf.Min(z1, z2);
            int zmax = Mathf.Max(z1, z2);

            int width = xmax - xmin + 1;
            int height = zmax - zmin + 1;

            float[,] heights = terrainData.GetHeights(xmin, zmin, width, height);
            StringBuilder sb = new();
            
            for (int z = height - 1; z >= 0; z--)
            {
                for (int x = 0; x < width; x++)
                {
                    sb.Append($"{heights[z, x]} ");
                }

                sb.AppendLine();
            }

            Debug.Log(sb);
        }

        private static bool Raycast(Vector3 origin, Vector3 direction)
        {
            return Physics.Raycast(origin, direction, TerrainToolSettings.Instance.CurrentSize * 0.66f);
        }
    }
}