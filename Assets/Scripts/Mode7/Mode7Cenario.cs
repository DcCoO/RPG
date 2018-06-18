using UnityEngine;

namespace Mode7 {

    [RequireComponent(typeof(SpriteRenderer))]
    public class Mode7Cenario : MonoBehaviour {
        
        void Start() {
            transform.eulerAngles = new Vector3(Mode7Constantes.angulo, 0, 0);
        }
    }

}