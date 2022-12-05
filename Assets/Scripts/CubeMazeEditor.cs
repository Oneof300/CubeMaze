using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CubeMaze))]
public class CubeMazeEditor : Editor
{
    override public void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var cubeMaze = target as CubeMaze;

        if (EditorGUILayout.LinkButton("Rotate X"))
            cubeMaze.buildPreview.Rotate(90f, 0f, 0f);

        if (EditorGUILayout.LinkButton("Rotate Y"))
            cubeMaze.buildPreview.Rotate(0f, 90f, 0f);

        if (EditorGUILayout.LinkButton("Rotate Z"))
            cubeMaze.buildPreview.Rotate(0f, 0f, 90f);
    }

    void OnSceneGUI()
    {
        var cubeMaze = target as CubeMaze;

        DrawGrid(cubeMaze);
        UpdateHoveredQuad(cubeMaze);

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            if (!cubeMaze.GetComponentsInChildren<Transform>().Any(
                segment => segment != cubeMaze.buildPreview && segment.transform.parent != cubeMaze.buildPreview && segment.position == cubeMaze.buildPreview.position
            ))
                Instantiate(cubeMaze.segments[(int)cubeMaze.selectedSegment], cubeMaze.buildPreview.position, cubeMaze.buildPreview.rotation, cubeMaze.transform);
            else
                Debug.Log("Position was blocked.");
        }
        else if (Event.current.type == EventType.MouseUp && Event.current.button == 1)
        {
            Transform hovered;
            if (hovered = cubeMaze.GetComponentsInChildren<Transform>().FirstOrDefault(
                segment => segment != cubeMaze.buildPreview && segment.transform.parent != cubeMaze.buildPreview && segment.position == cubeMaze.buildPreview.position
            ))
                DestroyImmediate(hovered.gameObject);
        }
    }

    void DrawGrid(CubeMaze cubeMaze)
    {
        var colorDefault = new Color(0.25f, 0.75f, 1f, 0.1f);
        var colorHighlight = new Color(0.25f, 0.75f, 1f, 0.4f);
        var start = cubeMaze.transform.position - Vector3.one * cubeMaze.size / 2f;

        for (ushort i = 0; i <= cubeMaze.size; ++i) {
            
            var layer = 0;
            var thickness = 1f;
            Handles.color = colorDefault;
            Handles.DrawLine(start + new Vector3(layer, 0, i), start + new Vector3(layer, cubeMaze.size, i), thickness);
            Handles.DrawLine(start + new Vector3(layer, i, 0), start + new Vector3(layer, i, cubeMaze.size), thickness);
            
            layer = cubeMaze.layer;
            thickness = 1.5f;
            Handles.color = colorHighlight;
            Handles.DrawLine(start + new Vector3(i, layer, 0), start + new Vector3(i, layer, cubeMaze.size), thickness);
            Handles.DrawLine(start + new Vector3(0, layer, i), start + new Vector3(cubeMaze.size, layer, i), thickness);

            layer = 0;
            thickness = 1f;
            Handles.color = colorDefault;
            Handles.DrawLine(start + new Vector3(0, i, layer), start + new Vector3(cubeMaze.size, i, layer), thickness);
            Handles.DrawLine(start + new Vector3(i, 0, layer), start + new Vector3(i, cubeMaze.size, layer), thickness);
        }
    }

    void UpdateHoveredQuad(CubeMaze cubeMaze)
    {
        var plane = new Plane(Vector3.up, cubeMaze.size / 2f - cubeMaze.layer);
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            var hitPoint = ray.GetPoint(enter);
            var sizeHalve = cubeMaze.size / 2f - 0.5f;
            var offset = cubeMaze.size % 2 == 0 ? 0.5f : 0f;
            Func<float, float> SnapToGrid = value => Mathf.Clamp(Mathf.Round(value + offset) - offset, -sizeHalve, sizeHalve);
            hitPoint.x = SnapToGrid(hitPoint.x);
            hitPoint.z = SnapToGrid(hitPoint.z);
            cubeMaze.buildPreview.position = hitPoint;
        }
        for (int i = 0; i < cubeMaze.buildPreview.childCount; i++)
        {
            cubeMaze.buildPreview.GetChild(i).gameObject.SetActive(i == (int)cubeMaze.selectedSegment);
        }
    }
}