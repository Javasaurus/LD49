using UnityEngine;

[ExecuteInEditMode]
public class PlatformController : MonoBehaviour
{
    public float force = 1f;
    public float xAngle = 0;
    public float zAngle = 0;

    [HideInInspector]
    public int cellSize = 1;
    [Range(1, 5)]
    public int fieldStage = 2;

    [HideInInspector]
    public int fieldSize;

    public Transform[,] ForceGrid;
    public float[,] ForceMap;
    public Vector3 center;

    public Vector3 selected;

    private Rigidbody rb;

    private void Awake()
    {
        fieldSize = fieldStage * 3;
        if (fieldSize % 2 == 0)
        {
            fieldSize++;
        }
        SetField();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Clicky");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 clickHit = toGrid(hit.point);
                selected = getGridPosition(clickHit);
                Debug.Log(selected);
                SetForce((int)(clickHit.x), (int)(clickHit.z), force);
                BlockPlacement.instance.DropObject(selected);
            }
        }
    }

    private void FixedUpdate()
    {
        if (GameState.instance.currentState == GameState.State.PHYSICS)
        {

            if (ForceMap == null || ForceMap.Length <= 0) return;
            for (int x = 0; x < fieldSize; x++)
            {
                for (int z = 0; z < fieldSize; z++)
                {
                    float force = ForceMap[x, z];
                    xAngle -= (x - fieldSize / 2) * force;
                    zAngle += (z - fieldSize / 2) * force;
                }
            }
            //if the total forces nullify eachother out, the angle should lerp to 0

            xAngle = Mathf.Lerp(xAngle, 0, Time.deltaTime);
            zAngle = Mathf.Lerp(zAngle, 0, Time.deltaTime);

        }
    }
    private void LateUpdate()
    {
        //clamp the rotation
        transform.parent.localRotation = Quaternion.Slerp(transform.parent.localRotation, Quaternion.Euler(zAngle, 0, xAngle), Time.deltaTime * 50);

    }
    private void OnDrawGizmos()
    {
        if (ForceGrid != null && ForceGrid.Length > 0)
        {
            for (int x = 0; x < fieldSize; x++)
            {
                for (int z = 0; z < fieldSize; z++)
                {
                    if (x < fieldSize && z < fieldSize)
                    {
                        Gizmos.color =
                            ForceGrid[x, z].position == center ? Color.black :
                            ForceGrid[x, z].position == selected ? Color.green :
                            Color.red;

                        Gizmos.DrawSphere(ForceGrid[x, z].position + (Vector3.up * .5f), .1f);
                    }
                }
            }
        }
    }
    private void SetField()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector3((fieldSize+1) * cellSize, 1, (fieldSize+1) * cellSize);

        if (ForceGrid != null && ForceGrid.Length > 0)
        {
            for (int x = 0; x < fieldSize; x++)
            {
                for (int z = 0; z < fieldSize; z++)
                {
                    GameObject.Destroy(ForceGrid[x, z].gameObject);
                }
            }
        }

        ForceGrid = new Transform[fieldSize, fieldSize];
        ForceMap = new float[fieldSize, fieldSize];
        for (int x = 0; x < fieldSize; x++)
        {
            for (int z = 0; z < fieldSize; z++)
            {
                ForceMap[x, z] = 0;

                ForceGrid[x, z] = new GameObject(x + "," + z).transform;
                ForceGrid[x, z].transform.parent = this.transform;
                ForceGrid[x, z].transform.position = transform.position + new Vector3(
                    x - (fieldSize / 2) - (cellSize / 4),
                    0,
                    z - (fieldSize / 2) - (cellSize / 4)
                    );

                if (x == fieldSize / 2 && z == fieldSize / 2)
                {
                    center = ForceGrid[x, z].position;
                }
            }
        }
        rb.isKinematic = false;
    }
    public void SetForce(int x, int z, float force)
    {
        if (ForceMap.Length <= 0) return;
        if (x >= 0 && x < ForceMap.Length && z >= 0 && z < ForceMap.Length)
        {
            ForceMap[x, z] += force;
        }
    }
    private Vector3 getGridPosition(Vector3 clickHit)
    {
        return ForceGrid[(int)(clickHit.x), (int)(clickHit.z)].position;
    }
    private Vector3 toGrid(Vector3 currentPos)
    {
        return new Vector3(Mathf.Round(currentPos.x + fieldSize / 2),
                             Mathf.Round(currentPos.y),
                             Mathf.Round(currentPos.z + fieldSize / 2));
    }
}
