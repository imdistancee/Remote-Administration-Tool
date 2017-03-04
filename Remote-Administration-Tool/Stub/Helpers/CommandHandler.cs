using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stub.Helpers
{
    public class CommandHandler
    {
        //this enum will be used for all the commands.
        public enum Commands : int
        {
            GET_INFO = 0,
            SCREEN_CAPTURE = 1,
            OPEN_PROCESS = 2,
            TASK_LIST = 3,
            KILL_TASK = 4,
            EXIT = 5
        }

        //this is for getting the enum for which command from the data.
        public Commands getCommand(string data)
        {
            //this returns the integer of the command and casts it aswell.
            return (Commands)int.Parse(data[0].ToString());
        }
    }
}
