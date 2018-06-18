using UnityEngine;

namespace Mode7 {

    [RequireComponent(typeof(SpriteRenderer))]
    public class Mode7Objeto : MonoBehaviour {
        
        void Start() {
            //transform.position = Quaternion.Euler(Mode7Constantes.angulo, 0, 0) * transform.position;
            GetComponent<SpriteRenderer>().sortingOrder = (int)(-transform.position.z * 1000f);
        }
    }

}