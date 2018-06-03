using System;
using System.Collections.Generic;

namespace Shebist
{
    public class Sentence
    {
        public string id, topicId;
        public bool isQuestionFirst;
        public List<string> questions = new List<string>(),
            contexts = new List<string>(),
            translations = new List<string>(),
            waysToQuestionsVoice = new List<string>();
        public string wayToSentenceVoice;
    }
}
