using UnityEngine;
using UnityEngine.UI;

namespace BitBox.Rendering
{
    public class RenderTextureGenerator : MonoBehaviour
    {
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private int _heightPixels = 360;

        private RenderTexture _renderTexture;

        private void Start()
        {
#if UNITY_EDITOR
            var width = Mathf.RoundToInt((_heightPixels * Screen.width) / (float)Screen.height);
#else
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            var width = Mathf.RoundToInt((_heightPixels * Screen.currentResolution.width) / (float)Screen.currentResolution.height);
#endif

            _renderTexture = new RenderTexture(width, _heightPixels, 32)
            {
                filterMode = FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp
            };
            _renderTexture.Create();

            _mainCamera.targetTexture = _renderTexture;

            _rawImage.texture = _renderTexture;
            _rawImage.gameObject.SetActive(true);
        }

        private void OnDestroy()
        {
            _renderTexture.Release();
            Destroy(_renderTexture);
        }
    }
}