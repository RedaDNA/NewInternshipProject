using Azure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class User : BaseEntity
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }

        public string Email { get; set; }



        /*    public  User(string username,string password)
            {
                UserName= username;
                    Password=password;

            }*/
    }
}
