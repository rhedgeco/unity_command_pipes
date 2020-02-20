# Unity Command Pipes
A named pipe abstraction system for sending and receiving inter-process commands in unity.

## Usage
#### Server Example
```c#
public class ExampleServerBehaviour : MonoBehaviour
{
    // store the pipe for access later
    private CommandPipeServer _pipeServer;
    
    private void Awake()
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
```
#### Client Example
Can be done in any other environment/language that supports connecting to named pipes.

This specific example uses the already made c# unity wrapper for it.
```c#
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
```