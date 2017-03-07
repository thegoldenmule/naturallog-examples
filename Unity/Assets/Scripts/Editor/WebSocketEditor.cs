using System;
using NaturalLog;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// For testing naturallog-server.
/// </summary>
public class WebSocketEditor : EditorWindow
{
    /// <summary>
    /// Random messages.
    /// </summary>
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

    /// <summary>
    /// NaturalLogger implementation.
    /// </summary>
    private NaturalLogger _logger = new NaturalLogger();

    /// <summary>
    /// IP of the logger to use.
    /// </summary>
    private string _ip = "127.0.0.1";

    /// <summary>
    /// True if the connection has been started.
    /// </summary>
    private bool _started = false;
    
    /// <summary>
    /// Opens the websocket editor.
    /// </summary>
    [MenuItem("Windows/WebSocketEditor")]
    private static void Open()
    {
        GetWindow<WebSocketEditor>();
    }

    /// <summary>
    /// Called to open the editor.
    /// </summary>
    private void OnEnable()
    {
        minSize = new Vector2(200, 200);
    }

    /// <summary>
    /// Called when the editor has been closed.
    /// </summary>
    private void OnDisable()
    {

    }

    /// <summary>
    /// Called every frame to draw controls.
    /// </summary>
    private void OnGUI()
    {
        _ip = EditorGUILayout.TextField("IP", _ip);
        _logger.Identity = EditorGUILayout.TextField("Identity", _logger.Identity);

        // button to start logging
        GUI.enabled = !_started;
        if (GUILayout.Button("Start"))
        {
            _started = true;

            _logger.Connect(_ip);
        }

        // button to stop logging
        GUI.enabled = _started;
        if (GUILayout.Button("Stop"))
        {
            _started = false;
            _logger.Disconnect();
        }

        // if we are logging, log at a random rate
        if (_started && Random.Range(0, 100) > 95)
        {
            var levels = (NaturalLogger.LogLevel[]) Enum.GetValues(typeof (NaturalLogger.LogLevel));

            // log a random message at a random level
            _logger.Log(
                levels[Random.Range(0, levels.Length - 1)],
                _Messages[Random.Range(0, _Messages.Length - 1)]);
        }

        Repaint();
    }
}