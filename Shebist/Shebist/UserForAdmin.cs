using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shebist
{
    public class UserForAdmin
    {
        public int id;
        public string login { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string dateOfRegistration { get; set; }
        public string lastEntrance { get; set; }
        public string totalInTheApp { get; set; }
        public string status { get; set; }
    }
}
