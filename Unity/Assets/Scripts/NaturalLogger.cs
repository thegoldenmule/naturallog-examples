using System;
using WebSocketSharp;

namespace NaturalLog
{
    /// <summary>
    /// Basic implementation of logger for NaturalLog server.
    /// </summary>
    public class NaturalLogger : IDisposable
    {
        /// <summary>
        /// Enumeration of all log levels.
        /// </summary>
        public enum LogLevel
        {
            Info,
            Debug,
            Warn,
            Error
        }

        /// <summary>
        /// To identify with log server.
        /// </summary>
        private string _identity;

        /// <summary>
        /// The socket over which to communicate.
        /// </summary>
        private WebSocket _socket;

        /// <summary>
        /// String identifier. This allows the client to identify with the log
        /// server and name the tab. If this is unset, 'Unidentified' is used.
        /// </summary>
        public string Identity
        {
            get { return _identity; }
            set
            {
                value = string.IsNullOrEmpty(value)
                    ? "Unidentified"
                    : value;

                if (value != _identity)
                {
                    _identity = value;

                    Identify();
                }
            }
        }

        /// <summary>
        /// Called when the logger has become disconnected.
        /// </summary>
        public event Action OnDisconnected;

        /// <summary>
        /// Connects to an ip + port.
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void Connect(string ip, int port = 9999)
        {
            Disconnect();

            _socket = new WebSocket(string.Format("ws://{0}:{1}", ip, port));
            _socket.OnOpen += (_, __) => Identify();
            _socket.OnClose += (_, ___) =>
            {
                if (null != OnDisconnected)
                {
                    OnDisconnected();
                }
            };
            _socket.Log.Level = WebSocketSharp.LogLevel.Trace;
            _socket.ConnectAsync();
        }

        /// <summary>
        /// Disconnects from server.
        /// </summary>
        public void Disconnect()
        {
            if (null != _socket)
            {
                _socket.Close();
                _socket = null;
            }
        }

        /// <summary>
        /// IDispose implementation, since WebSocket implements.
        /// </summary>
        public void Dispose()
        {
            // WebSocker::IDispose just calls ::Close()
            if (null != _socket)
            {
                _socket.Close();
                _socket = null;
            }
        }

        /// <summary>
        /// Sends a log.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LogLevel level, string message)
        {
            Send(level.ToString(), message);
        }

        /// <summary>
        /// Identifies with the server.
        /// </summary>
        private void Identify()
        {
            Send("Identify", _identity);
        }

        /// <summary>
        /// Sends a command + value to the server.s
        /// </summary>
        /// <param name="command"></param>
        /// <param name="value"></param>
        private void Send(string command, string value)
        {
            if (null != _socket && _socket.IsAlive)
            {
                _socket.Send("{" + command + "}:" + value);
            }
        }
    }
}