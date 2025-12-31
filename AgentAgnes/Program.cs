

using AgentAgnes;
using System.Net;
using Zaria.AI;
using Zaria.AI.Chat;

[assembly: AIPluginAttribute(
    "File Processing Plugin",
    """
    You are a helpfull AI agent named agnes that monitors communication in your organization so the workers don't have to.
    """)]


internal class Program
{
    async private static Task Main(string[] args)
    {
        var processor = new AIChatProcessor(AgentSettings.ai_endpoint, AgentSettings.ai_access_key, AgentSettings.ai_deployment_name)
        {
            MaxTokens = 800,
            Temperature = 0
        };
        await processor.InitializeAsync();

        var response = await processor.ProcessUserMessageAsync("What time is it");
        Console.WriteLine($"Your Answer is: {response}");
        Console.Read();
    }
}

public class SampleAgent : AIAgent
{
    [Skill("Call when you need to know the current date or time")]
    public SkillResponse GetDateTime()
    {
        var date = DateTime.Now;
        return Success($"{date}",false);
    }
}