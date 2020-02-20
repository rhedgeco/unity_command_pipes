using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace UnityCommandPipes
{
    /// <summary>
    /// Unity-esque abstraction of named pipe server for inter-process commands
    /// </summary>
    public class CommandPipeServer : IDisposable
    {
        private bool _listening;
        private bool _dispose;
        private Coroutine _callbackRoutine;
        private UnityAction<string> _commandCallback;
        private readonly Queue<string> _commandPool = new Queue<string>();

        /// <summary>
        /// Name of the pipe
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Creates a new CommandPipeServer
        /// </summary>
        /// <param name="id">Name of the pipe</param>
        /// <param name="commandCallback">Callback method when receiving commands</param>
        /// <param name="start">Whether or not to start the listener upon creation</param>
        public CommandPipeServer([NotNull] string id, [NotNull] UnityAction<string> commandCallback, bool start = true)
        {
            Id = id;
            _commandCallback = commandCallback;
            if(start) StartListening();
        }

        /// <summary>
        /// Invokes command callback with currently held commands stored in named pipe
        /// </summary>
        /// <remarks>Can be called as little, or as often as needed. (e.g. in an Update method)</remarks>
        public void InvokeCommands()
        {
            while (_commandPool.Count > 0)
            {
                _commandCallback.Invoke(_commandPool.Dequeue());
            }
        }

        /// <summary>
        /// Starts the command pipe listener for client requests
        /// </summary>
        public void StartListening()
        {
            if(_listening) return;
            _listening = true;
            Thread t = new Thread(ListenForCommands);
            t.Start();
        }

        /// <summary>
        /// Private callback for when a client connects to the named pipe
        /// </summary>
        private void ListenForCommands()
        {
            using (NamedPipeServerStream server = new NamedPipeServerStream(Id))
            {
                server.WaitForConnection();
                if (_dispose) return;

                using (StreamReader reader = new StreamReader(server))
                {
                    while (!_dispose)
                    {
                        string message;
                        if ((message = reader.ReadLine()) != null)
                        {
                            _commandPool.Enqueue(message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Disposes thread and pipe server.
        /// </summary>
        /// <remarks>Should always be called before or on application quit.</remarks>
        public void Dispose()
        {
            _dispose = true;

            // Weird hack to unlock "WaitForConnection" that prevents thread from terminating.
            // If a thread is not terminated, the unity editor will freeze the next time it is run.
            // Tried using "BeginWaitForConnection" async method and could not seem to reliably terminate thread.
            // The "temporary" solution is to force connect a new client, thus unlocking the "WaitForConnection".
            // Since we set _dispose to false first, we can just terminate the command loop before it starts.
            // If the client was already connected to, it will throw a Win32Exception and we can basically ignore it.
            // ...
            // I would really like to do this another way, as there may be unforeseen errors that I can't find now.
            using (NamedPipeClientStream abortClient = new NamedPipeClientStream(Id))
            {
                try
                {
                    abortClient.Connect();
                }
                catch (Win32Exception)
                {
                    // do nothing
                }
            }
        }
    }
}