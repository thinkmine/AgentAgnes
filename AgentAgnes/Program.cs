

using AgentAgnes;
using OpenAI.Images;
using System.Net;
using System.Net.Http.Headers;
using Zaria.AI;
using Zaria.AI.Chat;
using Zaria.Core;

[assembly: AIPluginAttribute(
    "File Processing Plugin",
    """
    You are a helpfull AI agent named agnes that monitors communication in your organization so the workers don't have to.
    """)]


internal class Program
{
    #region Helpers
    static void Write(string message)
    {
        Console.ResetColor();
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(message);
        Console.ResetColor();
    }

    static void WriteYellow(string message)
    {
        Console.BackgroundColor = ConsoleColor.Yellow;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(message);
        Console.ResetColor();
    }
    static void WriteGray(string message)
    {
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.ForegroundColor = ConsoleColor.Black;
        Console.Write(message);
        Console.ResetColor();
    }

    static void WriteRed(string message)
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(message);
        Console.ResetColor();
    }

    static string PromptSecret(string message)
    {
        WriteRed($"{message} ");
        WriteGray(" : ");
        Write(" ");
        var response = Console.ReadLine() ?? "";
        return response;
    }

    static string Prompt(string message)
    {
        WriteYellow($"{message} ");
        WriteGray(" > ");
        Write(" ");
        var response = Console.ReadLine() ?? "";
        return response;
    }

    #endregion

    async private static Task Main(string[] args)
    {

        int model_index = 0;
        AIChatProcessor? processor = null;

        while (true)
        {
            if (AgentSettings.AIAccessKey.IsNull)
            {
                try
                {
                    AgentSettings.AIAccessKey = PromptSecret("Enter the secret");
                    processor = new AIChatProcessor(AgentSettings.AIEndpoint, AgentSettings.AIAccessKey, AgentSettings.AIDeploymentName[model_index]);
                    await processor.InitializeAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not connect to AI environment");
                    break;
                }
            }
            else
            {
                processor = new AIChatProcessor(AgentSettings.AIEndpoint, AgentSettings.AIAccessKey, AgentSettings.AIDeploymentName[model_index]);
                await processor.InitializeAsync();
            }

            var command = Prompt($"{AgentSettings.AIDeploymentName[model_index]} ");

            if (command == "change")
            {
                model_index++;
                if (model_index > 2)
                    model_index = 0;

                processor = new AIChatProcessor(AgentSettings.AIEndpoint, AgentSettings.AIAccessKey, AgentSettings.AIDeploymentName[model_index]);
                await processor.InitializeAsync();
                continue;
            }
            else
            {

                var start = DateTime.Now;
                var response = await processor.ProcessUserMessageAsync(command);
                if (response == "end")
                {
                    Console.WriteLine("Thanks for chatting with me, goodbye!");
                    break;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine(response.Deserialize<AgnesMessage>().Message);
                    Console.WriteLine($"(This response tool {(DateTime.Now - start).TotalSeconds.Round(1)} seconds)");
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }

        }

    }
}

public class SampleAgent : AIAgent
{


    [Skill("Call when you need to end the application")]
    public SkillResponse EndApplication()
    {
        return Success("end", false);
    }

    [Skill("Call when you need to know the president of the United States")]
    public SkillResponse GetPresident()
    {
        return Success("Kamala Harris");
    }

    [Skill("Call when you need to know the current date or time")]
    public SkillResponse GetDateTime()
    {
        return Success($"{DateTime.Now}");
    }

    [Skill("Call whenever you need to get the count of messages that have been received")]
    public SkillResponse CountMessages(
           [Parameter("Represents the time period to search")]
            string time_period
       )
    {

        var sandbox_path = Path.Combine(Environment.CurrentDirectory, "wwwroot/aisandbox");
        var summary_files = Directory.GetFiles(sandbox_path);
        var summary_count = summary_files.Count();
        return Success(summary_count);

    }

}