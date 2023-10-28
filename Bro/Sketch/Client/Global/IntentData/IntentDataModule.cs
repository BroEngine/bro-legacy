using System;
using System.Collections;
using System.Collections.Generic;
using Bro.Client;

#if UNITY_ANDROID || UNITY_IOS
using UnityEngine;
#endif

namespace Bro.Sketch.Client
{
    public class IntentDataModule : IClientContextModule
    {
        private readonly List<string> _processedInviteHashed = new List<string>();
        
        protected IClientContext _context;
        
        public IList<CustomHandlerDispatcher.HandlerInfo> Handlers => new List<CustomHandlerDispatcher.HandlerInfo>();
        
        void IClientContextModule.Initialize(IClientContext context)
        {
            _context = context;
        }
        
        public IEnumerator Load()
        {
            yield return null;
        }

        public IEnumerator Unload()
        {
            yield return null;
        }
        
        private string GetIntentData()
        {
            #if UNITY_EDITOR
            return string.Empty;
            #endif
            
            #if UNITY_ANDROID
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
            var currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var intent = currentActivity.Call<AndroidJavaObject>("getIntent");
                
            var dataString  = intent.Call<string> ("getDataString");
            return dataString;
            
            #endif

            return string.Empty;
        }
        
        public void SetHashProcessed(string hash)
        {
            _processedInviteHashed.Add(hash);
        }
        
        public void InvokeShare(string link)
        {
            #if UNITY_EDITOR
            Bro.Log.Error("share link = " + link);
            return;
            #endif
            
            #if UNITY_ANDROID 
            var intentClass = new AndroidJavaClass("android.content.Intent");
            var intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), link);
            var unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            var jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "Share Via");
            currentActivity.Call("startActivity",jChooser);
            #endif
        }

        public string GetFriendInvitingHash()
        {
            var intentData = GetIntentData();
            if (!string.IsNullOrEmpty(intentData))
            {
                var data = intentData.Split(new string[] { "invite=" }, StringSplitOptions.None);
                if ( data.Length == 2 )
                {
                    var hash = data[1];
                    if (IdentificatorHash.IsValid(hash) && ! _processedInviteHashed.Contains( hash ))
                    {
                        return hash;
                    }
                }
            }
            return string.Empty;
        }
    }
}