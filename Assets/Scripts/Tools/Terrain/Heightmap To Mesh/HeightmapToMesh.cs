using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitBox.Utils;
using UnityEditor;
using UnityEngine.Serialization;

public partial class HeightmapToMesh : MonoBehaviour
{
    public float _cellSize;

    /*public int[,] _heightmap =
    {
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,2,2,2,2,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,4,4,4,4,4,4,4,4,4,4,4,2,2,2,2,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,4,4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,4,4,4,4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,4,4,4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,4,4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {4,2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {2,2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {2,2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {2,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8},
        {8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8,8}

    };*/
    
    /*public int[,] _heightmap =
    {
        {4,4,4,2,2,2,2,8},
        {4,4,2,2,2,2,8,8},
        {2,2,2,8,8,8,8,8},
        {2,2,8,8,8,8,8,8},
        {2,8,8,8,8,8,8,8},
        {8,8,8,8,8,8,8,8},
        {8,8,8,8,8,8,8,8},
        {8,8,8,8,8,8,8,8}

    };*/
    
    public int[,] _heightmap =
    {
        {6, 6, 0},
        {6, 2, 4},
        {0, 4, 4},

    };
    
    //limitation: two diagonals cannot be adjacent

    public int Size => _heightmap.GetLength(0);
    
    public Material FloorMaterial;
    public Material WallMaterial;

    void Start()
    {
        GenerateMesh();
    }
    
    


    public void GenerateMesh()
    {
        // deve-se começar pelo mais alto
        // as diagonais dos mais altos podem interferir com os mais baixos
        // entao as diagonais devem ser salvas pois uma diagonal do mais alto cria uma diagonal oposta no mais baixo
        // as diagonais de todos os niveis tem q ser salvas para os niveis abaixo saberem
        // se numa celula que vai virar diagonal, ja existe uma celula mais alta ocupando, entao ela nao vira diagonal
        var existingHeights = GetExistingHeights(_heightmap);
        var upperDiagonals = new Dictionary<(int, int), DiagonalData>();
        foreach (var height in existingHeights)
        {
            var layer = FilterHeight(_heightmap, height);
            var diagonals = FindDiagonals(layer, height, upperDiagonals);

            StringBuilder sb = new();
            for (int i = 0; i < Size; ++i)
            {
                for (int j = 0; j < Size; ++j)
                {
                    sb.Append($"{layer[i, j]} ");
                } 
                sb.AppendLine();
            }
            Debug.Log($"{sb}");
            
            // Sometimes a full cell is forced to become diagonal by a higher height
            // so we set the cell to 0 because it is already stored in diagonals
            foreach (var diagonal in diagonals) layer[diagonal.Row, diagonal.Column] = 0;   // If a diagonal was created over a full cell, remove the full cell
            
            var rectangles = CoverWithRectangles(layer);
            ApplyDiagonals(layer, diagonals, true);
            PrintGrid(layer);
            var rectangleDatas = rectangles.Select(x => new RectangleData(x, layer)).ToList();
            BuildGraph(layer, height);
            ApplyDiagonals(layer, diagonals, false);
            //foreach (var rectangle in rectangleDatas) print(rectangle);
            BuildSurface(rectangleDatas, diagonals, height);
            //return;
            foreach (var diagonal in diagonals) upperDiagonals[(diagonal.Row, diagonal.Column)] = diagonal;
        }
    }

    private List<int> GetExistingHeights(int[,] heightmap)
    {
        HashSet<int> existingHeights = new HashSet<int>();
        
        for (int i = 0; i < Size; ++i)
        {
            for (int j = 0; j < Size; ++j)
            {
                if (heightmap[i, j] is 0) continue;
                existingHeights.Add(heightmap[i, j]);
            } 
        }
        
        return existingHeights.OrderByDescending(x => x).ToList();
    }

    private int[,] FilterHeight(int[,] heightmap, int height)
    {
        var filteredHeightmap = (int[,])heightmap.Clone();
        
        for (int i = 0; i < Size; ++i)
        {
            for (int j = 0; j < Size; ++j)
            {
                if (filteredHeightmap[i, j] == height) filteredHeightmap[i, j] = 1;
                else filteredHeightmap[i, j] = 0;
            }
        }
        return filteredHeightmap;
    }

