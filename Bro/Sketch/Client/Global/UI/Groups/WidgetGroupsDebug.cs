using System;
using Bro.Client.Network;
using Bro.Sketch.Network;
using Bro.Toolbox.Client.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Bro.Sketch.Client.Groups
{
    [Window(WindowItemType.Widget)]
    public class WidgetGroupsDebug : Window
    {
        [SerializeField] private Button _buttonCreate;
        [SerializeField] private Button _buttonLeave;
        [SerializeField] private Button _buttonAdd;
        [SerializeField] private Button _buttonRemove;
        [SerializeField] private Button _buttonUpdate;
        [SerializeField] private Text _labelDebug;
        [SerializeField] private Text _labelHero;
        [SerializeField] private InputField _inputField;
        [SerializeField] private InputField _inputDataField;

        private NetworkEventObserver<GroupsDebugEvent> _debugEventObserver;
        private SessionModule _sessionModule; 
        
        private void Awake()
        {
            _buttonCreate.onClick.AddListener(OnButtonCreate);
            _buttonLeave.onClick.AddListener(OnButtonLeave);
            _buttonAdd.onClick.AddListener(OnButtonAdd);
            _buttonRemove.onClick.AddListener(OnButtonRemove);
            _buttonUpdate.onClick.AddListener(OnButtonUpdate);
        }
        
        private void OnDisable()
        {
            SendRequest(GroupDebug.Disable);
        }

        private void Destroy()
        {
            _debugEventObserver.Unsubscribe();
        }

        protected override void OnShow(IWindowArgs args)
        {
            _labelDebug.text = "no info";

            if (_debugEventObserver == null)
            {
                _debugEventObserver = new NetworkEventObserver<GroupsDebugEvent>(Context.GetNetworkEngine());
                _debugEventObserver.Subscribe(OnGroupsDebugEvent);
            }
            
            _sessionModule = Context.Application.GlobalContext.GetModule<SessionModule>();
            _labelHero.text = "hero id = " + _sessionModule.HeroUserId;
            
            SendRequest(GroupDebug.Enable);
        }

        private void OnGroupsDebugEvent(GroupsDebugEvent e)
        {
            var group = e.Group.IsInitialized ? e.Group.Value : null;
            UpdateInfo(group);
        }

        private void UpdateInfo(Group group)
        {
            if (group == null)
            {
                _labelDebug.text = "no group";
                return;
            }

            var debug = string.Empty;

            debug += "group id = " + group.GroupId + "\n";
            
            debug += "[ ";
            foreach (var user in group.Users)
            {
                debug += user + "; ";
            }
            debug += "] \n";
            
            debug += "data = " + group.Data + "\n";
            
            debug += "timestamp = " + group.Timestamp + "; delta = " + (TimeInfo.GlobalTimestamp - group.Timestamp) + "\n";
            
            _labelDebug.text = debug;
        }

        private int GetArgument()
        {
            try
            {
                return int.Parse(_inputField.text);
            }
            catch (Exception) { /* */ }
            return 0;
        }

        private void SendRequest(GroupDebug command)
        {
            var request = NetworkPool.GetOperation<GroupsDebugRequest>();
            request.Argument.Value = GetArgument();
            request.Data.Value = _inputDataField.text;
            request.Command.Value = (byte) command;
            var task = new SendRequestTask(request, Context.GetNetworkEngine());
            task.Launch(Context);
        }

        private void OnButtonCreate()
        {
            SendRequest(GroupDebug.Create);
        }
        
        private void OnButtonLeave()
        {
            SendRequest(GroupDebug.Leave);
        }
        
        private void OnButtonAdd()
        {
            SendRequest(GroupDebug.Add);
        }
        
        private void OnButtonRemove()
        {
            SendRequest(GroupDebug.Remove);
        }

        private void OnButtonUpdate()
        {
            SendRequest(GroupDebug.Update);
        }
    }
}