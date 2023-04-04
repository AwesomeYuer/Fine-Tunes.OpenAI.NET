// See https://aka.ms/new-console-template for more information
using OpenAI.FineTuning;
using OpenAI;

Console.WriteLine("Hello, World!");

var auth = OpenAIAuthentication.LoadFromDirectory();

var openAIClient = new OpenAIClient(auth);




var lines = new List<string>
            {
                new FineTuningTrainingData("Company: BHFF insurance\\nProduct: allround insurance\\nAd:One stop shop for all your insurance needs!\\nSupported:", "yes"),
                new FineTuningTrainingData("Company: Loft conversion specialists\\nProduct: -\\nAd:Straight teeth in weeks!\\nSupported:", "no")
            };

const string localTrainingDataPath = "fineTunesTestTrainingData.jsonl";
await File.WriteAllLinesAsync(localTrainingDataPath, lines);

//var fileData = await openAIClient.FilesEndpoint.UploadFileAsync(localTrainingDataPath, "fine-tune");
//File.Delete(localTrainingDataPath);
