using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booker
{
    public class BookCourt
    {
        public BookCourt() { }

        public void Run(Global global)
        {
            bool result = LogOn(global.UserId, global.Password);

            result = LogOff();
        }

        private bool LogOn(string userId, string password)
        {
            bool result = true;
            Console.WriteLine($"Attempting to log on using user id: '{userId}'");



            Console.WriteLine("We successfully logged on.");
            return result;
        }

        private bool LogOff()
        {
            bool result = true;

            Console.WriteLine("Attempting to log off.");



            Console.WriteLine("We successfully logged off.");

            return result;
        }


        private bool TryToBookCourt(Global global)
        {
            bool result = true;


            return result;
        }

    }
}
