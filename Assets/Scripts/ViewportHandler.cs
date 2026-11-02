// Attach this script on your main orthographic camera:

/* The MIT License (MIT)

Copyright (c) 2014, Marcel Căşvan

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class ViewportHandler : MonoBehaviour
{
    public Color wireColor = Color.white;
    public float UnitsSize = 1; // size of your scene in unity units
    public Constraint constraint = Constraint.Portrait;
    public static ViewportHandler Instance;
    public new Camera camera;

    public float Width { get; private set; }
    public float Height { get; private set; }

    // helper points:
    public Vector3 BottomLeft { get; private set; }
    public Vector3 BottomCenter { get; private set; }
    public Vector3 BottomRight { get; private set; }
    public Vector3 MiddleLeft { get; private set; }
    public Vector3 MiddleCenter { get; private set; }
    public Vector3 MiddleRight { get; private set; }
    public Vector3 TopLeft { get; private set; }
    public Vector3 TopCenter { get; private set; }
    public Vector3 TopRight { get; private set; }

    private void Awake()
    {
        camera = GetComponent<Camera>();
        Instance = this;
        ComputeResolution();
    }

    private void ComputeResolution()
    {
        if (constraint == Constraint.Landscape)
            camera.orthographicSize = 1f / camera.aspect * UnitsSize / 2f;
        else
            camera.orthographicSize = UnitsSize / 2f;

        Height = 2f * camera.orthographicSize;
        Width = Height * camera.aspect;

        var cameraX = camera.transform.position.x;
        var cameraY = camera.transform.position.y;
        var leftX = cameraX - Width / 2;
        var rightX = cameraX + Width / 2;
        var topY = cameraY + Height / 2;
        var bottomY = cameraY - Height / 2;

        BottomLeft = new Vector3(leftX, bottomY, 0);
        BottomCenter = new Vector3(cameraX, bottomY, 0);
        BottomRight = new Vector3(rightX, bottomY, 0);
        MiddleLeft = new Vector3(leftX, cameraY, 0);
        MiddleCenter = new Vector3(cameraX, cameraY, 0);
        MiddleRight = new Vector3(rightX, cameraY, 0);
        TopLeft = new Vector3(leftX, topY, 0);
        TopCenter = new Vector3(cameraX, topY, 0);
        TopRight = new Vector3(rightX, topY, 0);
    }

#if UNITY_EDITOR
    // Only recompute every frame in the editor, so builds don't pay for an empty Update
    private void Update()
    {
        ComputeResolution();
    }
#endif

    private void OnDrawGizmos()
    {
        if (camera == null)
            return;

        Gizmos.color = wireColor;

        var temp = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        if (camera.orthographic)
        {
            var spread = camera.farClipPlane - camera.nearClipPlane;
            var center = (camera.farClipPlane + camera.nearClipPlane) * 0.5f;
            Gizmos.DrawWireCube(new Vector3(0, 0, center), new Vector3(camera.orthographicSize * 2 * camera.aspect, camera.orthographicSize * 2, spread));
        }
        else
        {
            Gizmos.DrawFrustum(Vector3.zero, camera.fieldOfView, camera.farClipPlane, camera.nearClipPlane, camera.aspect);
        }
        Gizmos.matrix = temp;
    }

    public enum Constraint { Landscape, Portrait }
}
