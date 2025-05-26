using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class HeightmapToMesh
{
    public class Node
    {
        public readonly int Row; // Linha
        public readonly int Column; // Coluna
        public readonly Dictionary<(int, int), Node> Neighbors;

        public Node(int row, int column)
        {
            Row = row;
            Column = column;
            Neighbors = new();
        }
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

    private void BuildGraph(int[,] grid)
    {
        _graph = new();

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
                        _graph[key] = new Node(row, col);
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

        Debug.Log("Grafo criado com " + _graph.Count + " nodes");
        FillAdjacentyList();
        RemoveInternalEdges(grid);
        RemoveUselessNodes();
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
        Node newNode = new Node(node.Row, node.Column);
        
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
}
