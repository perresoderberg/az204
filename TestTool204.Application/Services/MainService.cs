using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TestTool204.Core.Domain.G;
using TestTool204.Core.Domain;
using TestTool204.Application.Interfaces;

namespace TestTool204.Application.Services
{
    public class MainService : IMainService
    {
        IIOService _ioService { get; set; }

        public MainService(IIOService ioService)
        {
            _ioService = ioService;
        }

        public async Task LoadQuestionFileAsync()
        {
            await _ioService.ReadLinesFromFileAsync();
        }
        public async Task<QandA> GetQuestionAsync(int numberInQueue)
        {
            if (numberInQueue > QandAService.QandAList.Count)
            {
                //done
                //Debugger.Break();
                return QandAService.QandAList[0];
                ;
            }

            return QandAService.QandAList[numberInQueue - 1];
        }

        public void ShuffleAnswers(QandA question)
        {
            Random rng = new Random();
            if (question.answerKind == AnswerKind.Dropdown)
            {
                var allGroups = question.answers.Select(x => x.answerDropdownGroup).Distinct().ToList();
                List<Answer> newAnswerList = new List<Answer>();
                foreach (var group in allGroups)
                {
                    newAnswerList.AddRange(question.answers.Where(x => x.answerDropdownGroup == group).OrderBy(a => rng.Next()).ToList());
                }
                question.answers.Clear();
                question.answers.AddRange(newAnswerList);
                return;
            }

            var groups = question.answers.Select(x => x.group).Distinct().ToList();
            if (groups.Count > 1)
            {
                List<Answer> newlist = new List<Answer>();
                foreach (var group in groups)
                {
                    newlist.Add(question.answers.Where(x => x.group == group && x.disableOption == 1).First());
                    newlist.AddRange(question.answers.Where(x => x.group == group && x.disableOption == 0).OrderBy(a => rng.Next()));
                }
                question.answers.Clear();
                question.answers.AddRange(newlist);
            }
            else
            {
                question.answers = question.answers.OrderBy(a => rng.Next()).ToList();
            }
        }

        public void ShuffleQuestions()
        {
            Random rng = new Random();

            List<QandA> newlist = QandAService.QandAList.OrderBy(a => rng.Next()).ToList();
            QandAService.QandAList.Clear();
            newlist.ForEach(x => { QandAService.QandAList.Add(x); });
        }

        public async Task WriteLogFile(GenerateList list)
        {
            await _ioService.WriteLogFile(list);
        }
    }
}
