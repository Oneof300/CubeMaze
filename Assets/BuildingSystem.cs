using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildingSystem : MonoBehaviour
{
    public ushort size;
    public ushort layer;
    public Axis axis;
    public Transform highlightedQuad;
}

[CustomEditor(typeof(BuildingSystem))]
public class BuildingSystemEditor : Editor
{
    void OnSceneGUI()
    {
        var system = target as BuildingSystem;
        var colorDefault = new Color(0.25f, 0.75f, 1f, 0.1f);
        var colorHighlight = new Color(0.25f, 0.75f, 1f, 0.4f);
        var start = system.transform.position - Vector3.one * system.size / 2f;

        for (ushort i = 0; i <= system.size; ++i) {
            
            var layer = system.axis == Axis.X ? system.layer : 0;
            var thickness = system.axis == Axis.X ? 1.5f : 1f;
            Handles.color = system.axis == Axis.X ? colorHighlight : colorDefault;
            Handles.DrawLine(start + new Vector3(layer, 0, i), start + new Vector3(layer, system.size, i), thickness);
            Handles.DrawLine(start + new Vector3(layer, i, 0), start + new Vector3(layer, i, system.size), thickness);
            
            layer = system.axis == Axis.Y ? system.layer : 0;
            thickness = system.axis == Axis.Y ? 1.5f : 1f;
            Handles.color = system.axis == Axis.Y ? colorHighlight : colorDefault;
            Handles.DrawLine(start + new Vector3(i, layer, 0), start + new Vector3(i, layer, system.size), thickness);
            Handles.DrawLine(start + new Vector3(0, layer, i), start + new Vector3(system.size, layer, i), thickness);

            layer = system.axis == Axis.Z ? system.layer : 0;
            thickness = system.axis == Axis.Z ? 1.5f : 1f;
            Handles.color = system.axis == Axis.Z ? colorHighlight : colorDefault;
            Handles.DrawLine(start + new Vector3(0, i, layer), start + new Vector3(system.size, i, layer), thickness);
            Handles.DrawLine(start + new Vector3(i, 0, layer), start + new Vector3(i, system.size, layer), thickness);
        }

        var plane = new Plane(system.axis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            _ => Vector3.forward
        }, system.size / 2f- system.layer);
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            var hitPoint = ray.GetPoint(enter);
            var sizeHalve = system.size / 2f - 0.5f;
            var offset = system.size % 2 == 0 ? 0.5f : 0f;
            Func<float, float> SnapToGrid = value => Mathf.Clamp(Mathf.Round(value + offset) - offset, -sizeHalve, sizeHalve);
            if (system.axis == Axis.X)
            {
                hitPoint.y = SnapToGrid(hitPoint.y);
                hitPoint.z = SnapToGrid(hitPoint.z);
            }
            else if (system.axis == Axis.Y)
            {
                hitPoint.x = SnapToGrid(hitPoint.x);
                hitPoint.z = SnapToGrid(hitPoint.z);
            }
            else if (system.axis == Axis.Z)
            {
                hitPoint.x = SnapToGrid(hitPoint.x);
                hitPoint.y = SnapToGrid(hitPoint.y);
            }
            system.highlightedQuad.position = hitPoint;
        }
    }
}

[Serializable]
public enum Axis
{
    X, Y, Z
}
