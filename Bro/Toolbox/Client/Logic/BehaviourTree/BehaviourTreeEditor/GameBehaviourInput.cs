#if UNITY_EDITOR
using Bro.Toolbox.Logic.BehaviourTree;
using UnityEditor;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    //[CustomEditor(typeof(VehicleBehaviour))]
    public class GameBehaviourInput: UnityEditor.Editor
    {
        const int CharacterLayer = 8;
        private void OnSceneGUI()
        {
            if (GameBehaviourEditorCache.Instance.IsGBEditorActive)
            {
                switch (UnityEngine.Event.current.type) {
                    case EventType.MouseUp:
                        break;								
                    case EventType.MouseDown:
                        OnMouseDown();
                        break;
                    case EventType.MouseDrag:
                        break;
                    case EventType.MouseMove:
                        break;
                    case EventType.Layout:
                        break;
                }
            }
        }

        private void OnMouseDown()
        {
            Ray ray = HandleUtility.GUIPointToWorldRay (UnityEngine.Event.current.mousePosition);
            RaycastHit hit;

            var raycast = Physics.Raycast(ray, out hit, 1000f, 1 << CharacterLayer);

            if (raycast)
            {
               // var behaviour = hit.collider.gameObject.GetComponent<CharacterBehaviour>();
               // if (behaviour != null && behaviour.IsAlive && !behaviour.IsHeroOwner)
               // {
               //     var botBattleBehaviour = BattleLifecycle.Instance.BotController.GetBotBattleBehaviourBehaviour(behaviour.Character.OwnerId, (StandardCharacterBehaviour.Type)behaviour.Character.CharacterType);
//
               //     if (botBattleBehaviour != null)
               //     {
               //         GameBehaviourEditorCache.Instance.Tree = botBattleBehaviour.Tree;
               //     }
               //     else
               //     {
               //         Bro.Log.Error("Tree = NULL");
               //     }
               //     
               //     //var tree = behaviour.Character.
               // }
            }
        }
    }
}
#endif