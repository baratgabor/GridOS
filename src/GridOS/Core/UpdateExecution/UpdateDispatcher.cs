using System.Collections.Generic;
using System.Linq;
using System;

namespace IngameScript
{
    //|                     Method |      Mean |    Error |   StdDev |
    //|--------------------------- |----------:|---------:|---------:|
    //|   Test_UpdateDispatcher_V2 | 297.26 ns | 1.023 ns | 0.907 ns |
    //|   Test_UpdateDispatcher_V1 |  87.23 ns | 0.736 ns | 0.688 ns |
    //| Test_UpdateDispatcher_Fast |  34.12 ns | 0.261 ns | 0.244 ns |

    class FastUpdateDispatcher : IUpdateDispatcher
    {
        private readonly List<IUpdateSubscriber> _subscribers_once = new List<IUpdateSubscriber>();
        private readonly List<IUpdateSubscriber> _subscribers_update1 = new List<IUpdateSubscriber>();
        private readonly List<IUpdateSubscriber> _subscribers_update10 = new List<IUpdateSubscriber>();
        private readonly List<IUpdateSubscriber> _subscribers_update100 = new List<IUpdateSubscriber>();

        private readonly HashSet<IUpdateSubscriber> _registeredModules = new HashSet<IUpdateSubscriber>();

        private readonly Func<UpdateFrequency> _updateFrequencyGetter;
        private readonly Action<UpdateFrequency> _updateFrequencySetter;

        private readonly ILogger _logger;
        private readonly ProgressIndicator _progress = new ProgressIndicator();

        public FastUpdateDispatcher(ILogger logger, Func<UpdateFrequency> updateFrequencyGetter, Action<UpdateFrequency> updateFrequencySetter)
        {
            _logger = logger;
            _updateFrequencyGetter = updateFrequencyGetter;
            _updateFrequencySetter = updateFrequencySetter;
        }

        public void Add(IUpdateSubscriber module)
        {
            if (_registeredModules.Contains(module))
                return; // TODO: Consider throwing or returning bool.

            module.Frequency.UpdateFrequencyChanged += HandleModuleUpdateFrequencyChanges;
            _registeredModules.Add(module);

            var moduleUpdateFrequency = module.Frequency.Get();

            if ((moduleUpdateFrequency & UpdateFrequency.Once) != 0)
                _subscribers_once.Add(module);

            if ((moduleUpdateFrequency & UpdateFrequency.Update1) != 0)
                _subscribers_update1.Add(module);

            if ((moduleUpdateFrequency & UpdateFrequency.Update10) != 0)
                _subscribers_update10.Add(module);

            if ((moduleUpdateFrequency & UpdateFrequency.Update100) != 0)
                _subscribers_update100.Add(module);

            _logger.Log(LogLevel.Information, "Update Dispatcher has registered module '{0}' for update type '{1}'.", module.ModuleDisplayName, module.Frequency.UpdateTypeEquivalent);
            UpdateMasterUpdateFrequency();
        }

        public void Remove(IUpdateSubscriber module)
        {
            if (!_registeredModules.Contains(module))
                return; // TODO: Consider throwing or returning bool.

            module.Frequency.UpdateFrequencyChanged -= HandleModuleUpdateFrequencyChanges;
            _registeredModules.Remove(module);

            _subscribers_once.Remove(module);
            _subscribers_update1.Remove(module);
            _subscribers_update10.Remove(module);
            _subscribers_update100.Remove(module);

            _logger.Log(LogLevel.Information, "Update Dispatcher has removed module '{0}'.", module.ModuleDisplayName);
            UpdateMasterUpdateFrequency();
        }

        public void Dispatch(UpdateType updateType)
        {
            if (updateType < UpdateType.Update1)
                return;

            _logger.Log(LogLevel.Debug, "{0} Update Dispatcher reports:\r\n", _progress.Get());

            if ((updateType & UpdateType.Once) != 0)
                ExecuteList(_subscribers_once, updateType, clearListAfterExecution: true);

            if ((updateType & UpdateType.Update1) != 0)
                ExecuteList(_subscribers_update1, updateType);

            if ((updateType & UpdateType.Update10) != 0)
                ExecuteList(_subscribers_update10, updateType);

            if ((updateType & UpdateType.Update100) != 0)
                ExecuteList(_subscribers_update100, updateType);
        }

        private void ExecuteList(List<IUpdateSubscriber> list, UpdateType updateType, bool clearListAfterExecution = false)
        {
            var listCount = list.Count;

            _logger.Log(LogLevel.Debug, "- Executing update list: {0}.\r\nModules in list: {1}.", updateType, listCount);

            for (int i = 0; i < listCount; i++)
            {
                var module = list[i];
                try
                {
                    module.Update(updateType);
                    _logger.Log(LogLevel.Debug, "- Updated module: {0}.", module.ModuleDisplayName);
                }
                catch(Exception e)
                {
                    _logger.Log(LogLevel.Error, "Error updating module '{0}'.\r\nMessage:\r\n{1}.", module.ModuleDisplayName, e.Message);
                }
            }

            if (clearListAfterExecution)
                list.Clear();
        }
        
