using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using static TestTool204.Core.Domain.G;

namespace TestTool204.Core.Domain
{
    public static class G 
    {
        public class GenerateList
        {
            public List<int> easyList;
            public List<int> hardList;
        }
        public enum AnswerType
        {
            NotSet,
            MultiSelect,
            SingleSelect,
            DropdownSelect,
            OrderSelect
        };
        public enum AnswerKind
        {
            NotSet,
            Star,
            Letter,
            Dropdown,
            OrderGroups
        };

        public static string []allDropdownOptions = { };
                
        public static string []allNumbers= { };
        public static string letters = "abcdefghijklmnopqrstuvxyz";

        public static void Init()
        {
            foreach (int c in Enumerable.Range(1, 50))
            {
                allNumbers = allNumbers
                    .Concat(new string[] { c.ToString() + " " })
                    .Concat(new string[] { c.ToString() + "." })
                    .Concat(new string[] { c.ToString() + ")" })
                    .ToArray();
            }
            foreach (int c in Enumerable.Range(1, 9))
            {
                foreach (char l in letters)
                {
                    allDropdownOptions = allDropdownOptions
                    .Concat(new string[] { c.ToString() + l.ToString() } )
                    .ToArray();
                }
            }
        }

        public static int ConvertLetterToNumber(string v)
        {
            switch (v.ToLower())
            {
                case "a":
                    {
                        return 1;
                        break;
                    }
                case "b":
                    {
                        return 2;
                        break;
                    }
                case "c":
                    {
                        return 3;
                        break;
                    }
                case "d":
                    {
                        return 4;
                        break;
                    }
                case "e":
                    {
                        return 5;
                        break;
                    }
                case "f":
                    {
                        return 6;
                        break;
                    }
                case "g":
                    {
                        return 7;
                        break;
                    }
                case "h":
                    {
                        return 8;
                        break;
                    }
                case "i":
                    {
                        return 9;
                        break;
                    }
            }
            return 0;
        }
    }

    public class Answer
    {
        [JsonInclude]
        public AnswerType answerType;

        [JsonInclude]
        public string answerText;

        [JsonInclude]
        public int isCorrectAnswer;

        [JsonInclude]
        public int disableOption;
        

        [JsonInclude]
        public int questionOrder;

        [JsonInclude]
        public int answerDropdownGroup;

        [JsonInclude]
        public int answerNumber;
        
        [JsonInclude]
        public int group;

        [JsonInclude]
        public List<string> groupings;
    }
    public class QandA
    {

        [JsonInclude]
        public List<string> text = new List<string>();

        [JsonInclude]
        public AnswerKind answerKind;

        [JsonInclude]
        public List<Answer> answers = new List<Answer>();

        [JsonInclude]
        public List<string> images = new List<string>();
        
        [JsonInclude]
        public List<string> answerMisc = new List<string>();

        [JsonInclude]
        public int questionNumber = 0;

        [JsonInclude]
        public int currentQueueNumber = 0;

        [JsonInclude]
        public string wholeText { get; set; }


    }

}
