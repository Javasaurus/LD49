using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationTime = .75f;

    private float[] angles = new float[] { 45f, -45f, -135f, -225f };

    public int currentCameraPosition = 0;

    private Coroutine rotationRoutine;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Rotate(+1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Rotate(-1);
        }
    }

    public void Rotate(int direction)
    {

        if (rotationRoutine == null)
        {
            GameState.instance.currentState = GameState.State.CAMERA;

            currentCameraPosition += direction;

            if (currentCameraPosition < 0)
            {
                currentCameraPosition = angles.Length - 1;
            }
            else if (currentCameraPosition >= angles.Length)
            {
                currentCameraPosition = 0;
            }



            rotationRoutine = StartCoroutine(Rotate());
        }
    }

    private IEnumerator Rotate()
    {
        float rotatingTime = 0;
        if (rotationTime <= 0)
        {
            rotationTime = .1f;
        }
        Quaternion current = transform.rotation;
        Quaternion target = Quaternion.Euler(current.eulerAngles.x, angles[currentCameraPosition], current.eulerAngles.z);
        while (rotatingTime < rotationTime)
        {
            rotatingTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(current, target, rotatingTime / rotationTime);
            yield return null;
        }
        transform.rotation = target;
        if (currentCameraPosition == angles.Length)
        {
            currentCameraPosition = 0;
        }
        yield return new WaitForSeconds(.5f);
        GameState.instance.ResetState();
        rotationRoutine = null;
    }


}
