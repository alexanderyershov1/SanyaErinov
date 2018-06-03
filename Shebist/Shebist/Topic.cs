using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Shebist
{
    public class Topic
    {
        public string Id;
        public string Name { get; set; }
        public List<Sentence> Sentences = new List<Sentence>(),
            currentSentences = new List<Sentence>();
        public List<int> SequenceOfIndices;
        public int CurrentIndex;
        
    }
}
