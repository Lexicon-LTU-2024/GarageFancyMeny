using System;
using System.Linq;
using System.Text;

namespace Exercise5
{
    public class ConsoleUI : IUI
    {

        public ConsoleUI()
        {
        }

        public void Clear()
        {
            Console.Clear();
        }

        public void SetColor(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
        }

        public void SetColorNormal()
        {
            Console.ForegroundColor = Const.normalFG;
            Console.BackgroundColor = Const.normalBG;
        }

        public void Write(string text)
        {
            Console.Write(text);
        }

        public void WriteLine(string text = "")
        {
            Console.WriteLine(text);
        }

        public void Write(string text, ConsoleColor foreground)
        {
            Console.ForegroundColor = foreground;
            Write(text);
            Console.ForegroundColor = Const.normalFG;
        }

        public void WriteWarning(string text)
        {
            Write(text, Const.warningFG);
        }

        public void WriteSuccess(string text)
        {
            Write(text, Const.successFG);
        }

        public void DisplayLog(EventLog log)
        {
            var logs = log.GetLogEntries();
            if (logs.Length > 0)
            {
                SetColor(Const.logFG, Const.logHeaderBG);
                WriteLine("Time     Description".PadRight(40));
                SetColor(Const.logFG, Const.normalBG);
                for (int i = logs.Length - 1; i >= 0; i--)
                {
                    WriteLine(logs[i]);
                }
            }
            SetColorNormal();
        }

        public void DisplayMenu(Menu menu, int cursor)
        {
            SetColor(Const.menuFG, Const.menuBG);
            Console.SetCursorPosition(0, 0);
            Console.CursorVisible = false;
            WriteLine(Const.menuDivider);
            WriteLine(GetMenuHeader(menu.MenuName));
            WriteLine(Const.menuDivider);
            WriteLine(Const.menuFeed);
            for (int i = 0; i < menu.Options.Count; i++)
            {
                SetColor(Const.menuFG, Const.menuBG);
                Write("*  ");
                if (i == cursor)
                    SetColor(Const.menuFGcursor, Const.menuBGcursor);
                else
                    SetColor(Const.menuFG, Const.menuBG);
                var name = $" {menu.Options[i].OptionName.PadRight(32, ' ')} ";
                Write(name);
                SetColor(Const.menuFG, Const.menuBG);
                WriteLine("  *");
            }
            var feeds = 7 - menu.Options.Count;
            while (feeds > 0)
            {
                WriteLine(Const.menuFeed);
                feeds--;
            }
            WriteLine(Const.menuFeed);
            WriteLine(Const.menuDivider);
            SetColorNormal();
        }

        private string GetMenuHeader(string name)
        {
            int spaces = 38 - name.Length;
            string header = new String(' ', spaces / 2) + name;
            return $"*{header.PadRight(38)}*";
        }

        public string GetTextFromUser(string message, bool acceptEmpty = true)
        {
            Console.CursorVisible = true;
            bool success = false;
            string input = "";
            while (success == false)
            {
                Write(message);
                input = Console.ReadLine();
                if (input != "" || acceptEmpty)
                {
                    success = true;
                }
            }
            Console.CursorVisible = false;
            return input;
        }

        public int GetIntegerFromUser(string message, bool acceptEmpty = false)
        {
            bool success = false;
            int result = 0;
            while (success == false)
            {
                var input = GetTextFromUser(message, Const.AcceptEmptyString);
                if (acceptEmpty && input == "")
                {
                    success = true;
                    result = -1;
                }
                else
                {

                    success = int.TryParse(input, out result);
                    if (success == false)
                    {
                        WriteWarning(success ? "" : "Write an integer!\n");
                    }
                    else
                    {
                        success = (result >= 0);
                        if (!success)
                        {
                            WriteWarning(success ? "" : "Only positive numbers!\n");
                        }
                    }
                }
            }
            return result;
        }


        public void PromptUserForKey(string prefix = "")
        {
            WriteLine("\nPress a key to return to menu!");
            Console.ReadKey();
        }

        public ConsoleKey GetKeyFromUser()
        {
            return Console.ReadKey(true).Key; // intercept read
        }

        public void WaitAndClear()
        {
            PromptUserForKey(Const.newlines);
            Clear();
        }

        public void DisplayInputHeader(string header)
        {
            SetColor(Const.inputHeaderFG, Const.inputHeaderBG);
            int pads = (40 - header.Length) / 2;
            string text = header.PadLeft(pads + header.Length, ' ');
            text = text.PadRight(40, ' ');
            WriteLine(text);
            SetColorNormal();
        }
    }
}
