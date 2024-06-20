using System;

namespace Exercise5
{
    public interface IUI
    {
        void Clear();
        void SetColor(ConsoleColor foreground, ConsoleColor background);
        void SetColorNormal();

        void DisplayInputHeader(string header);
        void DisplayLog(EventLog log);
        void DisplayMenu(Menu menu, int cursor);

        string GetTextFromUser(string message, bool acceptEmpty = true);
        int GetIntegerFromUser(string message, bool acceptEmpty = false);

        ConsoleKey GetKeyFromUser();
        void PromptUserForKey(string prefix = "");
        void WaitAndClear();

        void Write(string text);
        void WriteLine(string text = "");
        void Write(string text, ConsoleColor foreground);
        void WriteSuccess(string text);
        void WriteWarning(string text);
    }
}