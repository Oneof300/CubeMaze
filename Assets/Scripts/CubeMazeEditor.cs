using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Cube Maze Tool", typeof(CubeMaze))]
public class CubeMazeEditorTool : EditorTool
{
    CubeMaze cubeMaze;
    bool mouseIsPressed;
    Vector3 wallStartPosition;

    void OnEnable()
    {
        cubeMaze = target as CubeMaze;
    }
    
    public override void OnActivated()
    {
        SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Entering Cube Maze Tool"), .1f);
        cubeMaze.buildPreview.gameObject.SetActive(true);
    }

    public override void OnWillBeDeactivated()
    {
        SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Exiting Cube Maze Tool"), .1f);
        cubeMaze.buildPreview.gameObject.SetActive(false);
    }

    public override void OnToolGUI(EditorWindow window)
    {
        if (!(window is SceneView sceneView))
            return;

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        DrawButtons();
        DrawGrid();
        if (!mouseIsPressed)
            UpdateBuildPreview();
        else if (cubeMaze.selectedSegment == CubeMazeSegment.Wall)
            UpdateWallPreview();
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            mouseIsPressed = true;
        else if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            mouseIsPressed = false;
            BuildSegment();
        }
        else if (Event.current.type == EventType.MouseUp && Event.current.button == 1)
            DestroySegment();
    }

    private void DrawButtons()
    {
        Handles.BeginGUI();
        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Layer " + cubeMaze.layer);
                    GUI.skin.horizontalSlider.fixedWidth = 150f;
                    cubeMaze.layer = (ushort)GUILayout.HorizontalSlider(cubeMaze.layer, 0, cubeMaze.size);
                }
                cubeMaze.selectedSegment = (CubeMazeSegment)EditorGUILayout.EnumPopup(cubeMaze.selectedSegment);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Rotate:");
                    using (new GUILayout.VerticalScope())
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("-X"))
                                cubeMaze.buildPreview.Rotate(-90f, 0f, 0f, Space.World);
                            if (GUILayout.Button("+X"))
                                cubeMaze.buildPreview.Rotate(90f, 0f, 0f, Space.World);
                        }
                        using (new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("-Y"))
                                cubeMaze.buildPreview.Rotate(0f, -90f, 0f, Space.World);
                            if (GUILayout.Button("+Y"))
                                cubeMaze.buildPreview.Rotate(0f, 90f, 0f, Space.World);
                        }
                        using (new GUILayout.HorizontalScope())
                        {
                            if (GUILayout.Button("-Z"))
                                cubeMaze.buildPreview.Rotate(0f, 0f, -90f, Space.World);
                            if (GUILayout.Button("+Z"))
                                cubeMaze.buildPreview.Rotate(0f, 0f, 90f, Space.World);
                        }
                    }
                }
            }
            GUILayout.FlexibleSpace();
        }
        Handles.EndGUI();
    }

    void DrawGrid()
    {
        var colorDefault = new Color(0.25f, 0.75f, 1f, 0.04f);
        var colorHighlight = new Color(0.25f, 0.75f, 1f, 0.12f);
        var start = cubeMaze.transform.position - Vector3.one * cubeMaze.size / 2f;

        for (ushort i = 0; i <= cubeMaze.size; ++i)
        {    
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

    void UpdateBuildPreview()
    {
        var plane = new Plane(Vector3.up, cubeMaze.size / 2f - cubeMaze.layer);
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            var hitPoint = ray.GetPoint(enter);
            Func<float, float, float> SnapToGrid = (value, size) => {
                var sizeHalve = size / 2f - 0.5f;
                var offset = size % 2 == 0 ? 0.5f : 0f;
                return Mathf.Clamp(Mathf.Round(value + offset) - offset, -sizeHalve, sizeHalve);
            };
            hitPoint.x = SnapToGrid(hitPoint.x, cubeMaze.size);
            hitPoint.y = hitPoint.y + 0.5f;
            hitPoint.z = SnapToGrid(hitPoint.z, cubeMaze.size);
            if (cubeMaze.selectedSegment == CubeMazeSegment.Wall)
            {
                hitPoint += cubeMaze.buildPreview.forward / -2f;
                wallStartPosition = hitPoint;
            }
            cubeMaze.buildPreview.position = hitPoint;
        }
        for (int i = 0; i < cubeMaze.buildPreview.childCount; i++)
        {
            var buildPreview = cubeMaze.buildPreview.GetChild(i);
            var isSelectedSegment = i == (int)cubeMaze.selectedSegment;
            buildPreview.gameObject.SetActive(isSelectedSegment);
            // if (isSelectedSegment) buildPreview.GetComponent<MeshRenderer>().material =
            //     IsBlocked(buildPreview.position, cubeMaze) ? cubeMaze.buildPreviewBlocked : cubeMaze.buildPreviewOpen;
        }
        if (cubeMaze.buildPreview.localScale.sqrMagnitude > 1)
        {
            cubeMaze.buildPreview.localScale = Vector3.one;
        }
    }

    void UpdateWallPreview()
    {
        var forward = cubeMaze.buildPreview.forward;
        var plane = new Plane(forward, wallStartPosition);
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (plane.Raycast(ray, out float enter))
        {
            var hitPoint = ray.GetPoint(enter);
            Func<float, float, float> SnapToGrid = (value, size) => {
                var sizeHalve = size / 2f - 0.5f;
                var offset = size % 2 == 0 ? 0.5f : 0f;
                return Mathf.Clamp(Mathf.Round(value + offset) - offset, -sizeHalve, sizeHalve);
            };
            hitPoint.x = Math.Abs(forward.x) > 0.9 ? hitPoint.x : SnapToGrid(hitPoint.x, cubeMaze.size);
            hitPoint.y = Math.Abs(forward.y) > 0.9 ? hitPoint.y : SnapToGrid(hitPoint.y, cubeMaze.size);
            hitPoint.z = Math.Abs(forward.z) > 0.9 ? hitPoint.z : SnapToGrid(hitPoint.z, cubeMaze.size);
            var newPosition = (wallStartPosition + hitPoint) / 2f;
            var newScale = hitPoint - wallStartPosition;
            if (Math.Abs(forward.x) > 0.9)
            {
                newScale.x = Math.Abs(newScale.z) + 1f;
                newScale.y = Math.Abs(newScale.y) + 1f;
                newScale = Quaternion.Euler(-cubeMaze.buildPreview.rotation.x, 0f, 0f) * newScale;
            }
            else if (Math.Abs(forward.y) > 0.9)
            {
                newScale.x = Math.Abs(newScale.z) + 1f;
                newScale.y = Math.Abs(newScale.x) + 1f;
            }
            else
            {
                newScale.x = Math.Abs(newScale.x) + 1f;
                newScale.y = Math.Abs(newScale.y) + 1f;
                newScale = Quaternion.Euler(0f, 0f, -cubeMaze.buildPreview.rotation.z) * newScale;
            }
            newScale.z = 1f;
            cubeMaze.buildPreview.position = newPosition;
            cubeMaze.buildPreview.localScale = newScale;
        }
    }

    void BuildSegment()
    {
        if (!cubeMaze.GetComponentsInChildren<Transform>().Any(
                segment => segment != cubeMaze.buildPreview && segment.transform.parent != cubeMaze.buildPreview && segment.position == cubeMaze.buildPreview.position
            ))
            {
                var newSegment = Instantiate(cubeMaze.segments[(int)cubeMaze.selectedSegment], cubeMaze.buildPreview.position, cubeMaze.buildPreview.rotation, cubeMaze.transform);
                if (cubeMaze.selectedSegment == CubeMazeSegment.Wall)
                    newSegment.transform.localScale = new Vector3(cubeMaze.buildPreview.localScale.x, cubeMaze.buildPreview.localScale.y, 0.1f);
                Undo.RegisterCreatedObjectUndo(newSegment, "Create " + newSegment.name);
            }
            else
                Debug.Log("Position was blocked.");
    }

    void DestroySegment()
    {
        Transform hovered;
        if (hovered = cubeMaze.GetComponentsInChildren<Transform>().FirstOrDefault(
            segment => segment != cubeMaze.buildPreview && segment.transform.parent != cubeMaze.buildPreview && segment.position == cubeMaze.buildPreview.position
        ))
            Undo.DestroyObjectImmediate(hovered.gameObject);
    }

    bool IsBlocked(Vector3 position)
    {
        if (cubeMaze.selectedSegment == CubeMazeSegment.Wall)
            return cubeMaze.nodes[(int)position.x, (int)position.y, (int)position.z].edges[position.x % 1 == 0.5f ? 3 : position.y % 1 == 0.5f ? 4 : 5].state == EdgeState.blocked;
        else
            return cubeMaze.nodes[(int)position.x, (int)position.y, (int)position.z].state == NodeState.blocked;
    }
}