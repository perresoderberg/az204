using TestTool204.Core.Domain;

namespace TestTool204.Application.Interfaces
{
    public interface IIOService
    {
        IAsyncEnumerable<string> ReadAsync(StreamReader stream);
        Task ReadLinesFromFileAsync();
        Task WriteLogFile(G.GenerateList list);
    }
}