    private List<DiagonalData> FindDiagonals(int[,] grid, int height, Dictionary<(int, int), DiagonalData> upperDiagonals)
    {
        var diagonals = new List<DiagonalData>();
        for (int i = 0; i < Size; ++i)
        {
            for (int j = 0; j < Size; ++j)
            {
                // If grid[i, j] is 1, there is a possible diagonal:
                // If the cell [i, j] is part of a diagonal formed by a higher height,
                // this height will become the other half of the diagonal and lock the cell
                if (grid[i, j] is 1)
                {
                    if (upperDiagonals.TryGetValue((i, j), out var diagonalData))   // This happens when a diagonal formed by an upper height forces the current height cell to also be diagonal)
                    {
                        diagonals.Add(new DiagonalData(i, j, height, diagonalData.Type.Opposite()));
                    }
                    continue;
                }
                
                //if (_heightmap[i, j] > height) continue;    // Only check for diagonals in cells that are not occupied by a higher height
                
                // If there is a possible diagonal, it is necessary to check if
                // the cell [i, j] is part of a diagonal formed by a higher angle.
                // If it is, the cell will be locked for the lower heights
                bool previousDiagonalData = upperDiagonals.TryGetValue((i, j), out var data);
                if (CheckDiagonal(grid, i, j, 1, 1))
                {
                    if (_heightmap[i, j] > height) continue; // Only check for diagonals in cells that are not occupied by a higher height
                    diagonals.Add(new DiagonalData(i, j, height, EDiagonalType.TopRight));
                    if (previousDiagonalData) _heightmap[i, j] = data.Height;   // Overwrite the height of the cell so that lower heights have no business with it
                }
                else if (CheckDiagonal(grid, i, j, 1, -1))
                {
                    if (_heightmap[i, j] > height) continue; // Only check for diagonals in cells that are not occupied by a higher height
                    diagonals.Add(new DiagonalData(i, j, height, EDiagonalType.TopLeft));
                    if (previousDiagonalData) _heightmap[i, j] = data.Height;   // Overwrite the height of the cell so that lower heights have no business with it
                }
                else if (CheckDiagonal(grid, i, j, -1, 1))
                {
                    if (_heightmap[i, j] > height) continue; // Only check for diagonals in cells that are not occupied by a higher height
                    diagonals.Add(new DiagonalData(i, j, height, EDiagonalType.BottomRight));
                    if (previousDiagonalData) _heightmap[i, j] = data.Height;   // Overwrite the height of the cell so that lower heights have no business with it
                }
                else if (CheckDiagonal(grid, i, j, -1, -1))
                {
                    if (_heightmap[i, j] > height) continue; // Only check for diagonals in cells that are not occupied by a higher height
                    diagonals.Add(new DiagonalData(i, j, height, EDiagonalType.BottomLeft));
                    if (previousDiagonalData) _heightmap[i, j] = data.Height;   // Overwrite the height of the cell so that lower heights have no business with it
                }
            }
        }

        return diagonals;
    }

    private bool CheckDiagonal(int[,] heightmap, int i, int j, int di, int dj)
    {
        var positionsOf1 = new HashSet<(int, int)> { (i + di, j), (i, j + dj), (i + di, j + dj) };
        var positionsOf0 = new HashSet<(int, int)> { (i - di, j), (i, j - dj) };

        foreach (var position in positionsOf1)
        {
            if (position.Item1 < 0 || position.Item1 >= Size || position.Item2 < 0 || position.Item2 >= Size) return false;
            if (heightmap[position.Item1, position.Item2] != 1) return false;
        }
        
        foreach (var position in positionsOf0)
        {
            if (position.Item1 < 0 || position.Item1 >= Size || position.Item2 < 0 || position.Item2 >= Size) continue;
            if (heightmap[position.Item1, position.Item2] != 0) return false;
        }

        return true;
    }

    private void ApplyDiagonals(int[,] grid, List<DiagonalData> diagonals, bool apply)
    {
        foreach (var diagonal in diagonals) grid[diagonal.Row, diagonal.Column] = apply ? (int)diagonal.Type : 0;
    }
    
    private List<RectInt> CoverWithRectangles(int[,] map)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        List<RectInt> rectangles = new();

