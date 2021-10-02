using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlacement : MonoBehaviour
{
    public static BlockPlacement instance;
    public GameObject prefab;
    public Transform platform;
    // Start is called before the first frame update
    private Coroutine droppingRoutine;

    private void Awake()
    {
        if (instance != null)
        {
            GameObject.Destroy(instance.gameObject);
        }
        instance = this;
    }

    public void DropObject(Vector3 targetPosition)
    {
         if (droppingRoutine == null)
        {
            droppingRoutine=StartCoroutine(DropObject(prefab, targetPosition, .5f));
        }
    }

    public IEnumerator DropObject(GameObject prefab, Vector3 targetPosition, float dropTime)
    {
        GameState.instance.currentState = GameState.State.PLACING;
        GameObject tmp = Instantiate(prefab);
        tmp.transform.parent = platform;
        tmp.transform.localRotation = Quaternion.Euler(Vector3.zero);

        Vector3 initPosition = targetPosition + (platform.up * 10);
        Vector3 dropPosition = targetPosition + (platform.up * 1f);
        float droppingTime = 0;
        while (droppingTime < dropTime)
        {
            droppingTime += Time.deltaTime;
            tmp.transform.position = Vector3.Lerp(initPosition, dropPosition, droppingTime / dropTime);
            yield return null;
        }
        tmp.transform.position = dropPosition;


        yield return new WaitForSeconds(.5f);
        GameState.instance.currentState = GameState.State.PHYSICS;
        droppingRoutine = null;
    }




}
