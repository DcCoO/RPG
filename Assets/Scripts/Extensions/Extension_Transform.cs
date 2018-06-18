using UnityEngine;

public static class Extension_Transform {

    public static void x(this Transform transform, float value) {
        transform.position = new Vector3(value, transform.position.y, transform.position.z);
    }
    public static float x(this Transform transform) {
        return transform.position.x;
    }
    public static void y(this Transform transform, float value) {
        transform.position = new Vector3(transform.position.x, value, transform.position.z);
    }
    public static float y(this Transform transform) {
        return transform.position.y;
    }
    public static void z(this Transform transform, float value) {
        transform.position = new Vector3(transform.position.x, transform.position.y, value);
    }
    public static float z(this Transform transform) {
        return transform.position.z;
    }
}
