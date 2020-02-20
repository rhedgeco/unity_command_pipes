using System;
using System.IO;
using System.IO.Pipes;

namespace UnityCommandPipes
{
    /// <summary>
    /// Unity-esque abstraction of named pipe client for inter-process commands
    /// This is not the only way the pipe can be used. The pipe can be connected to from any language with named pipes
    /// </summary>
    public class CommandPipeClient : IDisposable
    {
        private NamedPipeClientStream _client;

        /// <summary>
        /// Name of the pipe
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Creates a new CommandPipeClient
        /// </summary>
        /// <param name="id">Name of the pipe</param>
        /// <param name="connect">Whether or not to connect the client upon creation</param>
        public CommandPipeClient(string id, bool connect = true)
        {
            Id = id;
            _client = new NamedPipeClientStream(Id);
            if (connect) ConnectToServer();
        }

        /// <summary>
        /// Connects the client to the target pipe
        /// </summary>
        /// <param name="timeout">Milliseconds to time out connection attempt</param>
        public void ConnectToServer(int timeout = -1)
        {
            if (timeout < 0) _client.Connect();
            else _client.Connect(timeout);
        }

        /// <summary>
        /// Sends a command to the Command Pipe
        /// </summary>
        /// <param name="command">Command to be sent</param>
        /// <remarks>Will fail if not connected already</remarks>
        public void SendCommand(string command)
        {
            using (StreamWriter writer = new StreamWriter(_client))
            {
                writer.WriteLine(command);
            }
        }

        /// <summary>
        /// Disposes pipe client.
        /// </summary>
        /// <remarks>Should usually be called before or on application quit.</remarks>
        public void Dispose()
        {
            _client.Close();
        }
    }
}