#if UNITY_EDITOR
using System;
 
using UnityEditor;
using UnityEngine;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    public class NodeData
    {
        public string Title;
        
        #pragma warning disable 414
        private bool _isDragged;
        private bool _isSelected;
        #pragma warning restore 414

        private readonly ConnectionPointNode _inPoint;
        private readonly ConnectionPointNode _outPoint;

        private GUIStyle _style;
        private readonly GUIStyle _defaultNodeStyle;
        private readonly GUIStyle _selectedNodeStyle;
        
        private Action<NodeData> _onRemoveNode;
        private Action<NodeData> _onAddNode;

        public int Id => Model.Id;
        public Rect BBox;
        public readonly Node Model;

        public NodeData(Node node, GUIStyle nodeStyle, GUIStyle selectedStyle, float windowWidth, float windowHeight, float windowSpaceX, float windowSpaceY, Action<NodeData> OnClickAddNode, Action<NodeData> OnClickRemoveNode)
        {
            Model = node;
            BBox = new Rect(windowSpaceX, windowSpaceY, windowWidth, windowHeight);
            _inPoint = new ConnectionPointNode(this, ConnectionPointNodeType.In);
            _outPoint = new ConnectionPointNode(this, ConnectionPointNodeType.Out);
            _defaultNodeStyle = nodeStyle;
            _selectedNodeStyle = selectedStyle;
            _onRemoveNode = OnClickRemoveNode;
            _onAddNode = OnClickAddNode;
        }
        
        public void Draw()
        {
            _inPoint.Draw();
            _outPoint.Draw();
            _style = Model.IsActive ? _selectedNodeStyle : _defaultNodeStyle;
            GUI.Box(BBox, Title, _style);
        }
        
        public void Drag(Vector2 delta)
        {
            BBox.position += delta;
        }

        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (BBox.Contains(e.mousePosition))
                        {
                            
                            _isDragged = true;
                            GUI.changed = true;
                            _isSelected = true;
                            //_style = _selectedNodeStyle;
                        }
                        else
                        {
                            GUI.changed = true;
                            _isSelected = false;
                            //_style = _defaultNodeStyle;
                        }
                    }
    
                    if (e.button == 1 && BBox.Contains(e.mousePosition))
                    {
                        if (!Application.isPlaying)
                        {
                            ProcessContextMenu();
                            e.Use(); 
                        }
                    }
                    break;
    
                case EventType.MouseUp:
                    _isDragged = false;
                    break;
    
                case EventType.MouseDrag:
                    if (e.button == 0 && _isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }
    
            return false;
        }
    
        private void ProcessContextMenu()
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
            genericMenu.AddItem(new GUIContent("Add node"), false, OnClickAddNode);
            genericMenu.ShowAsContext();
        }
    
        private void OnClickAddNode()
        {
            _onAddNode?.Invoke(this);
        }
        private void OnClickRemoveNode()
        {
            _onRemoveNode?.Invoke(this);
        }
    }
}
#endif