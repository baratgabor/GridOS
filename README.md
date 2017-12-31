# GridOS
Modular multitasking and command handling ingame script for Space Engineers. This script provides a framework for creating separate code modules to run on a single Programmable Block.

Experimental version; frequent, breaking changes. Primarily made for my own use.

Initial documentation; will be expanded later.

## How does it work

- Each module is a separate class that implements the `IModule` interface. Implementing this interface is what makes you able to register the module in the `GridOS` instance.

- Each module can choose to implement an additional `IUpdateSubscriber` interface for subscribing to recurring automatic execusion, which simply requires setting an `UpdateFrequency` and declaring an `Update()` method.

- Each module can choose to implement an additional `ICommandPublisher` interface for publishing commands, which can be executed both from argument, and from a command menu that this framework provides.

## Planned Features

- **Expanded selection of update frequencies:** E.g. 200 ticks, 1-2 minutes, etc. Currently the code modules can set only the vanilla update frequencies (`Update1`, `Update10`, `Update100`, and `None`).
- **Load balancing:** Currently, if e.g. 10 modules are registered for the `Update100` tier, all of them will execute in the same Programmable Block invocation (in the same tick). I'm planning to introduce load-balancing, which will offset each module's running cycle. The tradeoff will be a higher base frequency of the Programmable Block itself. This feature will probably be switchable.
- **Built-in support for menu commands with non-static title:** Currently, if you want to change the display title of a menu command, you have to directly change the property of the `CommandItem` instance, and then a "property changed" event built into the `CommandItem` class takes care of notifying the menu to update. But this can get messy pretty fast if you have to manage the title updates of multiple commands.
- **More screens to display, including a configuration screen:** Currently, GridOS' display capability is limited to displaying the menu of the registered commands. I'm planning to introduce multiple screens, for example a configuration or a status screen, or possibly giving the ability for each module to publish their own information/configuration screen.
- **Communication and data sharing between modules:** Currently, the modules are completely separated, but I want to add built-in options for inter-module communication. E.g. a message bus, in which modules can subscribe to topics, and publish payloads on topics.
- **Persistent storage for modules:** At the moment no persistent storage access is available to modules.
- **Exception handling for each module:** The main system will be protected by module exceptions. Either by discarding the malfunctioning module, or by forcing the modules to implement a Reset() method for resetting themselves.

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

## Instantiating the framework and registering a module
The following example shows the current instantiation chain of the framework, along with the registration of a single module. **Note that this is very temporary, and included here only for the sake of completeness.** Most of the dependency instantiation will be moved inside the class to make it easier to use.

```csharp
private GridOS gridOS;

public Program()
{
    Func<UpdateFrequency> _updateFrequencyGetter = () => Runtime.UpdateFrequency;
    Action<UpdateFrequency> _updateFrequencySetter = (x) => Runtime.UpdateFrequency = x;

    IMyTextPanel gridOSDisplay = GridTerminalSystem.GetBlockWithName("GridOSDisplay") as IMyTextPanel;

    gridOS = new GridOS(
        Echo,
        Runtime,
        new UpdateDispatcherAndController1(Echo, _updateFrequencyGetter, _updateFrequencySetter),
        new CommandDispatcher(),
        new GridOSDisplay(new CommandMenu())
    );

    // for showing command menu; optional
    // you can register multiple textpanels, but currently all of them will show the same content
    gridOS.RegisterTextPanel(gridOSDisplay);

    ExampleModule exampleModule = new ExampleModule();
    gridOS.RegisterModule(exampleModule);
}
```
