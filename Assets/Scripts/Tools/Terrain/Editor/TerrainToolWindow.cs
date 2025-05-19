using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

namespace BitBox.Tools.Terrain
{
    public class TerrainToolWindow : EditorWindow
    {
        private Toggle _activeToggle => rootVisualElement.Q<Toggle>("ActiveToggle");
        private SliderInt _sizeSlider => rootVisualElement.Q<SliderInt>("SizeSlider");
        private SliderInt _heightSlider => rootVisualElement.Q<SliderInt>("HeightSlider");
        
        [MenuItem("Tools/Terrain Tool")]
        public static void ShowWindow()
        {
            TerrainToolWindow window = GetWindow<TerrainToolWindow>();
            window.titleContent = new GUIContent("Terrain Tool");
        }

        private void InitializeData()
        {
            _activeToggle.value = TerrainToolSettings.Instance.IsActive;
            _sizeSlider.value = TerrainToolSettings.Instance.CurrentSize;
            _heightSlider.value = TerrainToolSettings.Instance.CurrentHeight;
        }

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualTreeAsset asset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Tools/Terrain/TerrainTool.uxml");
            asset.CloneTree(root);
            
            _activeToggle.RegisterValueChangedCallback(ToggleToolState);
            _sizeSlider.RegisterValueChangedCallback(ChangeSize);
            _heightSlider.RegisterValueChangedCallback(ChangeHeight);
            
            InitializeData();
        }
        
        private void ToggleToolState(ChangeEvent<bool> evt)
        {
            TerrainToolSettings.Instance.SetActive(evt.newValue);
        }

        private void ChangeSize(ChangeEvent<int> evt)
        {
            TerrainToolSettings.Instance.SetCurrentSize(evt.newValue);
        }
        
        private void ChangeHeight(ChangeEvent<int> evt)
        {
            TerrainToolSettings.Instance.SetCurrentHeight(evt.newValue);
        }
    }
}
