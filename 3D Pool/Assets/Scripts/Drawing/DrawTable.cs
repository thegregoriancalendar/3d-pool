using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using System.Linq;

public class DrawTable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject linePrefab;

    List<Line> lines = new List<Line>();
    Line currentLine;

    public EventSystem eventSys;

    public bool addLineMode = false; // figure out getters and setters at some point pls

    private bool mouseIsOver;

    float distanceTolerance = 14f;

    public float width = 1f;

    public GameObject highlightCirclePrefab;
    GameObject highlightCircle;

    public GameObject poolTable;

    public GameObject dingle;

    public Camera fakeCamera;
    //Mesh mesh;

    // Start is called before the first frame update
    void Start()
    {
        Line.linePrefab = linePrefab;
        poolTable.GetComponent<MeshFilter>().mesh = new Mesh();

    }

    // Update is called once per frame

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseIsOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseIsOver = false;
        addLineMode = false;
    }

    void Update()
    {

        (Line, bool) hiPoint = cursorNearEndPoint();

        if (addLineMode && mouseIsOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (highlightCircle == null)
                {
                    lines.Add(new Line(Input.mousePosition, transform));

                    addMeshVertices();
                }
                else
                {
                    lines.Add(new Line(highlightCircle.transform.position, transform));
                }
            }

            if (Input.GetMouseButton(0))
            {

                if (highlightCircle == null)
                {
                    lines[lines.Count - 1].UpdateLine(Input.mousePosition, false);
                } 
                else
                {
                    lines[lines.Count - 1].UpdateLine(highlightCircle.transform.position, false);
                }



            }

            if (Input.GetMouseButtonUp(0))
            {
                if (highlightCircle == null)
                {
                    addMeshVertices();
                }

                int len = poolTable.GetComponent<MeshFilter>().mesh.vertices.Length;

                // change mesh orientation

                Vector3 direction;

                if (highlightCircle == null)
                {
                    direction = poolTable.GetComponent<MeshFilter>().mesh.vertices[len - 4] - poolTable.GetComponent<MeshFilter>().mesh.vertices[len - 8];
                }
                else
                {
                    direction = poolTable.GetComponent<MeshFilter>().mesh.vertices[0] - poolTable.GetComponent<MeshFilter>().mesh.vertices[len - 4];
                }

                Vector3 offset = Vector3.Normalize(Vector3.Cross(fakeCamera.transform.forward, direction)) * width;

                Vector3[] vertices = poolTable.GetComponent<MeshFilter>().mesh.vertices;

                if (highlightCircle == null)
                {
                    vertices[len - 8] += offset;
                    vertices[len - 7] += offset;

                    vertices[len - 6] -= offset;
                    vertices[len - 5] -= offset;
                } 
                else
                {
                    vertices[3] += offset;
                    vertices[2] += offset;

                    vertices[1] -= offset;
                    vertices[0] -= offset;
                }

                vertices[len - 4] += offset;
                vertices[len - 3] += offset;

                vertices[len - 2] -= offset;
                vertices[len - 1] -= offset;

                poolTable.GetComponent<MeshFilter>().mesh.vertices = vertices;

                poolTable.GetComponent<MeshFilter>().mesh.RecalculateNormals();



                if (len > 7)
                {
                    //int[] triangles = { len - 4, len - 2, len - 3, len - 1, len - 3, len - 2 };

                    int[] triangles = {1, 2, 6,
                                       6, 5, 1,
                                       1, 5, 4,
                                       5, 8, 4,
                                       3, 4, 8,
                                       8, 7, 3,
                                       2, 3, 7,
                                       7, 6, 2};

                    if (highlightCircle  == null)
                    {
                        for (int i = 0; i < triangles.Length; i++)
                        {
                            triangles[i] += len - 9;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < triangles.Length; i++)
                        {
                            if (triangles[i] < 5)
                            {
                                triangles[i] += len - 5;
                            }
                            else
                            {
                                triangles[i] -= 5;
                            }
                        }
                    }
                    

                    poolTable.GetComponent<MeshFilter>().mesh.triangles = addElementsToArray<int>(poolTable.GetComponent<MeshFilter>().mesh.triangles, triangles);
                }


                

                poolTable.GetComponent<MeshFilter>().mesh.RecalculateNormals();


                foreach (Vector3 vertex in poolTable.GetComponent<MeshFilter>().mesh.vertices)
                {
                    Debug.Log(vertex);
                    Instantiate(dingle, vertex, Quaternion.identity);
                }

                foreach (int triangle in poolTable.GetComponent<MeshFilter>().mesh.triangles)
                {
                    Debug.Log(triangle);
                }
                //addLineMode = false;
            }
        }

        // point to highlight

        if (hiPoint.Item1 != null)
        {
            if (highlightCircle == null)
            {
                if (hiPoint.Item2)
                {
                    highlightCircle = Instantiate(highlightCirclePrefab, hiPoint.Item1.start, Quaternion.identity, transform);
                }
                else
                {
                    highlightCircle = Instantiate(highlightCirclePrefab, hiPoint.Item1.end, Quaternion.identity, transform);
                }
            }
        } 
        else
        {
            if (highlightCircle != null)
            {
                Destroy(highlightCircle);
                highlightCircle = null;
            }
        }
    }

    public void addMeshVertices()
    {

        Vector3[] newPoints = { fakeCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3)), 
                                fakeCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 4)),
                                fakeCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 4)),
                                fakeCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 3)),
                                };

        poolTable.GetComponent<MeshFilter>().mesh.vertices = addElementsToArray<Vector3>(poolTable.GetComponent<MeshFilter>().mesh.vertices, newPoints);
    }

    public void setLineMode()
    {
        addLineMode = !addLineMode;
    }

    (Line, bool) cursorNearEndPoint()
    {
        if (!mouseIsOver)
        {
            return (null, false);
        }

        foreach (Line line in lines)
        {
            if (Input.GetMouseButton(0) && line == lines[lines.Count - 1])
            {
                continue;
            }

            if (Vector3.Distance(Input.mousePosition, line.start) < distanceTolerance)
            {
                return (line, true);
            }

            if (Vector3.Distance(Input.mousePosition, line.end) < distanceTolerance)
            {
                return (line, false);
            }
        }

        return (null, false);
    }

    T[] addElementsToArray<T>(T[] array, T[] newElements)
    {
        T[] newArray = array.Concat(newElements).ToArray();

        return newArray;
    }
}
