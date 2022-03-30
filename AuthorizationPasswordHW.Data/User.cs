using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationPasswordHW.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string HashedPassword { get; set; }
        public string Email { get; set; }

    }
}
