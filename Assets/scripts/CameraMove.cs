using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform cameraParent;
    [Header("Camera settings")]
    public float ySensitivity, xSensitivity, scrollSensitivity, truckSensitivity;
    public float translateSensitivity;
    public bool invertTruck;
    public bool useNewCameraMovement = true;

    //other 
    float xRotation, yRotation;
    bool middleMouseToggled = false;





    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }



    private void Update()
    {
        if (useNewCameraMovement)
        {
            newCameraMovement();
        }
        else
        {
            oldCameraMovement();
        }
    }

    private void oldCameraMovement()
    {
        if (!middleMouseToggled)
        {
            updateRotation();
        }
        closeInOrOut();
        TranslateCameraX();
    }

    void newCameraMovement()
    {
        toggleMiddleMouse();
        if (middleMouseToggled)
        {
            updateRotation();
        }
        TranslateCamera();
        AdjustHeight();
    }


    //new cameraMovement machanics
    void TranslateCamera()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
        {

            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");

            Vector3 originalVector = new Vector3(horizontal, 0, vertical);

            Vector3 moveVector = rotateVector(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y,
                transform.localEulerAngles.z,
                originalVector
            );

            moveVector = moveVector * Time.deltaTime * translateSensitivity;

            transform.position += new Vector3(moveVector.x, 0, moveVector.z);
        }

    }

    void AdjustHeight()
    {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * scrollSensitivity;
        transform.position += new Vector3(0, mouseWheel, 0);
    }

    //rotation is handled the same way with the only diff that it only works while pressing middle mouse button

    //old cameraMovement

    //based on mouse movement input camera is rotated accordingly
    void updateRotation()
    {
        //mouse input
        float cursorX = Input.GetAxis("Mouse X") * Time.deltaTime * xSensitivity;
        float cursorY = Input.GetAxis("Mouse Y") * Time.deltaTime * ySensitivity;

        yRotation += cursorX;
        xRotation -= cursorY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);


        //camera rotation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
    }

    //based on mousewheel scroll camera moves closer or further away from the camera direction (local z axis)
    void closeInOrOut()
    {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * scrollSensitivity;
        if (Mathf.Abs(mouseWheel) > 0)
        {

            Vector3 moveVector = rotateVector(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y,
                transform.localEulerAngles.z,
                Vector3.forward
            );


            transform.position += moveVector * mouseWheel;
        }
    }


    //based on mouse middle click and drag the camera moves in x, z axis
    void TranslateCameraX()
    {
        toggleMiddleMouse();

        //perform truck or translation within its local 'x' axis
        if (middleMouseToggled)
        {
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * truckSensitivity;
            Vector3 localDirection = transform.TransformDirection(Vector3.right);
            //Debug.Log($"local x direction: {localDirection}");
            if (invertTruck)
                mouseX = -mouseX;

            transform.position += localDirection * mouseX;
        }

    }

    private void toggleMiddleMouse()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            //Debug.Log("middle mouse button down");
            middleMouseToggled = true;
        }

        if (Input.GetKeyUp(KeyCode.Mouse2))
        {
            //Debug.Log("Middle mouse button up");
            middleMouseToggled = false;
        }
    }

    Vector3 rotateVector(float xAngle, float yAngle, float zAngle, Vector3 pVector)
    {
        //Debug.Log($"angles: {xAngle}, {yAngle}, {zAngle}");

        Vector3 returnVector;

        float xAngleRad = Mathf.Deg2Rad * xAngle;
        float yAngleRad = Mathf.Deg2Rad * yAngle;
        float zAngleRad = Mathf.Deg2Rad * zAngle;

        //x rotation matrix
        Matrix4x4 xRotationMatrix = Matrix4x4.identity;
        xRotationMatrix.m11 = Mathf.Cos(xAngleRad);
        xRotationMatrix.m12 = -Mathf.Sin(xAngleRad);
        xRotationMatrix.m21 = Mathf.Sin(xAngleRad);
        xRotationMatrix.m22 = Mathf.Cos(xAngleRad);

        //Debug.Log("xRotMat");
        //Debug.Log(xRotationMatrix);

        //y rotation matrix
        Matrix4x4 yRotationMatrix = Matrix4x4.identity;
        yRotationMatrix.m00 = Mathf.Cos(yAngleRad);
        yRotationMatrix.m02 = Mathf.Sin(yAngleRad);
        yRotationMatrix.m20 = -Mathf.Sin(yAngleRad);
        yRotationMatrix.m22 = Mathf.Cos(yAngleRad);

        //Debug.Log("yRotMat");
        //Debug.Log(yRotationMatrix);

        //z rotation matrix
        Matrix4x4 zRotationMatrix = Matrix4x4.identity;
        zRotationMatrix.m00 = Mathf.Cos(zAngleRad);
        zRotationMatrix.m01 = -Mathf.Sin(zAngleRad);
        zRotationMatrix.m10 = Mathf.Sin(zAngleRad);
        zRotationMatrix.m11 = Mathf.Cos(zAngleRad);

        //Debug.Log("zRotMat");
        //Debug.Log(zRotationMatrix);

        //final rotation matrix
        //order matter in matrix multiplication. In order to ensure correct rotation finalMatrix 
        //must follow Unity's euler angle representation that is z,y,x in that order
        Matrix4x4 finalMatrix = zRotationMatrix * yRotationMatrix * xRotationMatrix;

        //Debug.Log("finalMat");
        //Debug.Log(finalMatrix);

        returnVector = finalMatrix.MultiplyPoint3x4(pVector);

        //Debug.Log($"returnVector: {returnVector}");
        return returnVector;
    }

}
