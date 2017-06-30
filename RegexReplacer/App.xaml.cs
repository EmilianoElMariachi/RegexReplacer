using System;
using System.Windows;
using NDesk.Options;

namespace RegexReplacer
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string filePattern;
            string findRegex;
            string replace;

            var optionSet = new OptionSet
            {
                {
                    "fs|files=",
                    "",
                    f => filePattern = f
                },
                {
                    "f|find=",
                    "",
                    f => findRegex = f
                },
                {
                    "r|replace=",
                    "",
                    f => replace = f
                }
            };


            try
            {
                var extra = optionSet.Parse(e.Args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Try '--help' for more information.");
                return;
            }
        }
    }
}