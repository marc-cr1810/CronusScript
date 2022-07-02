using CronusScript.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CronusScript
{
    public class Script
    {
        private string Filepath = "";
        private ScriptError Error = new ScriptError() { Level = ScriptError.ErrorLevel.OK };

        private Dictionary<string, CObject> Global = new Dictionary<string, CObject>();

        public Script()
        {
            Global.Add("__CRONUS_SCRIPT_VERSION__", new StringObject(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion));
        }

        public static ScriptError LoadScript(out Script script, string filepath)
        {
            script = new Script();

            if (!File.Exists(filepath))
            {
                script.SetError(ScriptError.ErrorLevel.FATAL, $"Invalid filepath \"{filepath}\"");
                return script.GetError();
            }
            script.Filepath = filepath;

            return new ScriptError() { Level = ScriptError.ErrorLevel.OK };
        }

        private void SetError(ScriptError.ErrorLevel level, string message)
        {
            Error.Level = level;
            Error.Message = message;
        }

        public bool HasError()
        {
            return Error.Level != ScriptError.ErrorLevel.OK;
        }

        public ScriptError GetError()
        {
            return Error;
        }
    }

    public struct ScriptError
    {
        public enum ErrorLevel
        {
            OK,
            WARN,
            ERROR,
            FATAL
        }

        public ErrorLevel Level;
        public string Message;
    }
}
