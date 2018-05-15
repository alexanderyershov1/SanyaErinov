using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shebist
{
    public class Topic
    {
        public string Id;
        public string Name { get; set; }
        public List<Word> Words = new List<Word>(), currentWords = new List<Word>();
        public List<short> SequenceOfIndices = new List<short>();
        public short CurrentIndex;
        
    }
}
