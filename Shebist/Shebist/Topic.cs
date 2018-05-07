using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shebist
{
    public class Topic
    {
        public int Id;
        public string Name { get; set; }
        public List<Word> Words = new List<Word>(), currentWords = new List<Word>();
        public List<int> SequenceOfIndices = new List<int>();
        public int CurrentIndex;
    }
}