        private void UpdateMasterUpdateFrequency()
        {
            UpdateFrequency newUpdateFrequency = 0;

            if (_subscribers_once.Count > 0)
                newUpdateFrequency |= UpdateFrequency.Once;

            if (_subscribers_update1.Count > 0)
                newUpdateFrequency |= UpdateFrequency.Update1;

            if (_subscribers_update10.Count > 0)
                newUpdateFrequency |= UpdateFrequency.Update10;

            if (_subscribers_update100.Count > 0)
                newUpdateFrequency |= UpdateFrequency.Update100;

            _updateFrequencySetter(newUpdateFrequency);
            _logger.Log(LogLevel.Information, "Master update frequency changed to '{0}'.", newUpdateFrequency);
        }

        private void HandleModuleUpdateFrequencyChanges(IUpdateSubscriber module, UpdateFrequency oldFrequency, UpdateFrequency newFrequency)
        {
            _logger.Log(LogLevel.Information, "UpdateDispatcher: Changing update frequency of module '{0}', from '{1}' to '{2}'.", module.ModuleDisplayName, oldFrequency, newFrequency);

            if (oldFrequency > 0)
            {
                if ((oldFrequency & UpdateFrequency.Once) != 0)
                    _subscribers_once.Remove(module);

                if ((oldFrequency & UpdateFrequency.Update1) != 0)
                    _subscribers_update1.Remove(module);

                if ((oldFrequency & UpdateFrequency.Update10) != 0)
                    _subscribers_update10.Remove(module);

                if ((oldFrequency & UpdateFrequency.Update10) != 0)
                    _subscribers_update10.Remove(module);
            }
            if (newFrequency > 0)
            {
                if ((newFrequency & UpdateFrequency.Once) != 0)
                    _subscribers_once.Add(module);

                if ((newFrequency & UpdateFrequency.Update1) != 0)
                    _subscribers_update1.Add(module);

                if ((newFrequency & UpdateFrequency.Update10) != 0)
                    _subscribers_update10.Add(module);

                if ((newFrequency & UpdateFrequency.Update10) != 0)
                    _subscribers_update10.Add(module);
            }

            UpdateMasterUpdateFrequency();
        }

        public void DisableUpdates()
        {
            _logger.Log(LogLevel.Information, "Disabling recurring updates.");
            _updateFrequencySetter(UpdateFrequency.None);
        }

        public void EnableUpdates()
        {
            _logger.Log(LogLevel.Information, "Enabling recurring updates.");
            UpdateMasterUpdateFrequency();
        }
    }

    /// <summary>
    /// Your friendly class responsible for executing the appropriate modules at each update cycle. Also handles modules' UpdateFrequency changes, and establishes and sets the base UpdateFrequency for the Programmable Block.
    /// This is Strategy1, optimized for efficiently running large number of components with the same UpdateFrequency. Modules' UpdateFrequency changes are relatively expensive with this strategy.
    /// </summary>
    class UpdateDispatcher_v1 : IUpdateDispatcher
    {
        // TODO: replace dictionary with List<KeyValuePair>, since we're iterating it now
        private readonly Dictionary<UpdateType, List<IUpdateSubscriber>> _moduleLists = new Dictionary<UpdateType, List<IUpdateSubscriber>>();
        private readonly List<UpdateFrequency> _allUpdateFrequencies = new List<UpdateFrequency>
        {
            UpdateFrequency.Once,
            UpdateFrequency.Update1,
            UpdateFrequency.Update10,
            UpdateFrequency.Update100
        };

        private readonly Func<UpdateFrequency> _updateFrequencyGetter;
        private readonly Action<UpdateFrequency> _updateFrequencySetter;

        private readonly ILogger _logger;
        private readonly ProgressIndicator _progress = new ProgressIndicator();

        public UpdateDispatcher_v1(ILogger logger, Func<UpdateFrequency> updateFrequencyGetter, Action<UpdateFrequency> updateFrequencySetter)
        {
            _logger = logger;
            _updateFrequencyGetter = updateFrequencyGetter;
            _updateFrequencySetter = updateFrequencySetter;
        }

        public void Add(IUpdateSubscriber module)
        {
            UpdateType updateType = ConvertUpdateFrequency(module.Frequency.Get());
            var TargetModuleList = GetOrCreateModuleList(updateType);

            if (!TargetModuleList.Contains(module))
            {
                module.Frequency.UpdateFrequencyChanged += HandleModuleUpdateFrequencyChanges;
                TargetModuleList.Add(module);

                UpdateMasterUpdateFrequency();
            }
        }

        public void Remove(IUpdateSubscriber module)
        {
            var moduleListEntry = _moduleLists.FirstOrDefault(moduleList => moduleList.Value.Contains(module));

            if (!moduleListEntry.Equals(default(KeyValuePair<UpdateType, IUpdateSubscriber>)))
            {
                Remove(module, moduleListEntry.Value, moduleListEntry.Key);
            }
        }

