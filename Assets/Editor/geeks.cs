using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GraphEditorWindow : EditorWindow
{
    DialogueNode[] Nodes = new DialogueNode[]{
        new DialogueNode(new Rect(100,100,100,40), 1, 2),
        new DialogueNode(new Rect(150, 150, 100, 40)),
        new DialogueNode(new Rect(100, 200, 100, 40))
    };
    Vector2 scrollArea = new Vector2(0, 0);

    [MenuItem("Window/Graph Editor Window")]
    static void Init()
    {
        GetWindow(typeof(GraphEditorWindow));
    }

    private static Texture2D _bgCache;
    private static Texture2D _background
    {
        get
        {
            if (!_bgCache)
            {
                _bgCache = new Texture2D(100, 40);
                var color = new Color(1f, 0.7f, 0f);
                var trans = new Color(0f, 0f, 0f, 0f);
                for (var y = 0; y < 40; y++)
                    for (var x = 0; x < 100; x++)
                    {
                        if (y < 20)
                        {
                            if (x - 20 + y > 0 && x + 20 - y < 100)
                                _bgCache.SetPixel(x, y, color);
                            else
                                _bgCache.SetPixel(x, y, trans);
                        }
                        else
                        {
                            if (x > y - 20 && x < 120 - y)
                                _bgCache.SetPixel(x, y, color);
                            else _bgCache.SetPixel(x, y, trans);

                        }
                    }
                _bgCache.Apply();
            }
            return _bgCache;
        }
    }

    private static Texture2D _bgColorCache;
    private static Texture2D _bgColor
    {
        get
        {
            if (!_bgColorCache)
            {
                _bgColorCache = new Texture2D(1, 1);
                _bgColorCache.SetPixel(0, 0, new Color(0.3f, 0.3f, 0.3f, 1));
                _bgColorCache.Apply();
            }
            return _bgColorCache;
        }
    }

    private GUIStyle Style = new GUIStyle();

    private void OnGUI()
    {
        scrollArea = GUI.BeginScrollView(new Rect(Vector2.zero, new Vector2(Screen.width - 4, Screen.height - 21)),
            scrollArea, new Rect(ScrollAreaMin(), ScrollAreaMax()));

        List<Vector4> lines = new List<Vector4>();

        Handles.BeginGUI();
        foreach (var n in Nodes)
        {
            for (var i = 0; i < n.Connections.Length; i++)
            {
                var connection = Nodes[n.Connections[i]].Rect.center;
                Handles.DrawBezier(n.Rect.center, connection, n.Rect.center, connection, Color.red, null, 5f);
                lines.Add(new Vector4(n.Rect.center.x, n.Rect.center.y, connection.x, connection.y));
            }
        }
        Handles.EndGUI();

        Style.normal.background = _bgColor;
        EditorGUILayout.BeginHorizontal(Style);
        BeginWindows();
        Style.normal.background = _background;
        for (var i = 0; i < Nodes.Length; i++)
        {
            var rect = GUI.Window(i, Nodes[i].Rect, WindowFunction, string.Empty, Style);
            if (rect.x < 0)
                rect = new Rect(0, rect.y, rect.width, rect.height);
            if (rect.y < 0)
                rect = new Rect(rect.x, 0, rect.width, rect.height);
            Nodes[i].Rect = rect;
        }
        EndWindows();
        EditorGUILayout.EndHorizontal();
        GUI.EndScrollView();

        var e = Event.current;
        if (e.isMouse && e.button == 0)
        {
            foreach (var l in lines)
            {

                if (HandleUtility.DistancePointToLine(e.mousePosition, new Vector2(l.x, l.y), new Vector2(l.z, l.w)) < 6)
                {
                    Debug.Log(lines.IndexOf(l));
                    e.Use();
                    break;
                }
            }
        }
    }

    void WindowFunction(int windowID)
    {
        GUI.DragWindow();
    }

    Vector2 ScrollAreaMax()
    {
        var request = Vector2.zero;
        foreach (var n in Nodes)
        {
            request.x = Mathf.Max(request.x, n.Rect.xMax);
            request.y = Mathf.Max(request.y, n.Rect.yMax);
        }
        return request;
    }

    Vector2 ScrollAreaMin()
    {
        var request = Vector2.zero;
        foreach (var n in Nodes)
        {
            request.x = Mathf.Min(request.x, n.Rect.xMin);
            request.y = Mathf.Min(request.y, n.Rect.yMin);
        }
        return request;
    }

    struct DialogueNode
    {
        public Rect Rect;
        public int[] Connections;
        public DialogueNode(Rect area, params int[] connections)
        {
            Rect = area;
            Connections = connections;
        }
    }
}