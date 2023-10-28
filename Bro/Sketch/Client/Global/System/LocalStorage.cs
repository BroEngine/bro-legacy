using System;
using Bro.Json;
using UnityEngine;

namespace Bro.Sketch.Client
{
    public class LocalStorage<T> where T : new()
    {
        private readonly string _storageKey;
        private T _model = new T();
        public T Model => _model;

        public LocalStorage(string storageKey)
        {
            _storageKey = storageKey;
        }
        
        public void Load()
        {
            var json = PlayerPrefs.GetString(_storageKey);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    var o = JsonConvert.DeserializeObject<T>(json);
                    if (o != null)
                    {
                        _model = o;
                    }
                }
                catch (Exception e)
                {
                    Bro.Log.Error(e);
                }
            }
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(_model);
            PlayerPrefs.SetString(_storageKey, json);
        }
    }
}