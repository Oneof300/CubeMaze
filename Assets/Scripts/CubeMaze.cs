using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CubeMaze : MonoBehaviour
{
    public ushort size;
    public GameObject[] segments;
    public Transform buildPreview;
    public ushort layer;
    public CubeMazeSegment selectedSegment;
}

public enum CubeMazeSegment
{
    Straight = 0, End = 1, CurveSmall = 2, CurveBig = 3
}
