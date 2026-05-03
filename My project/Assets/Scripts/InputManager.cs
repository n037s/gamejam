using UnityEngine;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using System.Threading.Tasks;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private string sessionType = "default-session";

    private bool _isQuitting = false;

    void Update()
    {
        // Check if the Escape key is pressed down
        if (!_isQuitting && Keyboard.current.escapeKey.isPressed)
        {
            _isQuitting = true;
            _ = QuitAsync();
        }
    }

    private async Task QuitAsync()
    {

        try 
        {
            if (MultiplayerService.Instance != null && 
                MultiplayerService.Instance.Sessions.ContainsKey(sessionType))
            {
                await MultiplayerService.Instance.Sessions[sessionType].LeaveAsync();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"[InputManager] Error leaving the session: {e.Message}");
        }

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();

            await Task.Delay(500);
        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
#else
        Application.Quit();
#endif
    }
}