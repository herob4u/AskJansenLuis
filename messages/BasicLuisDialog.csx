using System;
using System.Threading.Tasks;
using System.Net;

using Newtonsoft.Json;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

// For more information about this template visit http://aka.ms/azurebots-csharp-luis
[Serializable]
public class BasicLuisDialog : LuisDialog<object>
{
    // Caches the user string response to a prompt dialog
    private string stringResult;
    private string answer = string.Empty;
    Dictionary<string,string> promptQs = new Dictionary<string,string>() {
            {"Methods Available","What methods are available to search for content"},
            {"Wildcard Available","What types of things can I search for"},
            {"Search all documents","What wildcards can be used in searches"},
            {"Info available to search for","What is the difference between searching for 'Authoring Documents' versus 'Published Documents'"},
            {"'Current' documents vs other options","How do I just search for all documents"},
            {"'Authoring Documents' vs 'Published Documents'","What is the difference between searching for 'Current' documents versus other options"}};
    
    public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(Utils.GetAppSetting("LuisAppId"), Utils.GetAppSetting("LuisAPIKey"))))
    {
    }


    /// <summary>
    ///  Handles all generic definition inquiries.
    ///  i.e "What is ___?". Uses the Entity name as an object of inquiry.
    /// </summary>
    [LuisIntent("Define")]
    public async Task Define(IDialogContext context, LuisResult result)
    {  
        await context.PostAsync("Define Intent:");
        
        if(result.Entities != null && result.Entities.Count <= 0)
        {
            await context.PostAsync("No Entities Found. Use Utterance: " + result.Query);
            answer = GetQnAResponse(result.Query);
        }
        else
        {
            answer = GetQnAResponse($"what is {result.Entities[0].Entity}");
        }
        
        await context.PostAsync(answer);
        context.Wait(MessageReceived);
    }
  
    
    [LuisIntent("Need.Use")]
    public async Task NeedUseIntent(IDialogContext context, LuisResult result)
    {

        if(result.Entities == null || result.Entities.Count == 0)
        {
            answer = GetQnAResponse(result.Query);
        }
        else
        {
            answer = GetQnAResponse($"Do we still need to use {result.Entities[0].Entity}");
        }
        await context.PostAsync(answer);

        context.Wait(MessageReceived);
    }
  
    [LuisIntent("How.Set")]
    public async Task HowToSet(IDialogContext context, LuisResult result)
    {
        if (result.Entities == null || result.Entities.Count == 0)
        {
            answer = GetQnAResponse(result.Query);
        }
        else
        {
            foreach(EntityRecommendation entity in result.Entities)
            {
                if(entity.Entity == "Look Ahead")
                {
                    answer = GetQnAResponse($"What is {entity.Entity} and when might I want to turn it off?");
                }
                else
                {
                    answer = GetQnAResponse($"How do I set {entity.Entity}");
                }
            }
        }
        await context.PostAsync(answer);

        context.Wait(MessageReceived);
    }
    
    [LuisIntent("How.Access")]
    public async Task HowToAccess(IDialogContext context, LuisResult result)
    {
        if (result.Entities == null || result.Entities.Count == 0)
        {
            answer = GetQnAResponse(result.Query);
        }
        else
        {
            await context.PostAsync(result.Entities[0].ToString());
            answer = GetQnAResponse($"How do I access {result.Entities[0].Entity}");
        }
        await context.PostAsync(answer);

        context.Wait(MessageReceived);
    }
    
    [LuisIntent("How.Install")]
    public async Task HowToInstall(IDialogContext context, LuisResult result)
    {
        if (result.Entities == null || result.Entities.Count == 0)
        {
            answer = GetQnAResponse(result.Query);
        }
        else
        {
            answer = GetQnAResponse($"How do I get {result.Entities[0].Entity} installed on my computer");
        }
        await context.PostAsync(answer);

        context.Wait(MessageReceived);
    }
    
    [LuisIntent("How.Login")]
    public async Task HowToLogin(IDialogContext context, LuisResult result)
    {
        if (result.Entities == null || result.Entities.Count == 0)
        {
            answer = GetQnAResponse(result.Query);
        }
        else
        {
            answer = GetQnAResponse($"How do I log in to {result.Entities[0].Entity}");
        }
        await context.PostAsync(answer);

        context.Wait(MessageReceived);
    }
    
    [LuisIntent("How.DocumentInfo")]
    public async Task DocumentInfo(IDialogContext context, LuisResult result)
    {
        if (result.Entities == null || result.Entities.Count == 0)
        {
            answer = GetQnAResponse(result.Query);
        }
        else
        {
            foreach (EntityRecommendation entity in result.Entities)
            {
                if (entity.Entity == "Icon Color")
                {
                    answer = GetQnAResponse($"Why are some document icons different {entity.Entity}? What do they mean?");
                }
                else if (entity.Entity == "Relationship")
                {
                    answer = GetQnAResponse($"How can I see all the {entity.Entity} a document has with other objects?");
                }
                else
                {
                    answer = GetQnAResponse($"How do I view a document's {entity.Entity}");
                }
            }
        }
        await context.PostAsync(answer);

        context.Wait(MessageReceived);
    }
    
    [LuisIntent("How.Modify")]
    public async Task HowToModify(IDialogContext context, LuisResult result)
    {
        if (result.Entities == null || result.Entities.Count <= 0 )
        {
            answer = GetQnAResponse(result.Query);
        }
        else
        {   
            string message = $"How do I {result.Entities[0].Entity}";
            if(result.Entities.Count > 1)
            {
                for(int i = 1 ; i < result.Entities.Count; i++)
                {
                    message = string.Join(" ", message, result.Entities[i].Entity);
                }
            }
            answer = GetQnAResponse(message);
        }
        await context.PostAsync(answer);

        context.Wait(MessageReceived);
    }
    
    [LuisIntent("How.Use")]
    public async Task HowToUse(IDialogContext context, LuisResult result)
    {
        if (result.Entities == null || result.Entities.Count == 0)
        {
            answer = GetQnAResponse(result.Query);
        }
        else
        {
            answer = GetQnAResponse($"How do I use {result.Entities[0].Entity}");
        }
        await context.PostAsync(answer);

        context.Wait(MessageReceived);
    }
    
    [LuisIntent("About.Search")]
    public async Task AboutSearch(IDialogContext context, LuisResult result)
    {
        PromptDialog.Choice<string>(context, SearchSelectedAsync, promptQs.Keys, "What would you like to know about the search feature?");
    }
    
    [LuisIntent("How.Document.See")]
    private async Task HowDocumentSee(IDialogContext context, LuisResult result)
    {
        if (result.Entities == null || result.Entities.Count == 0)
        {
            answer = GetQnAResponse(result.Query);
        }
        else
        {
            string msg = "How do I";
            foreach (EntityRecommendation entities in result.Entities)
            {
                msg = string.Join(" ", msg, entities.Entity);
            }
            
            answer  = GetQnAResponse(msg);
            await context.PostAsync(answer);

            context.Wait(MessageReceived);
        }
    }
    
    [LuisIntent("How.Document.Action")]
    private async Task HowDocumentAction(IDialogContext context, LuisResult result)
    {
        if (result.Entities == null || result.Entities.Count == 0)
        {
            answer = GetQnAResponse(result.Query);
        }
        else
        {
            string msg = "How do I";
            
            foreach (EntityRecommendation entities in result.Entities)
            {
                await context.PostAsync(entities.Type);
                msg = string.Join(" ", msg, entities.Entity);
            }
            await context.PostAsync(msg);
            answer  = GetQnAResponse(msg);
            await context.PostAsync(answer);

            context.Wait(MessageReceived);
        }
    }
    
    private async Task SearchSelectedAsync(IDialogContext context, IAwaitable<string> result)
    {
        await context.PostAsync(GetQnAResponse(promptQs[await result]));
        context.Wait(MessageReceived);
    }
    
    [LuisIntent("None")]
    public async Task NoneIntent(IDialogContext context, LuisResult result)
    {
        context.PostAsync(GetQnAResponse(result.Query));
        context.Wait(MessageReceived);
    }
    
    [LuisIntent("Greetings")]
    public async Task GreetingsIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync(GetQnAResponse("hi"));
        context.Wait(MessageReceived);
    }

    // Go to https://luis.ai and create a new intent, then train/publish your luis app.
    // Finally replace "MyIntent" with the name of your newly created intent in the following handler
    [LuisIntent("DefineOmar")]
    public async Task MyIntent(IDialogContext context, LuisResult result)
    {
        answer = GetQnAResponse("Do we still need to use iPas DM");
        await context.PostAsync(answer);
        //await context.PostAsync($"You have reached the MyIntent intent. You said: {result.Query}"); //
        context.Wait(MessageReceived);
    }

    [LuisIntent("Help")]
    public async Task HelpIntent(IDialogContext context, LuisResult result)
    {
        //context.Call(new HelpDialog(), ResumeAfterHelp);
        
    }

    
    private async Task ResumeAfterHelp(IDialogContext context, IAwaitable<object> result)
    {
        await context.PostAsync("Done HelpDialog");
        var message = await result;
        context.Wait(MessageReceived);
    }

    
    /// Handles acquiring string data from any
    /// string PromptDialog and setting the string result variable.
    private async Task GetStringFromPrompt(IDialogContext context, IAwaitable<string> result)
    {
        stringResult = await result;
        
        if(!string.IsNullOrEmpty(stringResult))
        {
            switch(stringResult)
            {
                case "Definitions":
                {await context.PostAsync(LuisData.HelpTopics[0]);}
                break;
                
                case "How To Use":
                {await context.PostAsync(LuisData.HelpTopics[1]);}
                break;
                
                case "How To Access":
                {await context.PostAsync(LuisData.HelpTopics[2]);}
                break;
                
                case "Login Information":
                {await context.PostAsync(LuisData.HelpTopics[3]);}
                break;
            }
        }
        
        context.Wait(MessageReceived);
    }
    
    /// Establishes an HTTP request to the QnA database
    /// and fetches an answer string based on the provided
    /// query. The code was supplied by Microsoft's online documentation:
    /// https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/httpendpoint
    private string GetQnAResponse(string _query)
    {
        string responseString = string.Empty;
    
        var query = _query; //User Query
        var knowledgebaseId = "c45530b5-3514-4e68-9d2e-e64fc5e44b50"; // Use knowledge base id created.
        var qnamakerSubscriptionKey = "0e175b6f52084371b63e81d00c65bc16"; //Use subscription key assigned to you.
        
        //Build the URI
        Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
        var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");
        
        //Add the question as part of the body
        var postBody = $"{{\"question\": \"{query}\"}}";
        
        //Send the POST request
        using (WebClient client = new WebClient())
        {
            //Set the encoding to UTF8
            client.Encoding = System.Text.Encoding.UTF8;
        
            //Add the subscription key header
            client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
            client.Headers.Add("Content-Type", "application/json");
            responseString = client.UploadString(builder.Uri, postBody);
        }
        
        // Parse the JSON response and return the answer string.
        QnAResult parsedResponse;
        
        try
        {
            parsedResponse = JsonConvert.DeserializeObject< QnAResult >(responseString);
        }
        catch
        {
            throw new Exception("Unable to deserialize QnA Maker response string.");
        }
        
        // Return the Answer string only.
        return (parsedResponse.Answer); 
    }
    
}


/// <summary>
/// A container for the JSON formatted
/// response acquired from the QnA database.
/// The services returns a JSON with the Answer text
/// and confidence score.
public class QnAResult
{
    /// <summary>
    /// The top answer found in the QnA Service.
    /// </summary>
    [JsonProperty(PropertyName = "answer")]
    public string Answer { get; set; }

    /// <summary>
    /// The score in range [0, 100] corresponding to the top answer found in the QnA    Service.
    /// </summary>
    [JsonProperty(PropertyName = "score")]
    public double Score { get; set; }
}

public class LuisData
{
    private enum DocumentSystems {iPas, Sharepoint, Aconex, SPF};
    private static List<string> _HelpTopics = new List<string>()
    {
        {"Definitions"},
        {"How To Use"},
        {"How To Access"},
        {"Login Information"}
    };
    
    public static List<string> HelpTopics {get {return _HelpTopics;} } 
}
