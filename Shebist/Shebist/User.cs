using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shebist
{
    [Serializable]
    public class User
    {
        public string Name, Login, Email, Password, ChoiceOfTopicGridVisibility;
        public int CurrentTopicId, IndexOfMainWords;
        public List<int> SequenceOfIndicesOfMainWords = new List<int>();
    }
}
