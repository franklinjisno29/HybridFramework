using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Google.Custom_Exceptions
{
    internal class ProjectExceptions : Exception
    {
        public List<string> CustomMessages = new();

        //public ProjectExceptions(string message) : base(message)
        //{
        //    CustomMessages.Add("E1");
        //    CustomMessages.Add("E2");
        //    CustomMessages.Add("E3");
        //}

        public ProjectExceptions(string message) : base(message)
        {
            CustomMessages.Add(message);
        }
    }
}
