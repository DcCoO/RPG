using UnityEngine;

namespace Mode7 {

    [RequireComponent(typeof(SpriteRenderer))]
    public class Mode7Player : MonoBehaviour {

        private SpriteRenderer sr;
        private Transform t;

        private void Start() {
            sr = GetComponent<SpriteRenderer>();
            t = transform;
        }

        void Update() {
            transform.z(transform.y() * Mathf.Sin(Mode7Constantes.angulo * Mathf.Deg2Rad));
            sr.sortingOrder = (int)(-t.z() * 1000f);
        }
    }

}