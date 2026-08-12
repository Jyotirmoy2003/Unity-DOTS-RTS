using UnityEngine;

public class MouseWorldPosition : MonoSingleton<MouseWorldPosition>
{
    public Vector3 GetPosition()
    {
        Ray mouseCameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // if(Physics.Raycast(mouseCameraRay,out RaycastHit raycastHit))
        // {
        //     return raycastHit.point;
        // }

        Plane plane = new Plane(Vector3.up,Vector3.zero);

        if(plane.Raycast(mouseCameraRay,out float distance))
        {
            return mouseCameraRay.GetPoint(distance);
        }
        else
        {
            return Vector3.zero;
        }
    }

    
}
