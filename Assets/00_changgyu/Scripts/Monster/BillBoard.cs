using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Transform cam;

    private void Start()
    {
        cam= UnityEngine.Camera.main.transform; 
    }

    private void LateUpdate()
    {
        transform.forward = cam.forward;
    }


}
