using CronusScript;

Script script;

if (Script.LoadScript(out script, "../../../TestScripts/test.crs").Level != ScriptError.ErrorLevel.OK)
{
    Console.WriteLine(script.GetError().Message);
}