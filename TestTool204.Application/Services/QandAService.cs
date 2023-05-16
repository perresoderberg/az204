using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using TestTool204.Core.Domain;
using static TestTool204.Core.Domain.Answer;
using static TestTool204.Core.Domain.G;
using static TestTool204.Core.Domain.QandA;

namespace TestTool204.Application.Services
{
    public class QandAService
    {
        public static List<QandA> QandAList = new List<QandA>();

        public static void CreateQuestionAndAnswer(int questionNumber, List<string> t, AnswerKind kind, List<Answer> ans, List<string> misc, List<string> images)
        {

            QandAList.Add(new QandA() { 
                questionNumber = questionNumber,  
                answerKind = kind,
                text = t,
                answerMisc = misc, 
                answers = ans,
                images = images
            });
        }
    }

 
}
