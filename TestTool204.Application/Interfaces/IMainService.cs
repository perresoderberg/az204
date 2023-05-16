using TestTool204.Core.Domain;

namespace TestTool204.Application.Interfaces
{
    public interface IMainService
    {
        Task<QandA> GetQuestionAsync(int numberInQueue);
        Task LoadQuestionFileAsync();
        void ShuffleAnswers(QandA question);
        void ShuffleQuestions();
        Task WriteLogFile(G.GenerateList list);
    }
}