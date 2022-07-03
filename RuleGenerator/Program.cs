namespace RuleGenerator
{
    internal static class Program
    {
        public static Dictionary<string, Rule> Rules = new Dictionary<string, Rule>();

        public static void AddRule(Rule rule)
        {
            if (Rules.ContainsKey(rule.Name))
                return;
            Rules.Add(rule.Name, rule);
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}