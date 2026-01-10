

using AgentAgnes;
using OpenAI.Images;
using System.Net;
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
    async private static Task Main(string[] args)
    {
        int model_index = 0;
        var processor = new AIChatProcessor(AgentSettings.AIEndpoint, AgentSettings.AIAccessKey, AgentSettings.AIDeploymentName[model_index]);
        await processor.InitializeAsync();

        while (true)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            WriteYellow($"{AgentSettings.AIDeploymentName[model_index]} ");
            
            WriteGray(" > ");
            Write(" ");

            var command = Console.ReadLine();

            if (command == "change")
            {
                model_index++;
                if (model_index > 2)
                    model_index = 0;

                processor = new AIChatProcessor(AgentSettings.AIEndpoint, AgentSettings.AIAccessKey, AgentSettings.AIDeploymentName[model_index]);
                await processor.InitializeAsync();
                continue;
            }

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