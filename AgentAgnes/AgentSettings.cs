using System;
using System.Collections.Generic;
using System.Text;

namespace AgentAgnes;

public  class AgentSettings
{
    public static string AIEndpoint { get; set; } = "https://thinkmine-aitests.openai.azure.com/";
    public static string AIAccessKey { get; set; } = "";
    public static string[] AIDeploymentName { get; set; } = ["gpt-5.2-chat","DeepSeek-V3.2","grok-3"];
}

public class AgnesMessage
{
    public string Message { get; set; } = "";
}
