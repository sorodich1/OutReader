using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutReader.Model
{
    public class ScadaUser
    {
        public ScadaUser(string node, string login, string pass)
        {
            Node = node;
            Login = login;
            Pass = pass;
        }
        public string Node { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
    }
}
