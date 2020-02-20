using UnityEngine;

namespace UnityCommandPipes.Examples
{
    public class ExampleServerBehaviour : MonoBehaviour
    {
        // store the pipe for access later
        private CommandPipeServer _pipeServer;
        
        private void Start()
        {
            // create the pipe server whenever necessary
            // will start listening automatically, unless otherwise specified with optional start variable
            _pipeServer = new CommandPipeServer("TestPipe", CommandCallback);
        }

        private void Update()
        {
            // invoke commands in update to stay most up to date with current command state
            _pipeServer.InvokeCommands();
        }

        private void CommandCallback(string command)
        {
            // process commands however you see fit
            if (command.Equals("SpawnCube"))
            {
                GameObject.CreatePrimitive(PrimitiveType.Cube);
            }
            
            Debug.Log($"Command : {command}");
        }

        // ALWAYS be sure to dispose of a pipe or you risk the Unity Editor freezing irreversibly.
        private void OnApplicationQuit()
        {
            _pipeServer.Dispose();
        }
    }
}