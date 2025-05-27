using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class HeightmapToMesh
{
    public class Node
    {
        public readonly int Id;
        public readonly int Row; // Linha
        public readonly int Column; // Coluna
        public readonly Dictionary<(int, int), Node> Neighbors;

        public Node(int id, int row, int column)
        {
            Id = id;
            Row = row;
            Column = column;
            Neighbors = new();
        }
        
        private static int _id = 0;
        public static int GetId() => _id++;
        
        public static void ResetId() => _id = 0;
    }

    private Dictionary<(int, int), Node> _graph;

    private void OnDrawGizmos()
    {
        if (_graph == null) return;

        foreach (var kvp in _graph)
        {
            var node = kvp.Value;
            Gizmos.color = node.Neighbors.Count > 0 ? Color.white : Color.red;

            Gizmos.DrawSphere(new Vector3(node.Column, 0, node.Row), 0.1f);

            foreach (var neighbor in node.Neighbors.Values)
            {
                Gizmos.DrawLine(
                    new Vector3(node.Column, 0, node.Row),
                    new Vector3(neighbor.Column, 0, neighbor.Row)
                );
            }
        }
    }

    private void BuildGraph(int[,] grid, int height)
    {
        _graph = new();
        Node.ResetId();

        for (int j = 0; j < Size; ++j) // linha
        {
            for (int i = 0; i < Size; ++i) // coluna
            {
                int value = grid[j, i]; // acesso como grid[linha, coluna]

                if (value == 0) continue;

                void TryAddNode(int col, int row)
                {
                    var key = (row, col); // chave = (j, i)
                    if (!_graph.ContainsKey(key))
                    {
                        _graph[key] = new Node(Node.GetId(), row, col);
                        // Debug.Log($"Criando nó ({row}, {col})");
                    }
                }

                if (value == 1)
                {
                    TryAddNode(i, j);
                    TryAddNode(i, j + 1);
                    TryAddNode(i + 1, j);
                    TryAddNode(i + 1, j + 1);
                }
                else if (value == (int)EDiagonalType.BottomLeft)
                {
                    TryAddNode(i, j);     
                    TryAddNode(i + 1, j);     
                    TryAddNode(i, j + 1); 

                    var aKey = (j + 1, i);
                    var bKey = (j, i + 1);

                    if (_graph.TryGetValue(aKey, out var nodeA) && _graph.TryGetValue(bKey, out var nodeB))
                    {
                        nodeA.Neighbors[bKey] = nodeB;
                        nodeB.Neighbors[aKey] = nodeA;
                    }
                }
                else if (value == (int)EDiagonalType.BottomRight)
                {
                    TryAddNode(i, j);     
                    TryAddNode(i + 1, j);     
                    TryAddNode(i + 1, j + 1); 

                    var aKey = (j, i);
                    var bKey = (j + 1, i + 1);

                    if (_graph.TryGetValue(aKey, out var nodeA) && _graph.TryGetValue(bKey, out var nodeB))
                    {
                        nodeA.Neighbors[bKey] = nodeB;
                        nodeB.Neighbors[aKey] = nodeA;
                    }
                }
                else if (value == (int)EDiagonalType.TopRight)
                {
                    TryAddNode(i, j + 1);
                    TryAddNode(i + 1, j);
                    TryAddNode(i + 1, j + 1);

                    var aKey = (j + 1, i);
                    var bKey = (j, i + 1);

                    if (_graph.TryGetValue(aKey, out var nodeA) && _graph.TryGetValue(bKey, out var nodeB))
                    {
                        nodeA.Neighbors[bKey] = nodeB;
                        nodeB.Neighbors[aKey] = nodeA;
                    }
                }
                else if (value == (int)EDiagonalType.TopLeft)
                {
                    TryAddNode(i, j);     
                    TryAddNode(i, j + 1);     
                    TryAddNode(i + 1, j + 1); 

                    var aKey = (j, i);
                    var bKey = (j + 1, i + 1);

                    if (_graph.TryGetValue(aKey, out var nodeA) && _graph.TryGetValue(bKey, out var nodeB))
                    {
                        nodeA.Neighbors[bKey] = nodeB;
                        nodeB.Neighbors[aKey] = nodeA;
                    }
                }
            }
        }
        
        if (_graph.Count is 0)
        {
            Debug.Log("No nodes were created in the graph. Check the grid data.");
            return;
        }

        FillAdjacentyList();
        RemoveInternalEdges(grid);
        RemoveUselessNodes();
        BuildBorders(grid, height);
    }
    
    private void FillAdjacentyList()
    {
        foreach (var node in _graph.Values)
        {
            var i = node.Column;
            var j = node.Row;
            if (_graph.TryGetValue((j, i - 1), out var left)) node.Neighbors[(j, i - 1)] = left;
            if (_graph.TryGetValue((j, i + 1), out var right)) node.Neighbors[(j, i + 1)] = right;
            if (_graph.TryGetValue((j - 1, i), out var bottom)) node.Neighbors[(j - 1, i)] = bottom;
            if (_graph.TryGetValue((j + 1, i), out var top)) node.Neighbors[(j + 1, i)] = top;
        }
    }

    private void RemoveInternalEdges(int[,] grid)
    {
        for (int j = 0; j < Size; ++j) 
        {
            for (int i = 0; i < Size; ++i)
            {
                if (!_graph.TryGetValue((j, i), out var node)) continue;
                RemoveInternalEdges(node, grid);
            }
        }
    }
    
    private void RemoveUselessNodes()
    {
        for (int j = 0; j < Size; ++j) 
        {
            for (int i = 0; i < Size; ++i)
            {
                if (!_graph.TryGetValue((j, i), out var node)) continue;
                RemoveUselessNodes(node);
            }
        }
    }

    private void RemoveInternalEdges(Node node, int[,] grid)
    {
        var position = (node.Row, node.Column);
        
        var topNeighbor = node.Neighbors.Values.FirstOrDefault(neighbor => neighbor.Column == node.Column && neighbor.Row > node.Row);

        if (topNeighbor is not null)
        {
            var column = position.Column;
            bool willRemove = true;
            for (int row = node.Row; row < topNeighbor.Row; ++row)
            {
                var left = (row, column - 1);
                var right = (row, column);
                
                // Check if exists
                bool leftIsEmpty = (left.Item1 < 0 || left.Item1 >= Size || left.Item2 < 0 || left.Item2 >= Size) || grid[left.Item1, left.Item2] is 0;
                bool rightIsEmpty = (right.Item1 < 0 || right.Item1 >= Size || right.Item2 < 0 || right.Item2 >= Size) || grid[right.Item1, right.Item2] is 0;
                if (leftIsEmpty != rightIsEmpty)
                {
                    willRemove = false;
                    break;
                }
            }

            if (willRemove)
            {
                node.Neighbors.Remove((topNeighbor.Row, topNeighbor.Column));
                topNeighbor.Neighbors.Remove((node.Row, node.Column));
            }
        }
        
        var rightNeighbor = node.Neighbors.Values.FirstOrDefault(neighbor => neighbor.Row == node.Row && neighbor.Column > node.Column);
        
        if (rightNeighbor is not null)
        {
            var row = position.Row;
            bool willRemove = true;
            for (int column = node.Column; column < rightNeighbor.Column; ++column)
            {
                var top = (row, column);
                var bottom = (row - 1, column);
                
                // Check if exists
                bool topIsEmpty = (top.Item1 < 0 || top.Item1 >= Size || top.Item2 < 0 || top.Item2 >= Size) || grid[top.Item1, top.Item2] is 0;
                bool bottomIsEmpty = (bottom.Item1 < 0 || bottom.Item1 >= Size || bottom.Item2 < 0 || bottom.Item2 >= Size) || grid[bottom.Item1, bottom.Item2] is 0;
                if (topIsEmpty != bottomIsEmpty)
                {
                    willRemove = false;
                    break;
                }
            }

            if (willRemove)
            {
                node.Neighbors.Remove((rightNeighbor.Row, rightNeighbor.Column));
                rightNeighbor.Neighbors.Remove((node.Row, node.Column));
            }
        }
        
        if (node.Neighbors.Count is 0)
        {
            _graph.Remove(position);
        }
        else if (node.Neighbors.Count is 4)
        {
            SplitNode(node, grid);
        }
    }
    
    private void SplitNode(Node node, int[,] grid)
    {
        Node newNode = new Node( Node.GetId(), node.Row, node.Column);
        
        var topNeighbor = node.Neighbors.Values.First(neighbor => neighbor.Column == node.Column && neighbor.Row > node.Row);
        
        node.Neighbors.Remove((topNeighbor.Row, topNeighbor.Column));
        topNeighbor.Neighbors.Remove((node.Row, node.Column));
        
        newNode.Neighbors[(topNeighbor.Row, topNeighbor.Column)] = topNeighbor;
        topNeighbor.Neighbors[(newNode.Row, newNode.Column)] = newNode;
        
        if (grid[node.Row, node.Column] is 1)
        {
            var rightNeighbor = node.Neighbors.Values.First(neighbor => neighbor.Row == node.Row && neighbor.Column > node.Column);
            
            node.Neighbors.Remove((rightNeighbor.Row, rightNeighbor.Column));
            rightNeighbor.Neighbors.Remove((node.Row, node.Column));
            
            newNode.Neighbors[(rightNeighbor.Row, rightNeighbor.Column)] = rightNeighbor;
            rightNeighbor.Neighbors[(newNode.Row, newNode.Column)] = newNode;
        }
        else
        {
            var leftNeighbor = node.Neighbors.Values.First(neighbor => neighbor.Row == node.Row && neighbor.Column < node.Column);
            
            node.Neighbors.Remove((leftNeighbor.Row, leftNeighbor.Column));
            leftNeighbor.Neighbors.Remove((node.Row, node.Column));
            
            newNode.Neighbors[(leftNeighbor.Row, leftNeighbor.Column)] = leftNeighbor;
            leftNeighbor.Neighbors[(newNode.Row, newNode.Column)] = newNode;
        }
    }

    private void RemoveUselessNodes(Node node)
    {
        var position = (node.Row, node.Column);
        
        var topNeighbor = node.Neighbors.Values.FirstOrDefault(neighbor => neighbor.Column == node.Column && neighbor.Row > node.Row);
        var bottomNeighbor = node.Neighbors.Values.FirstOrDefault(neighbor => neighbor.Column == node.Column && neighbor.Row < node.Row);

        if (topNeighbor is not null && bottomNeighbor is not null)
        {
            topNeighbor.Neighbors.Remove(position);
            bottomNeighbor.Neighbors.Remove(position);
            
            topNeighbor.Neighbors[(bottomNeighbor.Row, bottomNeighbor.Column)] = bottomNeighbor;
            bottomNeighbor.Neighbors[(topNeighbor.Row, topNeighbor.Column)] = topNeighbor;
            
            _graph.Remove(position);
        }
        
        var rightNeighbor = node.Neighbors.Values.FirstOrDefault(neighbor => neighbor.Row == node.Row && neighbor.Column > node.Column);
        var leftNeighbor = node.Neighbors.Values.FirstOrDefault(neighbor => neighbor.Row == node.Row && neighbor.Column < node.Column);
        
        if (rightNeighbor is not null && leftNeighbor is not null)
        {
            rightNeighbor.Neighbors.Remove(position);
            leftNeighbor.Neighbors.Remove(position);
            
            rightNeighbor.Neighbors[(leftNeighbor.Row, leftNeighbor.Column)] = leftNeighbor;
            leftNeighbor.Neighbors[(rightNeighbor.Row, rightNeighbor.Column)] = rightNeighbor;
            
            _graph.Remove(position);
        }
    }
    
    private void BuildBorders(int[,] grid, int height)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new();
        List<int> triangles = new();
        
        var mark = new Dictionary<int, bool>();
        
        foreach (var node in _graph.Values)
        {
            if (mark.ContainsKey(node.Id)) continue;
            //print($"ORIGIN AT ({node.Row}, {node.Column}) = {grid[node.Row, node.Column]}");
            bool clockwise = GetTriangleDirection(node, grid);
            BuildBorders(vertices, triangles, node, mark, height, clockwise);
        }
        
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        GameObject border = new GameObject($"[{height}] Border");
        border.transform.parent = transform;
        border.transform.localPosition = Vector3.zero;
        
        border.AddComponent<MeshFilter>().mesh = mesh;
        border.AddComponent<MeshRenderer>().sharedMaterial = WallMaterial;
    }

    private bool GetTriangleDirection(Node u, int[,] grid)
    {
        var v = u.Neighbors.Values.First();
        
        if (u.Row == v.Row)
        {
            return grid[u.Row, Mathf.Min(u.Column, v.Column)] is 0;
        }
        
        if (u.Column == v.Column)
        {
            return grid[Mathf.Min(u.Row, v.Row), u.Column] is 0;
        }

        
        if (u.Row < v.Row)
        {
            if (u.Column < v.Column)
            {
                //print("(1) AAAAAAAA " + u.Row + ", " + u.Column + " -> " + v.Row + ", " + v.Column);
                return grid[u.Row, u.Column] is (int)EDiagonalType.BottomRight;
            }
            else
            {
                //print("(2) AAAAAAAA " + u.Row + ", " + u.Column + " -> " + v.Row + ", " + v.Column);
                return grid[u.Row, u.Column - 1] is (int)EDiagonalType.TopRight;
            }
        }
        else
        {
            if (u.Column < v.Column)
            {
                //print("(3) AAAAAAAA " + u.Row + ", " + u.Column + " -> " + v.Row + ", " + v.Column);
                return grid[v.Row, u.Column] is (int)EDiagonalType.BottomLeft;
            }
            else
            {
                //print("(4) AAAAAAAA " + u.Row + ", " + u.Column + " -> " + v.Row + ", " + v.Column);
                return grid[v.Row, v.Column] is (int)EDiagonalType.TopRight;
            }
        }
    }
    
    private void BuildBorders(List<Vector3> vertices, List<int> triangles, Node origin, Dictionary<int, bool> mark, float height, bool clockwise)
    {
        mark[origin.Id] = true;
        
        Node next;
        Node prev = origin;
        
        while (true)
        {
            next = prev.Neighbors.Values.FirstOrDefault(u => !mark.ContainsKey(u.Id));
            if (next is null)
            {
                BuildRectangle(prev, origin, height, vertices, triangles, clockwise);
                break;
            }
            
            mark[next.Id] = true;
            BuildRectangle(prev, next, height, vertices, triangles, clockwise);
            prev = next;
        }
    }
    
    private void BuildRectangle(Node prev, Node next, float height, List<Vector3> vertices, List<int> triangles, bool clockwise)
    {
        Vector3 bottomLeft = new Vector3(prev.Column, 0, prev.Row);
        Vector3 bottomRight = new Vector3(next.Column, 0, next.Row);
        Vector3 topLeft = new Vector3(prev.Column, height, prev.Row);
        Vector3 topRight = new Vector3(next.Column, height, next.Row);

        // Index dos vértices no array
        int baseIndex = vertices.Count;

        // Adiciona os 4 vértices do quadrado da parede
        vertices.Add(bottomLeft);  // 0
        vertices.Add(bottomRight); // 1
        vertices.Add(topLeft);     // 2
        vertices.Add(topRight);    // 3

        if (clockwise)
        {
            triangles.Add(baseIndex + 0);
            triangles.Add(baseIndex + 1);
            triangles.Add(baseIndex + 2);

            triangles.Add(baseIndex + 3);
            triangles.Add(baseIndex + 2);
            triangles.Add(baseIndex + 1);
        }
        else
        {
            triangles.Add(baseIndex + 0);
            triangles.Add(baseIndex + 2);
            triangles.Add(baseIndex + 1);

            triangles.Add(baseIndex + 2);
            triangles.Add(baseIndex + 3);
            triangles.Add(baseIndex + 1);    
        }
    }
}
