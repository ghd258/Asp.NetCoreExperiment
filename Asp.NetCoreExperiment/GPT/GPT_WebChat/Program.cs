using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI.ChatCompletion;
using System.Collections.Generic;
using System.Text;
var key = File.ReadAllText(@"C:\\GPT\key.txt");
var builder = WebApplication.CreateBuilder(args);
var kernel = Kernel.Builder
    .Configure(c =>
    {
        c.AddOpenAIChatCompletionService("gpt-4", key, serviceId: "davinci-openai");
    })
    .Build();
var chatGPT = kernel.GetService<IChatCompletion>();
var chatHistory = (OpenAIChatHistory)chatGPT.CreateNewChat();
builder.Services.AddSingleton(chatGPT);
builder.Services.AddSingleton(chatHistory);
var app = builder.Build();
app.UseStaticFiles();
app.MapGet("/chat", AskAsync);
app.Run();
async IAsyncEnumerable<string> AskAsync(IChatCompletion chat, OpenAIChatHistory history, string ask)
{
    history.AddUserMessage(ask);
    var reply = chat.GenerateMessageStreamAsync(history, new ChatRequestSettings() { MaxTokens = 2048 });
    var answer = new StringBuilder();
    await foreach (var item in reply)
    { 
        if(item==null)
        {
            continue;
        }
        answer.Append(item);
        yield return item;
    }
    history.AddAssistantMessage(answer.ToString());
}