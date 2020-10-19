﻿namespace IngameScript
{
    class SettingsMenu : MenuGroup
    {
        public SettingsMenu(IDiagnosticServiceController diagnostics) : base("")
        {
            Label = "Settings";
            
            AddChild(new LogLevelCommand(diagnostics));
        }
    }
}