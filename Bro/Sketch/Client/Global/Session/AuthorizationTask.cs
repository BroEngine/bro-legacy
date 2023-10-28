using System.Collections.Generic;
using Bro.Client;
using Bro.Client.Network;
using Bro.Sketch.Network;

namespace Bro.Sketch.Client
{
    public class AuthorizationTask : SubscribableTask<AuthorizationTask>
    {
        private readonly IClientContext _context;
        private readonly int _sessionId;
        private readonly bool _resetProfile;

        public readonly List<ConfigDetails> ConfigDetailsList = new List<ConfigDetails>();
        public bool InQueue { get; private set; }
        public byte ErrorCode { get; private set; }

        public AuthorizationTask(IClientContext context, int sessionId, bool resetProfile)
        {
            _context = context;
            _sessionId = sessionId;
            _resetProfile = resetProfile;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);
            SendRequest(taskContext);
        }

        private void SendRequest(ITaskContext taskContext)
        {
            var request = NetworkPool.GetOperation<AuthorizationRequest>();
            request.Platform.Value = (byte) Platform.Type;
            request.SessionId.Value = _sessionId;
            request.DeviceId.Value = DeviceId.Value;
            request.ResetProfile.Value = _resetProfile;
            
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(DebugSettings.Instance.Auth.CustomDeviceId))
            {
                request.DeviceId.Value = DebugSettings.Instance.Auth.CustomDeviceId;
            }

            if (!string.IsNullOrEmpty(DebugSettings.Instance.Auth.CustomAppleId))
            {
                request.AppleId.Value = DebugSettings.Instance.Auth.CustomAppleId;
            }

            if (!string.IsNullOrEmpty(DebugSettings.Instance.Auth.CustomGoogleId))
            {
                request.GoogleId.Value = DebugSettings.Instance.Auth.CustomGoogleId;
            }

            if (!string.IsNullOrEmpty(DebugSettings.Instance.Auth.CustomFacebookId))
            {
                request.FacebookId.Value = DebugSettings.Instance.Auth.CustomFacebookId;
            }
#endif
            
            var task = new SendRequestTask<AuthorizationResponse>(_context, request, _context.GetNetworkEngine())
            {
                IncreasedTimeout = true
            };
            
            task.OnComplete += (OnAuthorizationCompleted);
            task.OnFail += (OnAuthorizationFailed);
            task.Launch(taskContext);
        }

        private void OnAuthorizationCompleted(SendRequestTask<AuthorizationResponse> task)
        {
            var response = task.Response;
            var authorized = response.IsAuthorized.IsInitialized && response.IsAuthorized.Value;
            var queued = response.IsAuthorized.IsInitialized && response.Queued.Value;

            ErrorCode = response.ErrorCode.IsInitialized ? response.ErrorCode.Value : (byte)0;
            
            Bro.Log.Info("authorization response authorized = " + authorized + "; queued = " + queued);

            if (!authorized && !queued)
            {
                Fail();
            }
            else
            {
                if (authorized)
                {
                    var userId = response.UserId.Value;
                    var configDetailsParams = response.ConfigDetailsArray.Params;
                    _context.GetModule<SessionModule>().HeroUserId = userId;
                    foreach (var param in configDetailsParams)
                    {
                        ConfigDetailsList.Add(param.Value);
                    }

                    Complete();
                }
                else
                {
                    OnQueued();
                }
            }
        }

        private void OnQueued()
        {
            Bro.Log.Error("authorization OnQueued");
            InQueue = true;
            _context.Scheduler.Schedule(() => SendRequest(TaskContext), 6.0f);
        }

        private void OnAuthorizationFailed(SendRequestTask<AuthorizationResponse> task)
        {
            Bro.Log.Error("authorization failed");
            Fail();
        }
    }
}