        public void Remove(IUpdateSubscriber module, List<IUpdateSubscriber> moduleList, UpdateType moduleListKey)
        {
            module.Frequency.UpdateFrequencyChanged -= HandleModuleUpdateFrequencyChanges;
            moduleList.Remove(module);

            // Remove module list for given update type, if list became empty
            if (moduleList.Count == 0)
                _moduleLists.Remove(moduleListKey);

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
            if (updateType < UpdateType.Update1)
                return;

            _logger.Log(LogLevel.Debug, "{0} UpdateDispatcher reports:\r\n", _progress.Get());
            _logger.Log(LogLevel.Debug, "# update tiers: {0}", _moduleLists.Count);

            foreach (var moduleListKeyValue in _moduleLists)
            {
                // If the UpdateType (can be multiple flags) of given list
                // is selected in the currently passed UpdateTypes,
                // execute all modules in that list.
                if ((updateType & moduleListKeyValue.Key) != 0)
                {
                    _logger.Log(LogLevel.Debug, "- Update tier engaged: {0}. Modules in tier: {1}.", moduleListKeyValue.Key, moduleListKeyValue.Value.Count);
                    foreach (var module in moduleListKeyValue.Value)
                    {
                        try
                        {
                            module.Update(updateType);
                        }
                        catch (Exception e)
                        {
                            _logger.Log(LogLevel.Error, "Error updating module {0}. Message: {1}.", ((IModule)module).ModuleDisplayName, e.Message);
                        }

                        _logger.Log(LogLevel.Debug, "- Updated module: {0}.", ((IModule)module).ModuleDisplayName);
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

        private void HandleModuleUpdateFrequencyChanges(IUpdateSubscriber module, UpdateFrequency oldUpdateFrequency, UpdateFrequency newUpdateFrequency)
        {
            // TODO: Make sure the logic is correct here, and executes per expectations in all scenarios...

            // Get module's old UpdateType from its old UpdateFrequency
            UpdateType oldUpdateType = ConvertUpdateFrequency(oldUpdateFrequency);
                
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

        public void DisableUpdates()
        {
            _updateFrequencySetter(UpdateFrequency.None);
        }

        public void EnableUpdates()
        {
            UpdateMasterUpdateFrequency();
        }
    }


    /// <summary>
    /// Your friendly class responsible for executing the appropriate modules at each update cycle. Also handles modules' UpdateFrequency changes, and establishes and sets the base UpdateFrequency for the Programmable Block.
    /// This is Strategy2, with simplified, flat storage, runs marginally slower, but difference is negligible with low number of modules. Modules' UpdateFrequency changes are cheap with this strategy.
    /// </summary>
    class UpdateDispatcher_v2 : IUpdateDispatcher
    {
        private List<IUpdateSubscriber> _moduleList = new List<IUpdateSubscriber>();
        private List<KeyValuePair<UpdateType, IUpdateSubscriber>> _moduleList2 = new List<KeyValuePair<UpdateType, IUpdateSubscriber>>();
        private readonly ILogger _logger;
        private Func<UpdateFrequency> _updateFrequencyGetter;
        private Action<UpdateFrequency> _updateFrequencySetter;

        private ProgressIndicator _progress = new ProgressIndicator();

        public UpdateDispatcher_v2(ILogger logger, Func<UpdateFrequency> updateFrequencyGetter, Action<UpdateFrequency> updateFrequencySetter)
        {
            _logger = logger;
            _updateFrequencyGetter = updateFrequencyGetter;
            _updateFrequencySetter = updateFrequencySetter;
        }

        public void Add(IUpdateSubscriber module)
        {
            if (_moduleList.Contains(module))
                return;

            module.Frequency.UpdateFrequencyChanged += HandleModuleUpdateFrequencyChanges;
            _moduleList.Add(module);
            UpdateMasterUpdateFrequency();
        }

        public void Remove(IUpdateSubscriber module)
        {
            if (!_moduleList.Contains(module))
                return;

            module.Frequency.UpdateFrequencyChanged -= HandleModuleUpdateFrequencyChanges;
            _moduleList.Remove(module);
            UpdateMasterUpdateFrequency();
        }

        public void Dispatch(UpdateType updateType)
        {
            _logger.Log(LogLevel.Debug, _progress.Get() + " UpdateDispatcher report:");
            _logger.Log(LogLevel.Debug, $"# of modules: {_moduleList.Count}");

            foreach (var m in _moduleList)
            {
                if ((updateType & m.Frequency.UpdateTypeEquivalent) != 0)
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
                NewUpdateFrequency |= m.Frequency.Get();
            }

            _updateFrequencySetter(NewUpdateFrequency);
        }

        private void HandleModuleUpdateFrequencyChanges(IUpdateSubscriber module, UpdateFrequency oldUpdateFrequency, UpdateFrequency newUpdateFrequency)
        {
            // Yep, we don't need any of the passed data here.
            UpdateMasterUpdateFrequency();
        }

        public void DisableUpdates()
        {
            _updateFrequencySetter(UpdateFrequency.None);
        }

        public void EnableUpdates()
        {
            UpdateMasterUpdateFrequency();
        }
    }
}

