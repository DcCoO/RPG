using System.Collections.Generic;
using System;
using AYellowpaper.SerializedCollections;
using BitBox.Utils;
using UnityEngine;

namespace BitBox.AI
{
    /// <summary>
    ///  Registry class for storing and retrieving references for agents.
    /// </summary>
    public static class Registry
    {
        private static readonly SerializedDictionary<EAgent, List<GameObject>> _agents = new();
        
        public static void Add(EAgent agentType, GameObject agentGameObject)
        {
            if (!_agents.ContainsKey(agentType)) _agents[agentType] = new List<GameObject>();
            _agents[agentType].Add(agentGameObject);
        }
        
        public static void Remove(EAgent agentType, GameObject agentGameObject)
        {
            if (_agents.ContainsKey(agentType)) _agents[agentType].Remove(agentGameObject);
        }
        
        public static void RemoveAll(EAgent agent)
        {
            if (_agents.ContainsKey(agent)) _agents[agent].Clear();
        }
        
        public static GameObject Get(EAgent agentType)
        {
            if (_agents.TryGetValue(agentType, out List<GameObject> gameObjects))
            {
                if (gameObjects.Count > 0)
                {
                    return gameObjects[0];
                }
            }
            return null;
        }
        
        public static List<GameObject> GetAll(EAgent agentType)
        {
            return _agents.GetValueOrDefault(agentType);
        }
    }
}
