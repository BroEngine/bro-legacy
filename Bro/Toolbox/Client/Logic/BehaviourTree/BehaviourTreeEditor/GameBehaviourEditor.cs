#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;
using Event = UnityEngine.Event;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    public class GameBehaviourEditorCache : StaticSingleton<GameBehaviourEditorCache>
    {
        public Node Tree;
        public bool IsGBEditorActive = false;
    }
    
    public class GameBehaviourEditor : EditorWindow
    {
        
        private Node _tree;
        public Node Tree
        {
            set
            {
                if (_tree != value)
                {
                    _tree = value;
                    UpdateStructure();
                }
            }
        }

        private const float WindowWidth = 200;
        private const float WindowHeight = 80;
        private const float WindowSpaceX = 8;
        private const float WindowSpaceY = 40;
        private const float ScaleSpeedFactor = 60f;

        private GUIStyle _nodeStyle;
        private GUIStyle _activeNodeStyle;
        private GUIStyle _pointStyle;

        private Vector2 _offset;
        private Vector2 _drag;

        private readonly Dictionary<int, NodeData> _nodes = new Dictionary<int, NodeData>();
        private readonly List<NodeData> _freeNodes = new List<NodeData>();
        private readonly List<NodeData> _connectedNodes = new List<NodeData>();
        private readonly Dictionary<int, int> _nodesWeight = new Dictionary<int, int>();
        
        private  Dictionary<string, Dictionary<string, Type>> _nodesTypes = new Dictionary<string, Dictionary<string, Type>>();
        
        private float _zoomScale = 1.0f;
        private Vector2 _scrollPosition;
        
        private static GameBehaviourEditor _editor;

        [MenuItem("Window/AI.Behaviour")]
        private static void ShowEditor()
        {
            _editor = GetWindow<GameBehaviourEditor>();
            Bro.Log.Error("SET editor.Tree");

            if (Application.isPlaying)
            {
                _editor.InitOnPlaying();
            }
            else
            {
                _editor.InitOnEditor();
            }
        }

        private void OnEnable()
        {
            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            _nodeStyle.border = new RectOffset(20, 20, 20, 20);
            _nodeStyle.alignment = TextAnchor.MiddleCenter;
            _nodeStyle.fontSize = 11;

            _activeNodeStyle = new GUIStyle();
            _activeNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
            _activeNodeStyle.border = new RectOffset(20, 20, 20, 20);
            _activeNodeStyle.alignment = TextAnchor.MiddleCenter;
            _activeNodeStyle.fontSize = 11;

            titleContent.text = "Bot Behaviour";
        }
        private void InitOnPlaying()
        {
            
            _editor.Tree = GameBehaviourEditorCache.Instance.Tree;
            GameBehaviourEditorCache.Instance.IsGBEditorActive = true;
        }
        
        private void InitOnEditor()
        {
            
            //InitNodeDictionary();
            
        }

        private void Update()
        {

            if (!Application.isPlaying)
            {
                if (_tree != null)
                {
                    _tree = null;
                    UpdateStructure();
                }
            }

            Repaint();
        }

        private static string GetNodeName(Node node)
        {
            var attribute = Attribute.GetCustomAttribute(node.GetType(), typeof(BehaviourNameAttribute)) as BehaviourNameAttribute;
            return attribute != null ? attribute.NodeName : node.GetType().Name;
        }
        
        private static Dictionary<string,PropertyInfo> GetParamProperties(object node)
        {
            Dictionary<string,PropertyInfo> propertyNames = new Dictionary<string,PropertyInfo>();
            var fields = node.GetType().GetProperties().Select(x => new
            {
                property = x,
                attribute = x.GetCustomAttributes(false).FirstOrDefault()
            }).ToList();

            foreach (var property in fields)
            {
                if (property.attribute is BehaviourContentAttribute attribute)
                {
                    propertyNames.Add(attribute.ContentName, property.property);
                }
            }
            return propertyNames;
        }
        
        private static Dictionary<string,FieldInfo> GetParamFields(object node)
        {
            Dictionary<string,FieldInfo> fieldNames = new Dictionary<string,FieldInfo>();
            var fields = node.GetType().GetFields().Select(x => new
            {
                field = x,
                attribute = x.GetCustomAttributes(false).FirstOrDefault()
            }).ToList();

            
            foreach (var field in fields)
            {
                if (field.attribute is BehaviourContentAttribute attribute)
                {
                    fieldNames.Add(attribute.ContentName, field.field);
                }
            }
            return fieldNames;
        }
        
        #region structure

        private int CalculateChildrenWeight(Node node, int initWeight)
        {
            var children = node.Children;
            
            var curWeight = initWeight;
            if (children != null)
            {
                foreach (var child in children)
                {
                    curWeight = CalculateChildrenWeight(child, curWeight);
                    ++curWeight;
                }
                --curWeight;
            }
            _nodesWeight[node.Id] = initWeight;//curWeight;
            return curWeight;
        }

        private void UpdateStructure()
        {
            Bro.Log.Info("UpdateStructure!");
            
            _nodes.Clear();
            _connectedNodes.Clear();
            _nodesWeight.Clear();
            
            if (_tree == null)
            {
                return;
            }

            List<Node> nodesToCheck = new List<Node>();
            nodesToCheck.Add(_tree);
            
            IEnumerable<Node> children;
            for (int i = 0; i < nodesToCheck.Count; ++i)
            {
                var node = nodesToCheck[i];
                _nodes.Add(node.Id, new NodeData(node, _nodeStyle, _activeNodeStyle, WindowWidth, WindowHeight, WindowSpaceX, WindowSpaceY, OnClickAddNode, OnClickRemoveNode));
                children = node.Children;
                if (children != null)
                {
                    nodesToCheck.AddRange(children);
                }
            }

            CalculateChildrenWeight(_tree, 0);

            for (var i = 0; i < nodesToCheck.Count; ++i)
            {
                var parent = nodesToCheck[i];
                children = parent.Children;
                if (children != null)
                {
                    foreach (var child in parent.Children)
                    {
                        _connectedNodes.Add(_nodes[parent.Id]);
                        _connectedNodes.Add(_nodes[child.Id]);
                    }
                }
            }

            var dx = WindowWidth + WindowSpaceX;
            var dy = WindowHeight + WindowSpaceY;

            
            // arrange windows positions
            for (var i = 0; i < nodesToCheck.Count; ++i)
            {
                var parent = nodesToCheck[i];
                children = parent.Children;
                if (children != null)
                {
                    foreach (var child in parent.Children)
                    {
                        var childNode = _nodes[child.Id];
                        var parentNode = _nodes[parent.Id];
                        childNode.BBox = parentNode.BBox;
                        childNode.BBox.x = WindowSpaceX + _nodesWeight[child.Id] * dx;
                        childNode.BBox.y += dy;
                    }
                }
            }
            
            Repaint();

        }

        #endregion

        #region Draw

        private void OnGUI()
        {
            var prevMatrix = GUI.matrix;
            var translation = Matrix4x4.TRS(_scrollPosition, Quaternion.identity, Vector3.one);  
            var scale = Matrix4x4.Scale(new Vector3(_zoomScale, _zoomScale, 1.0f));
            
            BeginWindows();
            
            DrawGrid(20, 0.4f, Color.gray);
            DrawGrid(100, 0.6f, Color.gray);

            GUI.EndGroup();
            GUI.matrix = translation * scale * translation.inverse;
            
            DrawNodes();
            for (int i = 0; i < _connectedNodes.Count; i += 2)
            {
                DrawNodeCurve(_connectedNodes[i].BBox, _connectedNodes[i + 1].BBox);
            }

            EndWindows();
            
            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);
            
            if (GUI.changed) Repaint();

            GUI.BeginGroup(new Rect(0.0f,0.0f, 500f, 300f));
            GUI.matrix = prevMatrix;
            DrawButtons();
        }

        private static void DrawButtons()
        {
            if (!Application.isPlaying)
            {
                var rectNew = new Rect(50, 40, 140f, 30f);
                var rectOpen = new Rect(240, 40, 140f, 30f);
                if (GUI.Button(rectNew, "New tree"))
                {
                    //if (OnClickConnectionPoint != null)
                    //{
                    // OnClickConnectionPoint(this);
                    //}
                }
                
                if (GUI.Button(rectOpen, "Open tree"))
                {
                    //if (OnClickConnectionPoint != null)
                    //{
                    // OnClickConnectionPoint(this);
                    //}
                }
            }
        }
        private void DrawNodes()
        {
            float maxX = float.MinValue, maxY = float.MinValue;
            
            foreach (var item in _nodes)
            {
                var node = item.Value;
                if (node.BBox.xMax > maxX)
                {
                    maxX = node.BBox.xMax;
                }

                if (node.BBox.yMax > maxY)
                {
                    maxY = node.BBox.yMax;
                }

                SetTextContentInNode(node);
                node.Draw();
            }
        }

        private void SetTextContentInNode(NodeData node)
        {
            var targetProperties = GetParamProperties(node.Model);
            var targetFields = GetParamFields(node.Model);
            var guiContent = "";

            foreach (var propertyInfo in targetProperties)
            {
                guiContent += propertyInfo.Key + ": " + propertyInfo.Value.GetValue(node.Model) + "\n";
            }
            foreach (var fieldInfo in targetFields)
            {
                guiContent += fieldInfo.Key + ": " + fieldInfo.Value.GetValue(node.Model) + "\n";
            }

            node.Title = GetNodeName(node.Model) + "\n" + guiContent;
        } 
        
        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            _offset += _drag * 0.5f;
            Vector3 newOffset = new Vector3(_offset.x % gridSpacing, _offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void ProcessEvents(Event e)
        {
            _drag = Vector2.zero;

            switch (e.type)
            {
                case EventType.MouseDown:
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        OnDrag(e.delta);
                    }
                    break;
                case EventType.ScrollWheel:
                    _zoomScale -= e.delta.y / ScaleSpeedFactor;
                    _scrollPosition = e.mousePosition;
                    _zoomScale = Mathf.Clamp(_zoomScale, 0.55f, 1.25f);
                    break;
            }
        }
        
        private void ProcessNodeEvents(Event e)
        {
            if (_nodes != null)
            {
                foreach (var item in _nodes)
                {
                    var node = item.Value;
                
                    bool guiChanged = node.ProcessEvents(e);

                    if (guiChanged)
                    {
                        GUI.changed = true;
                    }
                }
            }
        }
        
        private void OnDrag(Vector2 delta)
        {
            _drag = delta;

            foreach (var item in _nodes)
            {
                var node = item.Value;
                node.Drag(delta);
            }

            GUI.changed = true;
        }
        
        void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = new Vector3(start.x + start.width / 2, start.y + start.height, 0);
            Vector3 endPos = new Vector3(end.x + end.width / 2, end.y, 0);
            Vector3 startTan = startPos + Vector3.up * 20;
            Vector3 endTan = endPos + Vector3.down * 20;
            Color shadowCol = new Color(0, 0, 0, 0.02f);

            for (int i = 0; i < 3; i++)
            {// Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, UnityEngine.Color.black, null, 2);
        }
        
        private void OnClickRemoveNode(NodeData node)
        {

        }
        
        private void OnClickAddNode(NodeData node)
        {

            //if (_tree == null)
            //{
            //    _tree = new Node();
            //}
            //List<Node> nodesToCheck = new List<Node>();
            //nodesToCheck.Add(_tree);
            //
            //IEnumerable<Node> children;
            //for (int i = 0; i < nodesToCheck.Count; ++i)
            //{
            //    var node = nodesToCheck[i];
            //    _nodes.Add(node.Id, new NodeData(node, _nodeStyle, _activeNodeStyle, WindowWidth, WindowHeight, WindowSpaceX, WindowSpaceY));
            //    children = node.Children;
            //    if (children != null)
            //    {
            //        nodesToCheck.AddRange(children);
            //    }
            //}
            //
            //_freeNodes.Add(new NodeData(mousePosition, _nodeStyle, _activeNodeStyle, WindowWidth, WindowHeight, WindowSpaceX, WindowSpaceY));
        }
        #endregion
    }
    

}
#endif