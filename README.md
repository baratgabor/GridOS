# GridOS
Modular multitasking and command handling ingame script for Space Engineers. This script provides a framework for creating separate code modules to run on a single Programmable Block.

Experimental version, frequent changes. Primarily made for my own use.

Initial documentation; will be expanded later.

## How does it work

- Each module is a separate class that implements the `IModule` interface. Implementing this interface is what makes you able to register the module in the `GridOS` instance.

- Each module can choose to implement an additional `IUpdateSubscriber` interface for subscribing to recurring automatic execusion, which simply requires setting an `UpdateFrequency` and declaring an `Update()` method.

- Each module can choose to implement an additional `ICommandPublisher` interface for publishing commands, which can be executed both from argument, and from a command menu that this framework provides.

## Example module class with both update subscription and command publishing

The class below, after instantiating it, and registering it in the `GridOS` instance, will have its `Update()` cycle called according to its `UpdateFrequency` setting.

Additionally, the `ExecuteDummyCommand()` method will execute whenever the Programmable Block is invoked with the `"DummyCommand"` argument, plus the same command will be selectable from the GridOS's command menu.

```csharp
public class ExampleModule : IModule, ICommandPublisher, IUpdateSubscriber
{
    public string ModuleDisplayName { get; } = "Example Module";

    public ObservableUpdateFrequency UpdateFrequency { get; } = new ObservableUpdateFrequency(Sandbox.ModAPI.Ingame.UpdateFrequency.Update100);

    public List<CommandItem> Commands => _commands;
    private List<CommandItem> _commands = new List<CommandItem>();

    public ExampleModule()
    {
        _commands.Add(new CommandItem(
            CommandName: "DummyCommand",
            MenuDisplayName: "Dummy Command",
            MenuDisplayPriority: 1,
            FunctionalityName: ModuleDisplayName,
            Execute: ExecuteDummyCommand
        ));
    }

    public void Update(UpdateType updateType)
    {
        // Do something at each update cycle
    }

    private void ExecuteDummyCommand()
    {
        // Do something when command is called
    }
}
```