        int[,] temp = (int[,])map.Clone();

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                if (temp[y, x] == 1)
                {
                    int maxWidth = 0;
                    while (x + maxWidth < cols && temp[y, x + maxWidth] == 1)
                        maxWidth++;

                    int maxHeight = 0;
                    bool valid = true;
                    while (y + maxHeight < rows && valid)
                    {
                        for (int i = 0; i < maxWidth; i++)
                        {
                            if (temp[y + maxHeight, x + i] != 1)
                            {
                                valid = false;
                                break;
                            }
                        }
                        if (valid) maxHeight++;
                    }

                    for (int dy = 0; dy < maxHeight; dy++)
                        for (int dx = 0; dx < maxWidth; dx++)
                            temp[y + dy, x + dx] = 0;

                    rectangles.Add(new RectInt(x, y, maxWidth, maxHeight));
                }
            }
        }

        return rectangles;
    }

    private void BuildSurface(List<RectangleData> rectangles, List<DiagonalData> diagonals, int height)
    {
        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        int vertexIndex = 0;

        Dictionary<(int, int, int), int> vertexIndexMap = new();
        Action<(int, int, int)> AddVertex = v =>
        {
            if (!vertexIndexMap.ContainsKey(v))
            {
                vertices.Add(new Vector3(v.Item1, v.Item2, v.Item3));
                vertexIndexMap[v] = vertexIndex++;
            }
        };
        
        foreach (var rect in rectangles)
        {
            int x = rect.X;
            int z = rect.Y;
            int w = rect.Width;
            int h = rect.Height;

            var v0 = (x, height, z);
            var v1 = (x + w, height, z);
            var v2 = (x + w, height, z + h);
            var v3 = (x, height, z + h);

            AddVertex(v0);
            AddVertex(v1);
            AddVertex(v2);
            AddVertex(v3);
            
            triangles.Add(vertexIndexMap[v0]);
            triangles.Add(vertexIndexMap[v2]);
            triangles.Add(vertexIndexMap[v1]);

            triangles.Add(vertexIndexMap[v2]);
            triangles.Add(vertexIndexMap[v0]);
            triangles.Add(vertexIndexMap[v3]);
        }

        // 2. Adiciona as diagonais como triângulos
        foreach (var d in diagonals)
        {
            int x = d.Column;
            int y = d.Row;

            Vector3Int point0, point1, point2;

            switch (d.Type)
            {
                case EDiagonalType.TopRight:
                    point0 = new Vector3Int(x, 0, y + 1);     // top-left
                    point1 = new Vector3Int(x + 1, 0, y);     // bottom-right
                    point2 = new Vector3Int(x + 1, 0, y + 1); // top-right
                    break;

                case EDiagonalType.BottomRight:
                    point0 = new Vector3Int(x + 1, 0, y);     // bottom-right
                    point1 = new Vector3Int(x + 1, 0, y + 1); // top-right
                    point2 = new Vector3Int(x, 0, y);         // bottom-left
                    break;

                case EDiagonalType.BottomLeft:
                    point0 = new Vector3Int(x + 1, 0, y);     // bottom-right
                    point1 = new Vector3Int(x, 0, y + 1);     // top-left
                    point2 = new Vector3Int(x, 0, y);         // bottom-left
                    break;

                case EDiagonalType.TopLeft:
                    point0 = new Vector3Int(x, 0, y + 1);     // top-left
                    point1 = new Vector3Int(x, 0, y);         // bottom-left
                    point2 = new Vector3Int(x + 1, 0, y + 1); // top-right
                    break;

                default:
                    continue;
            }

            var v0 = (point0.x, height, point0.z);
            var v1 = (point1.x, height, point1.z);
            var v2 = (point2.x, height, point2.z);
            
            AddVertex(v0);
            AddVertex(v1);
            AddVertex(v2);
            
            triangles.Add(vertexIndexMap[v0]);
            triangles.Add(vertexIndexMap[v2]);
            triangles.Add(vertexIndexMap[v1]);
        }
        
        if (vertices.Count == 0 || triangles.Count == 0)
        {
            Debug.Log("No vertices or triangles to create a mesh.");
            return;
        }

        Mesh mesh = new Mesh();
        GameObject surface = new GameObject($"[{height}] Surface");
        surface.transform.parent = transform;
        surface.transform.localPosition = Vector3.zero;
        mesh.vertices = vertices.Select(x => x * _cellSize).ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        surface.AddComponent<MeshFilter>().mesh = mesh;
        surface.AddComponent<MeshRenderer>().sharedMaterial = FloorMaterial;
    }
    
    private void PrintGrid(int[,] grid)
    {
        var sb = new System.Text.StringBuilder();
            
        for (int i = grid.GetLength(0) - 1; i >= 0; --i)
        {
            for (int j = 0; j < grid.GetLength(1); ++j)
            {
                if (grid[i, j] == 0) sb.Append("⎕");
                else if (grid[i, j] == 1) sb.Append("█");
                else if (grid[i, j] == (int)EDiagonalType.BottomLeft) sb.Append("◣");
                else if (grid[i, j] == (int)EDiagonalType.BottomRight) sb.Append("◢");
                else if (grid[i, j] == (int)EDiagonalType.TopLeft) sb.Append("◤");
                else if (grid[i, j] == (int)EDiagonalType.TopRight) sb.Append("◥");
                else sb.Append(grid[i, j]);
                if (j != grid.GetLength(1) - 1)
                    sb.Append(" ");
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }
    
}