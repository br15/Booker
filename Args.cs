using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booker
{
    public class Args
    {
        public Args(string[] args, Global global)
        {
            // Validate our parameters.
            if (args == null || ) throw new ArgumentException("");

             // It looks like we've got some parameters. Let's validate them.
            for(int i = 0; i < args.Length; ++i)
            {

                switch(arg)
                {
                    case "gamedate":
                    case "gd":
                        break;

                    case "gametime":
                    case "gt":
                        break;

                    case "userid":
                    case "u":
                        break;

                    case "password":
                    case "p":
                        break;

                    default:
                        // Ignore unknow arguments.
                        break;
                }
            }
        }
    }
}
