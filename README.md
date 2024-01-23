# Transcript Summary API - Blob Trigger
In this example, I demonstrate how to leverage an Isolated Azure Function Blob Trigger that parses a Teams Meeting Transcript into a condensed format that is then used by AI to summarize the condensed transcript.  

## Technologies used

1. Isolated Azure Functions with a Blob Trigger
2. Azure BlobServiceClient for writing to containers
3. Semantic Kernel for AI Summarization purposes using Dependency Injection for the Kernel
4. StreamReader and Regex for file parsing
5. Environment.GetEnvironmentVariable to read settings from **local.settings.json** and Azure Configuration Settings
6. Custom TranscriptService to condense the transcript down into a format that results in fewer tokens being used by AI

## Requirements for this example

1. You simply need to create 3 containers in a storage account; **input, output and summary**
 
2. Rename the **local.settings.json.bak** file to **local.settings.json**, then  set the following values in **local.settings.json** file

   ~~~
        "BlobConnection": "<Your connection string to the storage account>",
        "ApiDeploymentName": "<Your Deployment Name>",
        "ApiEndpoint": "<Your Endpoint>",
        "ApiKey": "<Your API Key"
   ~~~

   When deploying to Azure, you will need to make sure you create Configuration settings for BlobConnection, ApiDeploymentName, ApiEndPoint and ApiKey. When the funciton is running in Azure it readys the variables from Configuration settings and not the local.settings.json file.

3. The sample file found in the Data folder

## Important Notes

- The name of the input file is important as well and has the following format: **transcript#1baa3e6e-46d7-4039-9581-871766800237#someemail@test.com#.txt**.  

  ~~~
       **transcript** followed a # a **GUID** followed by a # then an email address, followed by a #, then the extension .TXT.  
  ~~~

- The client that is pushing the file to an Azure Blob continer will need to generate a GUID and ensure the filename matches this format.
- This format is using by the processing logic to extract the GUID and email address of the submitter and is used when writting new data to the output and summary containers
- This format also allows the data to be written to various backing sources i.e. CosmosDB, SQL or other systems for history purposes

## How to use this example?
1. You can clone the repo and run it locally using Visual Studio.

2. Open the solution and press F5 to run it locally.

3. Navigate to the Azure Portal and drop/copy the **transcript#1baa3e6e-46d7-4039-9581-871766800237#someemail@test.com#.txt** file found in the Data folder to the Azure **input** container you created in the step above.  If you set a breakpoint on the Blob Trigger, it will get hit once the file has been copied in the **input** container.  You can step through the logic and watch the code condense the file and copy the condensed version to the output folder.

4. Now, navigate to the **output** container in the Azure portal and make note of the name of the file, it should be **ReformattedTranscript#1baa3e6e-46d7-4039-9581-871766800237#someemail@test.com#.txt** you will need it for the next step. If you navigate to the **summary** folder you will also find the AI summarized version of the transcript.

## How can this be leveraged to summarize Meeting Transcripts?
- Create a Canvas App that allows users to upload transcript to the input container using the file name formated outlined above.  The Blob Trigger would be triggered and the summary would be written to the summary folder.  The Canvas App could then retreive the **summary** from the **summary** folder.
- Create a Web App that uploads the transcript and reads the summary from the **summary** folder
- Basically, any client that can upload a file to Azure Storage can leverage this logic

## Additional features that can be added to the solution

1. Expose an HTTP Trigger that allows the client to pass a GUID that is used to retreive summaries from the **summary** container.

2. Add another Blob Trigger that allows for Document translation services i.e. convert documents written in English into other languages.  In other words you can convert the meeting summary into other languages.  



