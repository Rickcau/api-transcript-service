using Azure.Storage.Blobs;


namespace api_transcript_service.Util;
public class BlobHelper
{
    public string? ConnectionString { get; set; }
    public string? Container { get; set; }

    public async Task<bool> WriteToBlobAsync(Stream fileStream,string blobName)
    {
        // TBD: Try Catch 
        var blobServiceClient = new BlobServiceClient(ConnectionString); 
        var containerClient = blobServiceClient.GetBlobContainerClient(Container);  
        await containerClient.CreateIfNotExistsAsync();
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(fileStream, true);
        return true;

    }

    public async Task<string> GetCondensedTranscriptFromBlob(string fileName)
    {
        // TBD: Try Catch 
        var blobServiceClient = new BlobServiceClient(ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(Container);
        var blobClient = containerClient.GetBlobClient(fileName);
        // check if blob exists
        if (await blobClient.ExistsAsync())
        {
            // Download the blob content to a memory stream
            MemoryStream stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);

            // Convert the stream to a string
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string transcript = reader.ReadToEnd();

            // Close the stream and the reader
            stream.Close();
            reader.Close();

            // Return the transcript string
            return transcript;
        }
        else
        {
            // Return an error message if the blob does not exist
            return "The file " + fileName + " does not exist in the blob container.";
        }
    }

    public async Task<string> GetContentFromBlob(string fileName)
    {
        // TBD: Try Catch 
        var blobServiceClient = new BlobServiceClient(ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(Container);
        var blobClient = containerClient.GetBlobClient(fileName);
        // check if blob exists
        if (await blobClient.ExistsAsync())
        {
            // Download the blob content to a memory stream
            MemoryStream stream = new MemoryStream();
            await blobClient.DownloadToAsync(stream);

            // Convert the stream to a string
            stream.Position = 0;
            StreamReader reader = new StreamReader(stream);
            string content = reader.ReadToEnd();

            // Close the stream and the reader
            stream.Close();
            reader.Close();

            // Return the transcript string
            return content;
        }
        else
        {
            // Return an error message if the blob does not exist
            return "Error: The file " + fileName + " does not exist in the blob container.";
        }
    }
}