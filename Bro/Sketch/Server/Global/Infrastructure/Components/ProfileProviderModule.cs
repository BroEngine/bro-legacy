using System;
using System.Collections.Generic;
using Bro.Json;
using Bro.Server.Context;
using Bro.Server.Network;
using Bro.Service;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ProfileProviderModule<T> : IProfileProvider, IServerContextModule where T : IProfileData, new()
    {
        private IServerContext _context;
        private IProfileValidatorModule _profileValidator;
        
        bool IProfileProvider.Load(UserIdentity identity, IClientPeer peer, bool resetProfile, Action<Profile, byte> callback)
        {
            if (peer?.Context == null || callback == null)
            {
                return false;
            }
            
            var loadRequest = new ProfileLoadRequest();
            loadRequest.UserId.Value = identity.UserId;
            loadRequest.DeviceId.Value = identity.DeviceId;
            loadRequest.GoogleId.Value = identity.GoogleId;
            loadRequest.AppleId.Value = identity.AppleId;
            loadRequest.FacebookId.Value = identity.FacebookId;
            loadRequest.AutoCreate.Value = identity.UserId == 0;

            BrokerEngine.Instance.SendRequest(loadRequest, (response) =>
            {
                if (response != null)
                {
                    var loadResponse = (ProfileLoadResponse) response;
                    var userId = loadResponse.UserId.Value;
                    var json = loadResponse.Save.Value;

                    identity.SetUserName(loadResponse.Name.Value);
                    
                    if (resetProfile && _profileValidator != null)
                    {
                        Bro.Log.Info("profile for user id = " + userId + " will be reset");
                        json = _profileValidator.Default();
                    }

                    var facebookId = loadResponse.FacebookId.Value;
                    var googleId = loadResponse.GoogleId.Value;
                    var appleId = loadResponse.AppleId.Value;
                    
                    Bro.Log.Info("profile :: profile loaded for user id = " + userId + " and identity = " + identity + ", save file length = " + json.Length);
                        
                    if (userId > 0)
                    {
                        var profile = new Profile(peer.GetActor());

                        if (_profileValidator != null)
                        {
                            try
                            {
                                json = _profileValidator.Validate(json);
                            }
                            catch (Exception)
                            {
                                Bro.Log.Error("profile validation failed, peer will be disconnect");
                                peer.Context.Scheduler.Schedule(() => callback(null, (byte) AuthorizationErrorCode.ValidationFailed));
                                return;
                            }
                        }

                        try
                        {
                            var data = JsonConvert.DeserializeObject<T>(json, JsonSettings.AutoSettings);
                            data.Initialize();
                            identity.UpdateAuthorization(userId, googleId, appleId, facebookId);
                            profile.SetData(identity, data);
                            peer.Context.Scheduler.Schedule(() => callback(profile, 0));
                        }
                        catch (Exception e)
                        {
                            Alert("profile :: profile deserialization failed for user id = " + userId);
                            Bro.Log.Error(e);
                            peer.Context.Scheduler.Schedule(() => callback(null, 0));
                        }
                    }
                    else
                    {
                        Alert("profile :: profile load failed case returned user id = 0");
                        peer.Context.Scheduler.Schedule(() => callback(null, 0));
                    }
                }
                else
                {
                    Alert("profile :: profile load failed case request is null");
                    peer.Context.Scheduler.Schedule(() => callback(null, 0));
                }
            });

            return true;
        }
        
        void IProfileProvider.Save(Profile profile, IClientPeer peer, Action<bool> callback)
        {
            var json = JsonConvert.SerializeObject(profile.Data, Formatting.None, JsonSettings.AutoSettings);
            var identity = profile.UserIdentity.Clone();
            
            var saveRequest = new ProfileSaveRequest();
            saveRequest.UserId.Value = identity.UserId;
            saveRequest.UserName.Value = identity.UserName;
            saveRequest.SaveData.Value = json;
            
            BrokerEngine.Instance.SendRequest(saveRequest, (response) =>
            {
                var context = peer?.Context ?? _context;
                
                if (response != null)
                {
                    var saveResponse = (ProfileSaveResponse) response;
                    var successfully = saveResponse.Successfully.Value;
                    if (!successfully)
                    {
                        Alert("profile :: profile save result = " + successfully + " for user id = " + identity.UserId);
                    }
                    context.Scheduler.Schedule(() => callback?.Invoke( successfully ));
                }
                else
                {
                    Alert("profile :: profile save timeouts and failed for user id = " + identity.UserId);
                    context.Scheduler.Schedule(() => callback?.Invoke(false));
                }
            });
        }
        
        public void Load(int userId, Action<T> callback)
        {
            var loadRequest = new ProfileLoadRequest();
            loadRequest.UserId.Value = userId;
            loadRequest.DeviceId.Value = string.Empty;
            loadRequest.GoogleId.Value = string.Empty;
            loadRequest.AppleId.Value = string.Empty;
            loadRequest.FacebookId.Value = string.Empty;
            loadRequest.AutoCreate.Value = false;

            BrokerEngine.Instance.SendRequest(loadRequest, (response) =>
            {
                if (response != null)
                {
                    var loadResponse = (ProfileLoadResponse) response;
                    var authorizedUserId = loadResponse.UserId.Value;
                    var json = loadResponse.Save.Value;

                    Bro.Log.Info("profile :: profile loaded for user id = " + userId + " save file length = " + json.Length);
                        
                    if (authorizedUserId > 0 && authorizedUserId == userId)
                    {
                        try
                        {
                            var data = JsonConvert.DeserializeObject<T>(json, JsonSettings.AutoSettings);
                            data.Initialize();
                            callback(data);
                        }
                        catch (Exception e)
                        {
                            Alert("profile :: profile deserialization failed for user id = " + userId);
                            Bro.Log.Error(e);
                            callback(default);
                        }
                    }
                    else
                    {
                        Alert("profile :: profile load failed case returned user id = 0");
                        callback(default);
                    }
                }
                else
                {
                    Alert("profile :: profile load failed case request is null");
                    callback(default);
                }
            });
        }
        
        private void Alert(string message)
        {
            Bro.Log.Error(message);
            #warning todo invoke global alert system
        }
        
        bool IProfileProvider.IsQueueBusy()
        {
            return false;
        }

        void IServerContextModule.Initialize(IServerContext context)
        {
            _context = context;
            _profileValidator = _context.GetModule<IProfileValidatorModule>();
        }

        IList<CustomHandlerDispatcher.HandlerInfo> IServerContextModule.Handlers => null;
        
    }
}