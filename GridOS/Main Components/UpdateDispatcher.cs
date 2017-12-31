using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        /// <summary>
        /// Your friendly class responsible for executing the appropriate modules at each update cycle. Also handles modules' UpdateFrequency changes, and establishes and sets the base UpdateFrequency for the Programmable Block.
        /// This is Strategy1, optimized for efficiently running large number of components with the same UpdateFrequency. Modules' UpdateFrequency changes are relatively expensive with this strategy.
        /// </summary>
        class UpdateDispatcherAndController1 : IUpdateDispatcherAndController
        {
            // TODO: replace dictionary with List<KeyValuePair>, since we're iterating it now
            private Dictionary<UpdateType, List<IUpdateSubscriber>> _moduleLists = new Dictionary<UpdateType, List<IUpdateSubscriber>>();
            private List<UpdateFrequency> _allUpdateFrequencies = new List<UpdateFrequency>
            {
                UpdateFrequency.Once,
                UpdateFrequency.Update1,
                UpdateFrequency.Update10,
                UpdateFrequency.Update100
            };

            private Func<UpdateFrequency> _updateFrequencyGetter;
            private Action<UpdateFrequency> _updateFrequencySetter;

            private Action<string> _echo;
            private ProgressIndicator _progress = new ProgressIndicator();

            public UpdateDispatcherAndController1(Action<string> echo, Func<UpdateFrequency> updateFrequencyGetter, Action<UpdateFrequency> updateFrequencySetter)
            {
                _echo = echo;
                _updateFrequencyGetter = updateFrequencyGetter;
                _updateFrequencySetter = updateFrequencySetter;
            }

            public void Add(IUpdateSubscriber module)
            {
                UpdateType updateType = ConvertUpdateFrequency(module.UpdateFrequency.Get());
                var TargetModuleList = GetOrCreateModuleList(updateType);

                if (!TargetModuleList.Contains(module))
                {
                    module.UpdateFrequency.UpdateFrequencyChanged += HandleModuleUpdateFrequencyChanges;
                    TargetModuleList.Add(module);

                    UpdateMasterUpdateFrequency();
                }
            }

            public void Remove(IUpdateSubscriber module)
            {
                var ModuleListKeyValue = _moduleLists.FirstOrDefault(moduleList => moduleList.Value.Contains(module));

                // If module was found in a list, remove it from the list
                if (ModuleListKeyValue.Equals(default(KeyValuePair<UpdateType, List<IModule>>)))
                {
                    Remove(module, ModuleListKeyValue.Value, ModuleListKeyValue.Key);
                }
            }

            public void Remove(IUpdateSubscriber module, List<IUpdateSubscriber> moduleList, UpdateType moduleListKey)
            {
                module.UpdateFrequency.UpdateFrequencyChanged -= HandleModuleUpdateFrequencyChanges;
                moduleList.Remove(module);

                // Remove module list for given update type, if list became empty
                if (moduleList.Count == 0)
                    _moduleLists.Remove(moduleListKey);

                // TODO: If module is permanently remove, NEED TO PROPERLY DISPOSE ALL MODULE AND MODULE MEMBER REFERENCES FROM MENU SYSTEM TOO
                UpdateMasterUpdateFrequency();
            }

            private List<IUpdateSubscriber> GetOrCreateModuleList(UpdateType updateType)
            {
                // Create module list for given update type, if doesn't yet exist
                if (!_moduleLists.ContainsKey(updateType))
                    _moduleLists[updateType] = new List<IUpdateSubscriber>();

                return _moduleLists[updateType];
            }

            public void Dispatch(UpdateType updateType)
            {
                _echo(_progress.Get() + " UpdateDispatcher reports:");
                _echo($"# update tiers: {_moduleLists.Count}");

                foreach (var moduleListKeyValue in _moduleLists)
                {
                    // If the UpdateType (can be multiple flags) of given list
                    // is selected in the currently passed UpdateTypes,
                    // execute all modules in that list.
                    if ((updateType & moduleListKeyValue.Key) != 0)
                    {
                        //_echo($"- Update tier engaged: {moduleListKeyValue.Key.ToString()}.");
                        foreach (var module in moduleListKeyValue.Value)
                        {
                            module.Update(updateType);
                            //_echo($"-- Dispatched to module: {((IModule)module).ModuleDisplayName}.");
                        }
                    }
                }
            }

            private void UpdateMasterUpdateFrequency()
            {
                UpdateFrequency NewUpdateFrequency = 0;

                foreach (var updateFrequency in _allUpdateFrequencies)
                {
                    foreach (var moduleListKeyValue in _moduleLists)
                    {
                        if ((ConvertUpdateFrequency(updateFrequency) & moduleListKeyValue.Key) != 0)
                        {
                            NewUpdateFrequency |= updateFrequency;
                            break;
                        }
                    }
                }

                _updateFrequencySetter(NewUpdateFrequency);
            }

            private void HandleModuleUpdateFrequencyChanges(ObservableUpdateFrequency obsUpdFreqOfModule, UpdateFrequency oldUpdateFrequency, UpdateFrequency newUpdateFrequency)
            {
                // TODO: Make sure the logic is correct here, and executes per expectations in all scenarios...

                // Get module's old UpdateType from its old UpdateFrequency
                UpdateType oldUpdateType = ConvertUpdateFrequency(oldUpdateFrequency);

                // Find module based on its UpdateFrequency property (which is an object)
                IUpdateSubscriber module = _moduleLists[oldUpdateType].Find((x) => x.UpdateFrequency == obsUpdFreqOfModule);
                
                // Remove module from the old update tier list, and remove the list too if it became empty
                Remove(module, _moduleLists[oldUpdateType], oldUpdateType);
                
                // Re-add module to new update tier list (basically register again), using the new setting inside its UpdateFrequency property
                Add(module);
                
                // Recalculate master update freq.
                UpdateMasterUpdateFrequency();
            }

            private UpdateType ConvertUpdateFrequency(UpdateFrequency updateFrequency)
            {
                UpdateType updateType = UpdateType.None;

                if ((updateFrequency & UpdateFrequency.Once) != 0)
                    updateType |= UpdateType.Once;
                if ((updateFrequency & UpdateFrequency.Update1) != 0)
                    updateType |= UpdateType.Update1;
                if ((updateFrequency & UpdateFrequency.Update10) != 0)
                    updateType |= UpdateType.Update10;
                if ((updateFrequency & UpdateFrequency.Update100) != 0)
                    updateType |= UpdateType.Update100;

                return updateType;
            }
        }

        /// <summary>
        /// Your friendly class responsible for executing the appropriate modules at each update cycle. Also handles modules' UpdateFrequency changes, and establishes and sets the base UpdateFrequency for the Programmable Block.
        /// This is Strategy2, with simplified, flat storage, runs marginally slower, but difference is negligible with low number of modules. Modules' UpdateFrequency changes are cheap with this strategy.
        /// </summary>
        class UpdateDispatcherAndController2 : IUpdateDispatcherAndController
        {
            private List<IUpdateSubscriber> _moduleList = new List<IUpdateSubscriber>();
            private List<KeyValuePair<UpdateType, IUpdateSubscriber>> _moduleList2 = new List<KeyValuePair<UpdateType, IUpdateSubscriber>>();

            private Func<UpdateFrequency> _updateFrequencyGetter;
            private Action<UpdateFrequency> _updateFrequencySetter;

            private Action<string> _echo;
            private ProgressIndicator _progress = new ProgressIndicator();

            public UpdateDispatcherAndController2(Action<string> echo, Func<UpdateFrequency> updateFrequencyGetter, Action<UpdateFrequency> updateFrequencySetter)
            {
                _echo = echo;
                _updateFrequencyGetter = updateFrequencyGetter;
                _updateFrequencySetter = updateFrequencySetter;
            }

            public void Add(IUpdateSubscriber module)
            {
                if (_moduleList.Contains(module))
                    return;

                module.UpdateFrequency.UpdateFrequencyChanged += HandleModuleUpdateFrequencyChanges;
                _moduleList.Add(module);
                UpdateMasterUpdateFrequency();
            }

            public void Remove(IUpdateSubscriber module)
            {
                if (!_moduleList.Contains(module))
                    return;

                module.UpdateFrequency.UpdateFrequencyChanged -= HandleModuleUpdateFrequencyChanges;
                _moduleList.Remove(module);
                UpdateMasterUpdateFrequency();
            }

            public void Dispatch(UpdateType updateType)
            {
                _echo(_progress.Get() + " UpdateDispatcher report:");
                _echo($"# of modules: {_moduleList.Count}");

                foreach (var m in _moduleList)
                {
                    if ((updateType & m.UpdateFrequency.UpdateTypeEquivalent) != 0)
                    {
                        m.Update(updateType);
                    }
                }
            }

            private void UpdateMasterUpdateFrequency()
            {
                UpdateFrequency NewUpdateFrequency = 0;

                foreach (var m in _moduleList)
                {
                    NewUpdateFrequency |= m.UpdateFrequency.Get();
                }

                _updateFrequencySetter(NewUpdateFrequency);
            }

            private void HandleModuleUpdateFrequencyChanges(ObservableUpdateFrequency obsUpdFreqOfModule, UpdateFrequency oldUpdateFrequency, UpdateFrequency newUpdateFrequency)
            {
                // Yep, we don't need any of the passed data here.
                UpdateMasterUpdateFrequency();
            }
        }
    }
}

