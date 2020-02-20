using UnityEngine;

namespace UnityCommandPipes.Examples
{
    /// <summary>
    /// Example class for sending commands through unity itself.
    /// Commands can be sent from other programs and languages.
    /// </summary>
    public class ExampleClientBehaviour : MonoBehaviour
    {
        // store the pipe for access later
        private CommandPipeClient _client;

        private void Start()
        {
            // create the pipe client whenever necessary
            // will connect automatically, unless otherwise specified with optional connect variable
            _client = new CommandPipeClient("TestPipe");
        }

        public void SendTestMessage()
        {
            // send messages however needed with this method
            _client.SendCommand("TestCommand");
        }

        // you usually want to dispose of the client on or before the application ends
        // this is not as necessary as it is for the server, as the client does not spin up any new threads
        private void OnApplicationQuit()
        {
            _client.Dispose();
        }
    }
}