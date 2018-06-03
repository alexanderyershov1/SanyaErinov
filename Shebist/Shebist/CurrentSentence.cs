using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shebist
{
    public class CurrentSentence
    {
        public string id, topicId;
        public bool isQuestionFirst;
        public List<string> questions = new List<string>(),
            contexts = new List<string>(),
            translations = new List<string>(),
            waysToQuestionsVoice = new List<string>();
        public string wayToSentenceVoice;

        public static explicit operator Sentence(CurrentSentence cs)
        {
            return new Sentence
            {
                id = cs.id,
                topicId = cs.topicId,
                isQuestionFirst = cs.isQuestionFirst,
                questions = cs.questions,
                contexts = cs.contexts,
                translations = cs.translations,
                waysToQuestionsVoice = cs.waysToQuestionsVoice,
                wayToSentenceVoice = cs.wayToSentenceVoice
            };
        }
    }
}
