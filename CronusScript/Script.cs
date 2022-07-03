using CronusScript.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CronusScript.Parser;

namespace CronusScript
{
    public class Script
    {
        private StringObject Filepath;
        private ScriptError Error = new ScriptError() { Level = ScriptError.ErrorLevel.OK };

        private Dictionary<string, CObject> Global = new Dictionary<string, CObject>();

        public Script(string filepath)
        {
            if (!File.Exists(filepath))
            {
                SetError(ScriptError.ErrorLevel.FATAL, $"Invalid filepath \"{filepath}\"");
            }
            Filepath = new StringObject(filepath);

            Global.Add("__CRONUS_SCRIPT_VERSION__", 
                new StringObject(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion));
            Global.Add("__FILEPATH__", Filepath);
        }

        public static ScriptError LoadScript(out Script script, string filepath)
        {
            script = new Script(filepath);
            if (script.HasError())
                return script.Error;

            script.Global.Add("__MAIN__", new NullObject());

            Generator.RunParser(script.Filepath);

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
