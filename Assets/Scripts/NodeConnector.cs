using System.Collections.Generic;
using UnityEngine;

public class NodeConnector : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform startingNode;
    public Transform endingNode;
    public List<Transform> allNodes;
    private List<Transform> playerSequence = new List<Transform>();
    private List<Vector3> points = new List<Vector3>();
    private List<LineSegment> previousSegments = new List<LineSegment>();
    private bool isDrawing = false;

    public float offsetDistance = 0.5f;

    private const float DistanceThreshold = 0.001f;

    void Start()
    {
        lineRenderer.positionCount = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Node"))
                {
                    StartDrawing(hit.transform, hit);
                }
            }
        }

        if (isDrawing && Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Node") && !playerSequence.Contains(hit.transform))
                {
                    AddPoint(hit.transform, hit);
                }
            }
            UpdateLineToMousePosition();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            CheckPuzzleCompletion();
            ResetPuzzle();
        }
    }

    private Vector3 GetOffsetPosition(Vector3 position, Vector3 normal)
    {
        return position + normal * offsetDistance;
    }

    void StartDrawing(Transform node, RaycastHit hit)
    {
        if (node == startingNode)
        {
            isDrawing = true;
            playerSequence.Clear();
            points.Clear();
            previousSegments.Clear();
            lineRenderer.positionCount = 0;

            AddPoint(node, hit);
            Debug.Log("Drawing started from the starting node.");
        }
        else
        {
            Debug.Log("Please start drawing from the starting node.");
        }
    }

    void UpdateLineToMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 mousePos = GetOffsetPosition(hit.point, hit.normal);

            if (lineRenderer.positionCount > 0)
            {
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, mousePos);
            }
        }
    }

    void AddPoint(Transform node, RaycastHit hit)
    {
        Vector3 offsetPos = GetOffsetPosition(node.position, hit.normal);
        Vector3 newSegmentStart = points.Count > 0 ? points[points.Count - 1] : offsetPos;
        Vector3 newSegmentEnd = offsetPos;

        LineSegment newSegment = new LineSegment(newSegmentStart, newSegmentEnd);

        if (LineSegmentIntersection.DoesIntersectWithAny(previousSegments.ToArray(), newSegment, out Vector3 intersectionPoint))
        {
            Debug.Log($"New segment intersects with an existing segment at: {intersectionPoint}");
            return;
        }

        previousSegments.Add(newSegment);

        playerSequence.Add(node);
        points.Add(offsetPos);

        lineRenderer.positionCount = points.Count + 1;
        lineRenderer.SetPositions(points.ToArray());
    }

    void CheckPuzzleCompletion()
    {
        if (playerSequence.Count < 2) 
        {
            return;
        }

        if (playerSequence[0] == startingNode && playerSequence[playerSequence.Count - 1] == endingNode)
        {
            if (IsFullSequenceConnected())
            {
                Debug.Log("All nodes are connected correctly!");
            }
            else
            {
                Debug.Log("Not all nodes are connected. Please try again.");
            }
        }
        else
        {
            Debug.Log("Please connect from start to end.");
        }
    }

    bool IsFullSequenceConnected()
    {
        int startIndex = allNodes.IndexOf(startingNode);
        int endIndex = allNodes.IndexOf(endingNode);

        if (startIndex < endIndex)
        {
            List<Transform> expectedSequence = allNodes.GetRange(startIndex, endIndex - startIndex + 1);

            foreach (Transform node in expectedSequence)
            {
                if (!playerSequence.Contains(node))
                {
                    return false;
                }
            }
            return true;
        }

        return false;
    }

    void ResetPuzzle()
    {
        playerSequence.Clear();
        points.Clear();
        previousSegments.Clear();
        lineRenderer.positionCount = 0;
    }
}
