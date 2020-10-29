using System;
using System.Diagnostics;

namespace IngameScript
{
    //
    // Summary:
    //     All programmable block scripts derive from this class, meaning that all properties
    //     in this class are directly available for use in your scripts. If you use Visual
    //     Studio or other external editors to write your scripts, you can derive directly
    //     from this class and have a compatible script template.
    public class MyGridProgram : IMyGridProgram
    {
        //
        // Summary:
        //     Provides access to the grid terminal system as viewed from this programmable
        //     block.
        public IMyGridTerminalSystem GridTerminalSystem { get; set; }
        //
        // Summary:
        //     Gets a reference to the currently running programmable block.
        public IMyProgrammableBlock Me { get; set; }
        //
        // Summary:
        //     Gets the amount of in-game time elapsed from the previous run.
        [Obsolete("Use Runtime.TimeSinceLastRun instead")]
        public TimeSpan ElapsedTime { get; protected set; }
        //
        // Summary:
        //     Gets runtime information for the running grid program.
        public IMyGridProgramRuntimeInfo Runtime { get; set; }
        //
        // Summary:
        //     Allows you to store data between game sessions.
        public string Storage { get; protected set; }
        //
        // Summary:
        //     Prints out text onto the currently running programmable block's detail info area.
        public Action<string> Echo => (s) => Debug.WriteLine(s);

        public bool HasMainMethod => true;

        public bool HasSaveMethod => true;

        IMyGridTerminalSystem IMyGridProgram.GridTerminalSystem
        {
            get { throw new NotImplementedException(); }
            set
            {
                throw new NotImplementedException();
            }
        }
        TimeSpan IMyGridProgram.ElapsedTime
        {
            get { throw new NotImplementedException(); }
            set
            {
                throw new NotImplementedException();
            }
        }
        string IMyGridProgram.Storage
        {
            get { throw new NotImplementedException(); }
            set
            {
                throw new NotImplementedException();
            }
        }
        IMyGridProgramRuntimeInfo IMyGridProgram.Runtime
        {
            get { throw new NotImplementedException(); }
            set
            {
                throw new NotImplementedException();
            }
        }
        Action<string> IMyGridProgram.Echo
        {
            get { throw new NotImplementedException(); }
            set
            {
                throw new NotImplementedException();
            }
        }

        public void Main(string argument)
        {
            throw new NotImplementedException();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
        //public IMyIntergridCommunicationSystem IGC { get; }
    }
}
