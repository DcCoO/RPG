using BitBox.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace BitBox.Tools.Terrain
{
    [CreateAssetMenu(fileName = "TerrainToolSettings", menuName = "Scriptable Objects/Terrain Tool Settings")]
    public class TerrainToolSettings : SingletonScriptableObject<TerrainToolSettings>
    {
        [field: SerializeField, ReadOnly] public bool IsActive { get; private set; }
        
        [Tooltip("If this size is N, it means the cells will occupy NxN slots in the heightmap matrix.")]
        [field: SerializeField, ReadOnly] public int CurrentSize { get; private set; }
        [field: SerializeField, ReadOnly] public int CurrentHeight { get; private set; }
        
        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            EditorUtility.SetDirty(this);
        }

        public void SetCurrentHeight(int height)
        {
            CurrentHeight = height;
            EditorUtility.SetDirty(this);
        }

        public void SetCurrentSize(int size)
        {
            CurrentSize = size;
            EditorUtility.SetDirty(this);
        }
    }
}
