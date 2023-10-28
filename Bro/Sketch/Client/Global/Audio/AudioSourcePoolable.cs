using Bro.Client;
using UnityEngine;

namespace Bro.Sketch.Client
{
    public class AudioSourcePoolable : MonoBehaviour, Bro.IPoolable
    {
        public AudioSource AudioSource;

       

        public void OnPoolOut()
        {
            gameObject.SetActive(true);
        }

        public void OnPoolIn()
        {
            gameObject.SetActive(false);
        }

   
    }
}