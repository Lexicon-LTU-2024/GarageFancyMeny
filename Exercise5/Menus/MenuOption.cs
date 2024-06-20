using System;
using System.Collections.Generic;
using System.Text;

namespace Exercise5
{
    public class MenuOption
    {
        public string OptionName { get; set; }
        public Action Action { get; set; }
        public Menu SubMenu { get; set; }

        public MenuOption(string optionName, Action action)
        {
            OptionName = optionName;
            Action = action;
            SubMenu = null;
        }
        public MenuOption(string optionName, Menu subMenu)
        {
            OptionName = optionName;
            Action = null;
            SubMenu = subMenu;
        }
    }
}
