using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DoodleModel;

namespace DoodleConsole

{
    class Program
    {
        static void Main(string[] args)
        {
            DTO_Users userObject = new DTO_Users();
            userObject.ID = 1;
            userObject.DisplayName = "Ralph";
            userObject.EmailAddress = "Something@msn.com";
            userObject.Password = "12";
            userObject.Picture = @"\\images\face.png";
            userObject.Active = true;

            Console.WriteLine(userObject.print());

            Console.ReadLine();
        }
    }
}
