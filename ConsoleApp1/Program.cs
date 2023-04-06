// See https://aka.ms/new-console-template for more information

// if you want to CreateFineTuneJobRequest at first 
// please remove the suffix, preserve "needCreateFineTuneJobRequest" only as below:
# define needCreateFineTuneJobRequest_111

using OpenAI;

#if needCreateFineTuneJobRequest
using OpenAI.FineTuning;
#endif

Console.WriteLine("Hello, World!");

var auth = OpenAIAuthentication.LoadFromDirectory();

var openAIClient = new OpenAIClient(auth);

var files = Directory.GetFiles(@"data\", "*.jsonl");
foreach (var file in files)
{
    string? customFineTunedModel = null;

#if needCreateFineTuneJobRequest
    var fileData = await openAIClient
                            .FilesEndpoint
                            .UploadFileAsync(file, "fine-tune");
    var fineTuneJobRequest = new CreateFineTuneJobRequest(fileData, model: "davinci");
    var fineTuneJob = await openAIClient
                                    .FineTuningEndpoint
                                    .CreateFineTuneJobAsync(fineTuneJobRequest);

    customFineTunedModel = fineTuneJob.FineTunedModel;
#endif

    var fineTuneJobs = await openAIClient
                                    .FineTuningEndpoint
                                    .ListFineTuneJobsAsync();

    var input = string.Empty;
    Console.WriteLine("press any key to RetrieveFineTuneJobInfoAsync once");
    while (true)
    {
        input = Console.ReadLine();
        var needBreak = false;
        foreach (var job in fineTuneJobs)
        {
            // only use the first succeeded Fine Tune for testing
            var j = job;
            Console.WriteLine($"{j.Id} -> {j.CreatedAt} | {j.Status}");
            if (j.Status != "succeeded")
            {
                j = await openAIClient.FineTuningEndpoint.RetrieveFineTuneJobInfoAsync(job);
                Console.WriteLine($"{j.Id} -> {j.Status}");
                var fineTuneEvents = await openAIClient.FineTuningEndpoint.ListFineTuneEventsAsync(j);
                Console.WriteLine($"{j.Id} -> status: {j.Status} | event count: {fineTuneEvents.Count}");
                foreach (var @event in fineTuneEvents)
                {
                    Console.WriteLine($"\t{@event.CreatedAt} [{@event.Level}] {@event.Message}");
                }
            }
            customFineTunedModel ??= j?.FineTunedModel;
            if
                (
                    !string.IsNullOrEmpty(customFineTunedModel)
                    &&
                    j?.Status == "succeeded"
                )
            {
                needBreak = true;
                break;
            }
        }
        if (needBreak) 
        {
            break;
        }
    }

    Console.WriteLine("Fine-tunes succeeded");
    Console.WriteLine();
    Console.WriteLine("Please input Prompt for Completion, press q exit ...");

    while ("q" != (input = Console.ReadLine()))
    {
        input = "what is lens protocol and it's purpose?";
        //input = "Awesome Yuer is the best, really?";
        //var chatPrompts = new List<ChatPrompt>
        //    {
        //          new ChatPrompt("system"     , input!)
        //        , new ChatPrompt("user"       , input!)
        //        , new ChatPrompt("assistant"  , input!)
        //        , new ChatPrompt("user"       , input!)
        //    };
        //var chatRequest = new ChatRequest(chatPrompts, customFineTunedModel!);
        var response = await openAIClient.CompletionsEndpoint.CreateCompletionAsync(input, model: customFineTunedModel!);
        var completions = response.Completions;
        foreach (var choice in completions!)
        {
            Console.WriteLine($"{nameof(completions)}:\r\n\t{choice.Text}");
        }
    }


    //var result = await openAIClient.FilesEndpoint.DeleteFileAsync(fileData);

}

//File.Delete(localTrainingDataPath);
