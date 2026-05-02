using UnityEngine; 
using UnityEngine.UIElements; 
using Unity.Services.Authentication;
using System.Threading.Tasks;

namespace menu
{
    [RequireComponent(typeof(UIDocument))]
    public class JoinSessionMenuController : MonoBehaviour
    {
        private TextField m_PseudoField;

        void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            m_PseudoField = root.Q<TextField>("PseudoField");
            if (m_PseudoField != null)
            {
                m_PseudoField.RegisterValueChangedCallback(OnPseudoChanged);
            }
        }

        void OnDisable()
        {
            if (m_PseudoField != null)
            {
                m_PseudoField.UnregisterValueChangedCallback(OnPseudoChanged);
            }
        }

        private void OnPseudoChanged(ChangeEvent<string> evt)
        {
            _ = UpdatePlayerNameAsync(evt.newValue);
        }

        private async Task UpdatePlayerNameAsync(string playerName)
        {
            if (string.IsNullOrWhiteSpace(playerName)) return;
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to update player name: {e.Message}");
            }
        }
    }
}