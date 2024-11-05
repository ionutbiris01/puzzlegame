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
        isDrawing = true;
        playerSequence.Clear();
        points.Clear();
        previousSegments.Clear();
        lineRenderer.positionCount = 0;

        AddPoint(node, hit);
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

        //Debug.Log($"new segment start: {newSegmentStart}, New segment end: {newSegmentEnd}");

        LineSegment newSegment = new LineSegment(newSegmentStart, newSegmentEnd);


        Vector3 intersectionPoint;
        if (LineSegmentIntersection.DoesIntersectWithAny(previousSegments.ToArray(), newSegment, out intersectionPoint))
        {
            Debug.Log($"new segment intersects with an existing segment at: {intersectionPoint}");
            
            return; 
        }

        previousSegments.Add(newSegment);
        //Debug.Log($"new segment: {newSegmentStart} to {newSegmentEnd}");

        playerSequence.Add(node);
        points.Add(offsetPos);

        lineRenderer.positionCount = points.Count + 1;
        lineRenderer.SetPositions(points.ToArray());
    }

    void CheckPuzzleCompletion()
    {
        if (playerSequence.Count > 0 && playerSequence[0] == startingNode && playerSequence[playerSequence.Count - 1] == endingNode)
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
            Debug.Log("Connection invalid. Please connect from start to end.");
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