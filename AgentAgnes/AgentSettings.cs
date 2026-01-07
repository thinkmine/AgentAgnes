using System;
using System.Collections.Generic;
using System.Text;

namespace AgentAgnes;

public  class AgentSettings
{
    //public static string  ai_endpoint = "https://thinkmine-ai-demos.openai.azure.com/";
    //public static string ai_access_key = "a57bf876732a4da7a0870c84373848f2";
    //public static string ai_deployment_name = "gpt-4o";

    
    public static string AIEndpoint { get; } = "https://thinkmine-aitests.openai.azure.com";
    public static string AIAccessKey { get;  } = "EUda8yFXDMBzFABbKZow34FqmHMmf5jnjurxj603MIrv5HWiJPgQJQQJ99CAACHYHv6XJ3w3AAAAACOG1Gdv";
    public static string[] AIDeploymentName { get; } = ["gpt-5.2-chat", "DeepSeek-V3.2", "grok-3"];
}

public class AgnesMessage
{
    public string Message { get; set; }
}
