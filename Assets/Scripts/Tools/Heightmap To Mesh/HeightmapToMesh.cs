using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitBox.Utils;
using UnityEditor;
using UnityEngine.Serialization;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public partial class HeightmapToMesh : MonoBehaviour
{
    public float _cellSize;
    /*public int[,] _heightmap = {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 5, 5, 5, 0, 0},
        {0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 0},
        {0, 0, 0, 0, 0, 5, 5, 5, 5, 5, 0},
        {0, 1, 1, 1, 1, 5, 5, 5, 5, 5, 0},
        {0, 0, 1, 1, 1, 5, 5, 5, 5, 5, 0},
        {0, 1, 1, 1, 1, 3, 3, 3, 3, 3, 0},
        {0, 0, 1, 1, 1, 3, 3, 3, 3, 0, 0},
        {0, 0, 0, 0, 0, 3, 3, 3, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
    };*/
    
    public int[,] _heightmap = 
    {
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0},
        {0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0},
        {0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,1,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,1,1,1,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,1,0,1,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,1,1,1,0,0,0,0,1,1,1,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,1,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
    };
    
    //limitation: two diagonals cannot be adjacent

    public int Size => 25;

    void Start()
    {
        GenerateMesh(_heightmap);
    }
    
    


    public void GenerateMesh(int[,] heightmap)
    {
        // deve-se começar pelo mais alto
        // as diagonais dos mais altos podem interferir com os mais baixos
        // entao as diagonais devem ser salvas pois uma diagonal do mais alto cria uma diagonal oposta no mais baixo
        // as diagonais de todos os niveis tem q ser salvas para os niveis abaixo saberem
        // se numa celula que vai virar diagonal, ja existe uma celula mais alta ocupando, entao ela nao vira diagonal
        var existingHeights = GetExistingHeights(heightmap);
        foreach (var height in existingHeights)
        {
            var layer = FilterHeight(heightmap, height);
            var diagonals = FindDiagonals(layer);
            var rectangles = CoverWithRectangles(layer);
            ApplyDiagonals(layer, diagonals, true);
            PrintGrid(layer);
            var rectangleDatas = rectangles.Select(x => new RectangleData(x, layer)).ToList();
            BuildGraph(layer);
            ApplyDiagonals(layer, diagonals, false);
            //foreach (var rectangle in rectangleDatas) print(rectangle);
            BuildSurface(rectangleDatas, diagonals, height);
            return;
        }
    }

    private List<int> GetExistingHeights(int[,] heightmap)
    {
        HashSet<int> existingHeights = new HashSet<int>();
        
        for (int i = 0; i < Size; ++i)
        {
            for (int j = 0; j < Size; ++j)
            {
                if (heightmap[i, j] == 0) continue;
                existingHeights.Add(heightmap[i, j]);
            } 
        }
        
        return existingHeights.OrderBy(x => x).ToList();
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

    private List<DiagonalData> FindDiagonals(int[,] heightmap)
    {
        var diagonals = new List<DiagonalData>();
        for (int i = 0; i < Size; ++i)
        {
            for (int j = 0; j < Size; ++j)
            {
                if (heightmap[i, j] != 0) continue;

                if (i == 8 && j == 4)
                {
                    StringBuilder sb = new();
                    for (int r = -1; r <= 1; ++r)
                    {
                        for (int c = -1; c <= 1; ++c)
                        {
                            sb.Append(heightmap[i + r, j + c]);
                        }

                        sb.AppendLine();
                    }
                    print(sb.ToString());
                    print(CheckDiagonal(heightmap, i, j, 1, 1));
                }
                
                if (CheckDiagonal(heightmap, i, j, 1, 1)) diagonals.Add(new DiagonalData(i, j, EDiagonalType.TopRight));
                else if (CheckDiagonal(heightmap, i, j, 1, -1)) diagonals.Add(new DiagonalData(i, j, EDiagonalType.TopLeft));
                else if (CheckDiagonal(heightmap, i, j, -1, 1)) diagonals.Add(new DiagonalData(i, j, EDiagonalType.BottomRight));
                else if (CheckDiagonal(heightmap, i, j, -1, -1)) diagonals.Add(new DiagonalData(i, j, EDiagonalType.BottomLeft));
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

        Mesh mesh = new Mesh();
        GameObject surface = new GameObject($"[{height}] Surface");
        surface.transform.parent = transform;
        surface.transform.localPosition = Vector3.zero;
        mesh.vertices = vertices.Select(x => x * _cellSize).ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        surface.AddComponent<MeshFilter>().mesh = mesh;
        surface.AddComponent<MeshRenderer>().sharedMaterial = GetComponent<MeshRenderer>().sharedMaterial;
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