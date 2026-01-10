using System;
using System.Collections.Generic;
using System.Text;

namespace AgentAgnes;

public  class AgentSettings
{
    public static string AIEndpoint { get; set; } = "https://thinkmine-ai-demos.openai.azure.com/";
    public static string AIAccessKey { get; set; } = "a57bf876732a4da7a0870c84373848f2";
    public static string AIDeploymentName { get; set; } = "gpt-4o2";
}

public class AgnesMessage
{
    public string Message { get; set; }
}
