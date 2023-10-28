#if UNITY_EDITOR
using UnityEngine;

namespace Bro.Toolbox.Logic.BehaviourTree
{
    public enum ConnectionPointNodeType { In, Out }
    
    public class ConnectionPointNode
    {
        private Rect _rect;

        private readonly ConnectionPointNodeType _type;

        private readonly NodeData _node;

        //private Action<ConnectionPointNode> OnClickConnectionPoint;

        public ConnectionPointNode(NodeData node, ConnectionPointNodeType type)
        {
            _node = node;
            _type = type;
            //this.OnClickConnectionPoint = OnClickConnectionPoint;
            _rect = new Rect(0, 0, 20f, 8f);
        }

        public void Draw()
        {
            _rect.x = _node.BBox.x + (_node.BBox.width * 0.5f) - _rect.width * 0.5f;

            switch (_type)
            {
                case ConnectionPointNodeType.In:
                    _rect.y = _node.BBox.y - _rect.height + 8f;
                    break;

                case ConnectionPointNodeType.Out:
                    _rect.y = _node.BBox.y + _node.BBox.height - 8f;
                    break;
            }

            if (GUI.Button(_rect, ""))
            {
                //if (OnClickConnectionPoint != null)
                //{
                   // OnClickConnectionPoint(this);
                //}
            }
        }
    }
}
#endif