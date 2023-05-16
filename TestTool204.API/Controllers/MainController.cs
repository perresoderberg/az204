using Microsoft.AspNetCore.Mvc;
using TestTool204.Application.Interfaces;
using TestTool204.Application.Services;
using static TestTool204.Core.Domain.G;

namespace TestTool204.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class MainController : ControllerBase
    {

        IMainService _mainService;

        public MainController(IMainService mainService)
        {
            _mainService = mainService;
        }

        [HttpGet("Initialize")]
        public async Task<IActionResult> Initialize()
        {
            try
            {
                await _mainService.LoadQuestionFileAsync();
                _mainService.ShuffleQuestions();
                return Ok(true);
            }
            catch
            {
                return BadRequest("Error");
            }
        }

        [HttpGet("GetQuestion")]
        public async Task<IActionResult> GetQuestion([FromQuery] int? currentQueueNumber)
        {
            try
            {
                var question = await _mainService.GetQuestionAsync(currentQueueNumber ?? 0);


                _mainService.ShuffleAnswers(question);
                question.currentQueueNumber = currentQueueNumber ?? 0;
                if (question is not null)
                    return Ok(question);
                return NoContent();
            }
            catch
            {
                return BadRequest("Error");
            }
        }
    
        [HttpGet("GenerateLog")]
        public async Task<IActionResult> GenerateLog([FromQuery] string GenerateList)
        {
            try
            {
                GenerateList list = Newtonsoft.Json.JsonConvert.DeserializeObject<GenerateList>(GenerateList.ToString());

                await _mainService.WriteLogFile(list);

                await Task.Delay(1000);
            }
            catch
            {

            }
            return Ok(true);
        }
    }
}
