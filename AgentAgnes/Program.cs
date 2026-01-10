

using AgentAgnes;
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
    async private static Task Main(string[] args)
    {
        var processor = new AIChatProcessor(AgentSettings.AIEndpoint, AgentSettings.AIAccessKey, AgentSettings.AIDeploymentName)
        {
            MaxTokens = 800,
            Temperature = 0
        };
        await processor.InitializeAsync();

        while (true)
        {
            Console.Write("ai> ");
            var command = Console.ReadLine();

            var response = await processor.ProcessUserMessageAsync(command);
            if (response == "end")
            {
                Console.WriteLine("Thanks for chatting with me, goodbye!");
                break;
            }
            else
                Console.WriteLine(response.Deserialize<AgnesMessage>().Message);

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
        return Success("Donald J. Trump");
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