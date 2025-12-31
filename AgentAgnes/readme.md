
You'll need Visual Studio Code
You'll need the latest version of .NET (.NET 10)
You'll need an Azure Subscription to work with

## Configuring your Agent
1. Create an account in Microsoft Foundry
    1. Go to Azure OpenAI
	1. Select AI Foundry and Create a new Foundry Service named ```Agnes-Agent```  Use all the default settings
	1. Select the newly created foundry service and click on Go to Foundry Project 
	1. Go to the model catalog and select gpt 4.2 (there are multiple models to choose from, 
	and a way to compare models based on the index value of each model)
	1. Click _Use this Model_ to begin the process of creating a model deployment for that
	particular model.
	1. Enter the name you would like the deployment to be called (using ```agnes-gpt-4.1```) and the kind of deployment to use
	Some deployment types are free and some cost more than others.  Be mindful of the number of tokens per minute
	and other factors.
	1. With the model deployed we can open it up in the playground and test it out quickly
	1. In the playgroung Setup section, Add "the price of tea in london in $10" then click _apply changes_
	1. Now in the chat section ask questions about tea.

	You've just completed the first part.  We now have a deployed model that we can use in our
applications.  If you click on the _view code_ button you will see a sample of how to 
leverage this code in your application.  You have choice here to do this in many languages.  For this
HOL we will be using c#, 
1. Click on the drop down and select c# to see how this code looks when using the 
standard c# SDK.  IT should be about 40 lines of code to get yourself going.  Let's get into
the code aspect of it now and see if we can do better.

1. Create a Console Application (create a new console application in visual studion code)
1. use ```dotnet add package Zaria.Ai``` to add a package reference to the Zaria AI platform we will be using
1. Create a cs class for handling project settings
1. Back in your foundy project, copy the API key into a public static property in your handler settings 
class called ```api_key```
1. Do the same for the deployment name.

## Using your agent from Code

1. Zaria uses the openai sdk internally, so in your FOundy project you'll have to get the 
endpoint for that. You can do this by selecting Azure OpenAI from the _Libraries_ section of 
your Foundy project landing page.  It should be ```https://agnes-foundry.openai.azure.com/```.  Put that into a public static property in your setting class as well.
1. Back in your program.cs add a reference to Zaria.AI by adding the following using statement:
```using Zaria.AI.Chat```
1. Next specify to the framework that this application will function as an AI plugin by adding the following:

```
[assembly: AIPluginAttribute(
    "Sample AI Agent",
    """
    You are a helpfull AI agent named agnes that monitors 
    communication in your organization so the workers don't have to.
    You know the price of tea in London is $10
    """)]
```

You can now create the code to mimick the behavior in the playground:
```
var processor = new AIChatProcessor(AgentSettings.ai_endpoint, AgentSettings.ai_access_key, AgentSettings.ai_deployment_name)
        {
            MaxTokens = 800,
            Temperature = 0
        };
        await processor.InitializeAsync();

        var response = await processor.ProcessUserMessageAsync("What the price of tea?");
        Console.WriteLine(response);
```
We are now getting the response we want back but it is coming as json.  Lets convert it into
a static type so we can work with the response messages easily (and generate more human readable responses)


## Incorporating Tool Calls
Before we go any further, lets update this to be a little more dynamic so we dont have to 
keep running the application.  We'll implement a tiny REPL loop so we can call our agent
multiple times.

Calling our AI to simply read back information we already have is pretty straighforward, but what happens
if we need to call it to get information we dont already have?

_what time is it?_

Ask agnes what time it is at the moment and you start to see the problem.  Additionally we can see
what information agnes has based on when the model was created.  It wont know anything further in the future than thatn

_when were you trained_

_who is the president of the United States_

To get access to this realtime information like the time, or pull more modern information
out of the AI we have to leverage tool calls.  First we'll create a tool call to help simplify
closing our application

To leverage tool calls you simply have to create a class that overrides AIAgent
```
public class SampleAgent : AIAgent
```

In this class any public functions that return ```SkillResponse``` and are decorated with
a ```Skill``` attribute will be registered with the AI.  The Skill attribute allows the
AI to know under what conditions the call must be made.  In this case we are telling the AI
to call this method whenever the user passes a message that it interprets as wanting to close
the application.  

```
    [Skill("Call when you need to end the application")]
    public SkillResponse EndApplication()
    {
        return Success("end", false);
    }
```
Here is how the code in Program.cs can now be changed to close the application.

```
    var response = await processor.ProcessUserMessageAsync(command);
    if (response == "end")
    {
        Console.WriteLine("Thanks for chatting with me, goodbye!");
        break;
    }
    else
        Console.WriteLine(response.Deserialize<AgnesResponse>().Message);
```

SkillResponses can be fed back into the AI fro further processing or return immediately.  In the
previous sample we explicitly state that we want to return immediately, this is because
the response is a command and we want to interpret it explicitly.  We can add additional
skills to our agent that do not return immediately.  

```
    [Skill("Call when you need to know the current date")]
    public SkillResponse GetDateTime()
    {
        var date = DateTime.Now;
        return Success($"{date}");
    }

    [Skill("Call when you need to know the president of the United States")]
    public SkillResponse GetPresident()
    {
        var date = DateTime.Now;
        return Success($"Donald Trump");
    }
```
