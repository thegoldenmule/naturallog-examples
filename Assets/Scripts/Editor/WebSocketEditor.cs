using System.Threading;
using UnityEditor;
using UnityEngine;
using WebSocketSharp;

public class WebSocketEditor : EditorWindow
{
    private static readonly string[] _Levels = {
        "Info",
        "Debug",
        "Warn",
        "Error"
    };

    private static readonly string[] _Messages =
    {
        "This is a short message!",
        "A forgiving album sells the radio skirt.",
        "An overriding adult skips the territory into the sharing catalog.",
        "Why won't any pattern seal an immortal?",
        "The upward bed jerks the choice mercury.",
        "The temper relates the twelve daughter over a charge.",
        "Finally, to those nations who would make themselves our adversary, we offer not a pledge but a request: that both sides begin anew the quest for peace, before the dark powers of destruction unleashed by science engulf all humanity in planned or accidental self-destruction.",
        "But neither can two great and powerful groups of nations take comfort from our present course -- both sides overburdened by the cost of modern weapons, both rightly alarmed by the steady spread of the deadly atom, yet both racing to alter that uncertain balance of terror that stays the hand of mankind's final war."
    };

    private WebSocket _socket;
    private string _uri = "ws://127.0.0.1:9999";
    private bool _started = false;

    [MenuItem("Windows/WebSocketEditor")]
    private static void Open()
    {
        GetWindow<WebSocketEditor>();
    }

    private void OnEnable()
    {
        minSize = new Vector2(200, 200);
    }

    private void OnDisable()
    {
        if (null != _socket)
        {
            _socket.Close();
            _socket = null;
        }
    }

    private void OnGUI()
    {
        _uri = EditorGUILayout.TextField("URI", _uri);

        GUI.enabled = !_started;
        if (GUILayout.Button("Start"))
        {
            _started = true;

            if (null != _socket)
            {
                _socket.Close();
                _socket = null;
            }

            _socket = new WebSocket(_uri);
            _socket.OnOpen += (sender, args) => Debug.Log("OnOpen");
            _socket.OnError += (sender, args) => Debug.Log("OnError : " + args.Message);
            _socket.OnClose += (sender, args) => Debug.Log("OnClose");
            _socket.OnMessage += (sender, args) => Debug.Log("OnMessage");
            _socket.Log.Level = LogLevel.Trace;
            _socket.Connect();
        }

        GUI.enabled = _started;
        if (GUILayout.Button("Stop"))
        {
            _started = false;
        }

        if (_started && Random.Range(0, 100) > 95)
        {
            // send
            try
            {
                _socket.Send(
                    "{" + _Levels[Random.Range(0, _Levels.Length - 1)] + "}:"
                    + _Messages[Random.Range(0, _Messages.Length - 1)]);
            }
            catch
            {
                _socket.Close();
                _socket = null;

                _started = false;
            }
        }

        Repaint();
    }
}