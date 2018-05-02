using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shebist
{
    public class Topic
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Word> words { get; set; }
    }
}
