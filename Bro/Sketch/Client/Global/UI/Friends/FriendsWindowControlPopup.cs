using Bro.Toolbox.Client;
using UnityEngine;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

namespace Bro.Sketch.Client.Friends
{
    public class FriendsWindowControlPopup : MonoBehaviour
    {
        [SerializeField] private Button _buttonDelete;
        [SerializeField] private Button _buttonProfile;
        
        private int _userId;
        private FriendsModule _friendsModule;
        
        private void Awake()
        {
            gameObject.SetActive(false);
            _buttonDelete.onClick.AddListener(OnButtonDelete);
            _buttonProfile.onClick.AddListener(OnButtonProfile);
        }

        public void Show(FriendsWindowItemFriend friendItem, FriendsModule friendsModule)
        {
            _friendsModule = friendsModule;
            _userId = friendItem.UserId;
            UpdatePosition(friendItem.transform);
            gameObject.SetActive(true);
        }

        private void UpdatePosition(Transform itemTransform)
        {
            var itemRect = transform as RectTransform;
            var currentRect = itemTransform as RectTransform;
            
            var itemAnchor = itemRect.GetLeftMiddleLocalPosition();
            var currentAnchor = currentRect.GetRightMiddleLocalPosition();

            Debug.Assert(currentRect != null, nameof(currentRect) + " != null");
            Debug.Assert(itemRect != null, nameof(itemRect) + " != null");
            
            itemRect.position = currentRect.position + currentAnchor - itemAnchor;
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }

        private void OnButtonDelete()
        {
            _friendsModule.RemoveFriend(_userId);
            Close();
        }

        private void OnButtonProfile()
        {
            Bro.Log.Error("todo");
            Close();
        }
        
        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && !RectTransformUtility.RectangleContainsScreenPoint((RectTransform) transform, Input.mousePosition))
            {
                Close();
            }
        }
    }
}