using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Diagnostics;
using TestTool204.Application.Interfaces;
using TestTool204.Core.Domain;
using static TestTool204.Core.Domain.G;

namespace TestTool204.Application.Services
{
    public class IOService : IIOService
    {
        string filename = "204Questions.txt";
        string logfilename = "LogEasyHard.txt";
        string workingDirectory, projectDirectory;
        private readonly IConfiguration _configuration;

        public IOService(IConfiguration configuration)
        {
            workingDirectory = Environment.CurrentDirectory;
            projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            _configuration = configuration;
        }

        public async IAsyncEnumerable<string> ReadAsync(StreamReader stream)
        {
            while (!stream.EndOfStream)
                yield return await stream.ReadLineAsync();
        }
        public async Task ReadLinesFromFileAsync()
        {
            G.Init();

            int questionNumber = 0;
            bool newQuestion = false;
            bool insertTextMode = false;
            bool insertAnswer = false;
            AnswerKind currentAnswerKind = AnswerKind.NotSet;

            List<string> text = new List<string>();
            List<string> answerMisc = new List<string>();

            List<Answer> answers = new List<Answer>();

            List<List<string>> groupings = new List<List<string>>();
            List<string> currentGroup = new List<string>();
            List<string> images = new List<string>();

            bool fetchGrouping = false;

            var storageAccount = _configuration["StorageAccountConnectionString"];
            var container = _configuration["Container"];

            BlobClient blobClient = new BlobClient(storageAccount, container, "204Questions.txt");
            
            await using var stream = await blobClient.OpenReadAsync();
            using var reader = new StreamReader(stream);

            await foreach (var line in ReadAsync(reader))
            {



            //string filenamepath = Path.Combine(projectDirectory, filename);
            //if (File.Exists(filenamepath))
            //{
            //    StreamReader Textfile = new StreamReader(filenamepath);



                //while ((line = await Textfile.ReadLineAsync()) != null)
                //{
                    if (line.Length > 0 && line[0] == '#')
                        continue;


                    switch (line)
                    {
                        case string x when x.Length > 3 && line.Substring(0, 3) == "***":
                            {
                                if (answers.Count > 0)
                                {
                                    QandAService.CreateQuestionAndAnswer(questionNumber, text, currentAnswerKind, answers, answerMisc, images);

                                    questionNumber = 0;
                                    newQuestion = false;
                                    insertTextMode = false;
                                    insertAnswer = false;
                                    currentAnswerKind = AnswerKind.NotSet;

                                    text = new List<string>();
                                    answerMisc = new List<string>();
                                    answers = new List<Answer>();
                                    groupings.Clear();
                                }
                                newQuestion = true;
                                insertTextMode = false;

                                break;
                            }
                        case string x when x.Length > 2 && x.Length < 8 && line.Substring(0, 2) == "N:":
                            {
                                string rest = line.Substring(2);
                                questionNumber = Int32.Parse(rest);
                                newQuestion = false;
                                break;
                            }
                        case string x when x.Length > 0 && line.Substring(0, 1) == "T":
                            {
                                newQuestion = false;
                                insertTextMode = true;
                                break;
                            }
                        case string x when x.Length > 5 && line.Substring(0, 5).ToLower() == "image":
                            {
                                newQuestion = false;
                                string[] answerText = line.Split(new[] { ' ' }, 2);
                                if (answerText.Length != 2)
                                    Debugger.Break();
                                images.Add(answerText[1]);
                                if (insertTextMode)
                                    text.Add($"image{images.Count}");
                                if (insertAnswer)
                                    answerMisc.Add($"image{images.Count}");
                                break;
                            }
                        case string x when x.Length > 5 && line.Substring(0, 6) == "group-":
                            {
                                insertTextMode = false;
                                newQuestion = false;
                                currentAnswerKind = AnswerKind.OrderGroups;
                                string[] answerText = line.Split(new[] { '-' });
                                if (answerText.Length != 3)
                                    Debugger.Break();
                                int groupNumber = Int32.Parse(answerText[1].ToString());
                                bool startTag = answerText[2].ToLower() == "start";
                                bool endTag = answerText[2].ToLower() == "end";
                                if (startTag)
                                    fetchGrouping = true;
                                if (endTag)
                                {
                                    groupings.Add(currentGroup.ToList());
                                    currentGroup.Clear();
                                    fetchGrouping = false;

                                }


                                break;
                            }
                        case string x when x.Length > 0 && fetchGrouping:
                            {
                                currentGroup.Add(x);
                                break;
                            }
                        case string x when x.Length > 1 && insertAnswer == false && allDropdownOptions.Contains(line.Substring(0, 2).ToLower()):
                            {
                                insertTextMode = false;
                                newQuestion = false;
                                currentAnswerKind = AnswerKind.Dropdown;

                                string[] answerText = line.Split(new[] { ' ' }, 2);
                                if (answerText.Length != 2)
                                    Debugger.Break();
                                char g = answerText[0][0];
                                char num = answerText[0][1];
                                int group = Int32.Parse(g.ToString());
                                int number = G.ConvertLetterToNumber(num.ToString());

                                answers.Add(new Answer() { answerDropdownGroup = group, answerNumber = number, answerText = answerText[1] });
                                break;
                            }
                        case string x when x.Length > 3 && currentAnswerKind != AnswerKind.Dropdown && insertAnswer == false && (allNumbers.Contains(line.Substring(0, 2)) || allNumbers.Contains(line.Substring(0, 3))):
                            {
                                insertTextMode = false;
                                newQuestion = false;
                                currentAnswerKind = AnswerKind.Letter;

                                string[] answerText = line.Split(new[] { ' ' }, 2);
                                if (answerText.Length != 2)
                                    Debugger.Break();

                                int group = 0;
                                if (char.IsDigit(answerText[1][0]) && answerText[1][1] == '_')
                                {
                                    group = Int32.Parse(answerText[1][0].ToString());
                                    answerText[1] = answerText[1].Substring(2);
                                }

                                int disableOption = 0;
                                if (answerText[1][0] == '[')
                                    disableOption = 1;

                                answers.Add(new Answer() { answerNumber = answers.Count + 1, answerText = answerText[1], disableOption = disableOption, group = group });
                                break;
                            }
                        case string x when x.Length > 2 && currentAnswerKind != AnswerKind.Dropdown && insertAnswer == false && line.Substring(0, 2) == "* ":
                            {
                                insertTextMode = false;
                                newQuestion = false;
                                currentAnswerKind = AnswerKind.Star;

                                string[] answerText = line.Split(new[] { ' ' }, 2);
                                if (answerText.Length != 2)
                                    Debugger.Break();

                                int disableOption = 0;
                                if (answerText[1][0] == '[')
                                    disableOption = 1;

                                answers.Add(new Answer() { answerNumber = answers.Count + 1, answerText = answerText[1], disableOption = disableOption });
                                break;
                            }
                        case string s when s.Length > 0 && insertTextMode:
                            {
                                newQuestion = false;
                                text.Add(line);
                                break;
                            }
                        case string x when x.Length > 6 && line.Substring(0, 6).ToUpper() == "ATYPE:":
                            {
                                string endofline = line.Substring(6, line.Length - 6);
                                if (endofline.ToLower() == "multiselect")
                                {
                                    answers.ForEach(x => { x.answerType = AnswerType.MultiSelect; });
                                }
                                else if (endofline.ToLower() == "singleselect")
                                {
                                    answers.ForEach(x => { x.answerType = AnswerType.SingleSelect; });
                                }
                                else if (endofline.ToLower() == "dropdown")
                                {
                                    answers.ForEach(x => { x.answerType = AnswerType.DropdownSelect; });
                                }
                                else if (endofline.ToLower() == "order")
                                {
                                    foreach (List<string> groupAnswer in groupings)
                                    {
                                        answers.Add(new Answer() { answerNumber = answers.Count + 1, groupings = groupAnswer, answerType = AnswerType.OrderSelect });
                                    }
                                }
                                break;
                            }
                        case string x when x.Length > 7 && line.Substring(0, 7).ToUpper() == "ANSWER:":
                            {
                                string endofline = line.Substring(7, line.Length - 7);
                                string[] parsed = endofline.Split(',');
                                if (currentAnswerKind == AnswerKind.Dropdown)
                                {
                                    foreach (string part in parsed)
                                    {
                                        int group = Int32.Parse(part.Substring(0, 1));
                                        int number = G.ConvertLetterToNumber(part.Substring(1, 1));

                                        answers.Where(x => x.answerDropdownGroup == group && x.answerNumber == number).ToList().ForEach(x => { x.isCorrectAnswer = 1; });

                                    }
                                }
                                else if (currentAnswerKind == AnswerKind.OrderGroups)
                                {
                                    for (var i = 0; i < parsed.Count(); i++)
                                    {
                                        var answerPart = Int32.Parse(parsed[i]);
                                        for (var j = 0; j < answers.Count; j++)
                                        {
                                            if (answerPart == answers[j].answerNumber)
                                            {
                                                answers[j].questionOrder = i + 1;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (Answer a in answers)
                                    {
                                        if (parsed.Any(x => x.Contains(a.answerNumber.ToString())))
                                        {
                                            a.isCorrectAnswer = 1;
                                        }
                                    }
                                }
                                insertAnswer = true;
                                break;
                            }
                        case string s when s.Length > 0 && insertAnswer:
                            {
                                answerMisc.Add(line);
                                break;
                            }

                        default:
                            break;

                    }


            }

                /*
<select id = "dropdown" onchange = "selectOption()">
<option>BMW</option>
<option>Range Rover</option>
<option>Mercedes</option>
<option>Honda city</option>
<option>Verna</option>
<option>Tata Safari</option>
</select>

  */
                foreach (var QandA in QandAService.QandAList)
                {
                    string wholeText = "";
                    foreach (var item in QandA.text)
                    {
                        if (wholeText != "")
                        {
                            wholeText += "<br/>";
                        }

                        wholeText += item;
                    }
                    QandA.wholeText = wholeText;

                    //if (QandA.answers.First().answerKind == AnswerKind.Dropdown)
                    //{

                    //    int idx = 1;
                    //    for (int i = 0; i < QandA.text.Count; i++)
                    //    {
                    //        string t = QandA.text[i];

                    //        if (t.Contains($"dropdown{idx}"))
                    //        {
                    //            var grouplist = QandA.answers.Where(x => x.answerDropdownGroup == idx).Select(x => x.answerText).ToList();

                    //            string html = "<select id=\"dropdown\" onchange=\"selectOption()\">";
                    //            grouplist.ForEach(x =>
                    //            {
                    //                html += $"<option>{x}</option>";
                    //            });
                    //            html += "</select>";

                    //            QandA.text[i] = t.Replace($"dropdown{idx}", html);
                    //            idx++;
                    //        }
                    //    }

                    //}
                }


                //Textfile.Close();

        }

        public Task WriteLogFile(GenerateList list)
        {
            string filenamepath = Path.Combine(projectDirectory, logfilename);
            using (StreamWriter writer = new StreamWriter(filenamepath))
            {
                writer.WriteLine("Easy");
                string easy = string.Join(',', list.easyList);
                writer.WriteLine(easy);
                writer.WriteLine("");
                writer.WriteLine("Hard");
                string hard = string.Join(',', list.hardList);
                writer.WriteLine(hard);
            }
            // Read a file
            //string readText = File.ReadAllText(filenamepath);
            //Console.WriteLine(readText);
            return Task.FromResult(0);
        }
    }
}
