using System;
using Bro.Client;
using UnityEngine;

namespace Bro.Sketch.Client.Friends
{
    public abstract class FriendsWindowItem : MonoBehaviour
    {
        public virtual int UserId
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        
        public virtual void Setup(Friend friend, IClientContext context, Action<FriendsWindowItemFriend> buttonCallback)
        {
            throw new NotImplementedException();
        }
    }
}