using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace IngameScript
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var t = new ConsoleTest();

            t.ConsoleMenuSimulation();
        }

        class ConsoleTest
        {
            int _numOfDraws = 0;
            readonly FakeDisplay _fakeDisplay = new FakeDisplay();

            public void ConsoleMenuSimulation()
            {
                // Wire GridOS output to Console
                _fakeDisplay.TextWritten += (content) => {
                    _numOfDraws++;
                    Console.Clear();
                    Console.WriteLine(content);
                    Console.WriteLine("\r\nNumber of draws:\r\n" + _numOfDraws + "\r\n");
                };

                GridOS gridOS = new GridOS(new MyGridProgram() {
                    Runtime = new FakeRuntimeInfo(),
                    Me = new FakeProgrammableBlock(),
                    GridTerminalSystem = new FakeGridTerminalSystem()
                });
                gridOS.RegisterModule(new TestModule());
                gridOS.RegisterTextSurface(_fakeDisplay);
                gridOS.Main(string.Empty, UpdateType.None);

                // Simuate recurring execution
                var timer = new Timer((_) => {
                    gridOS.Main(string.Empty, UpdateType.Update100);
                }, null,
                    dueTime: 2000,
                    period: 1000);

                while (true)
                {
                    ConsoleKeyInfo k = Console.ReadKey(true);

                    switch (k.Key)
                    {
                        case ConsoleKey.DownArrow:
                            gridOS.Main("Lcd1Down", UpdateType.None);
                            break;
                        case ConsoleKey.UpArrow:
                            gridOS.Main("Lcd1Up", UpdateType.None);
                            break;
                        case ConsoleKey.Enter:
                            gridOS.Main("Lcd1Select", UpdateType.None);
                            break;
                        case ConsoleKey.Escape:
                            return;
                    }
                }
            }
        }

        class TestModule : IModule, IUpdateSubscriber, IMenuContentPublisher
        {
            public string ModuleDisplayName => "Test Module";

            public ObservableUpdateFrequency Frequency => new ObservableUpdateFrequency(UpdateFrequency.Update100, this);

            public IEnumerable<IMenuItem> MenuItems => _menuItems;
            private readonly List<IMenuItem> _menuItems = new List<IMenuItem>();

            private readonly MenuItem _updatingItem1 = new MenuItem("UpdatingItem1");
            private readonly MenuItem _updatingItem2 = new MenuItem("UpdatingItem2");

            public TestModule()
            {
                MenuGroup menuGroup1 = new MenuGroup("Menu group");
                menuGroup1.AddChild(new MenuItem("Something interesting"));
                menuGroup1.AddChild(new MenuItem("Another interesting item"));
                menuGroup1.AddChild(new MenuItem("Third interesting item"));

                _menuItems.Add(new MenuItem("Item1"));
                _menuItems.Add(new MenuItem("Item2 - This is a longer item to test word wrapping"));
                _menuItems.Add(new MenuItem("Item3"));
                _menuItems.Add(new MenuCommand("Command1", DoSomething));
                _menuItems.Add(new MenuCommand("Command2", DoSomething));
                _menuItems.Add(new MenuCommand("Command3", DoSomething));
                _menuItems.Add(new MenuItem("Item4 - Another word wrapping testing longer item"));
                _menuItems.Add(new MenuItem("Item5"));
                _menuItems.Add(menuGroup1);
                _menuItems.Add(_updatingItem1);
                _menuItems.Add(_updatingItem2);
                _menuItems.Add(new MenuItem("Item6"));
                _menuItems.Add(new MenuItem("Item7"));
                _menuItems.Add(new MenuItem("Item8 is a very long item, it is long because it has a lot of things to communicate to the end user of the menu system"));
                _menuItems.Add(new MenuItem("Item9"));
                _menuItems.Add(new MenuItem("Item10"));
                _menuItems.Add(new MenuItem("Item11"));
            }

            public void Update(UpdateType updateType)
            {
                var rnd = new Random();
                _updatingItem1.Label = $"UpdatingItem1 - {rnd.Next(1, 100)}";
                _updatingItem2.Label = $"UpdatingItem2 - {rnd.Next(1, 100)}";
            }

            private void DoSomething()
            {
                Debug.WriteLine($"Executed Command");
            }
        }
    }
}
