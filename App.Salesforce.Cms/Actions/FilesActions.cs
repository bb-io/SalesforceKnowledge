using RestSharp;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common;
using App.Salesforce.Cms.Models.Responses;
using App.Salesforce.Cms.Models.Requests;
using App.Salesforce.Cms.Dtos;
using Apps.Salesforce.Cms.Dtos;

namespace Apps.Salesforce.Cms.Actions
{
    [ActionList]
    public class FilesActions
    {
        [Action("List all files", Description = "List all files")]
        public ListAllFilesResponse ListAllFiles(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var query = "SELECT FIELDS(ALL) FROM ContentDocument LIMIT 200";
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/query?q={query}", Method.Get, authenticationCredentialsProviders);
            return client.Get<ListAllFilesResponse>(request);
        }

        [Action("Get file info", Description = "Get file info by id")]
        public FileInfoDto GetFileInfo(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] GetFileInfoRequest input)
        {
            var query = $"SELECT FIELDS(ALL) FROM ContentDocument WHERE Id = '{input.FileId}'";
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/query?q={query}", Method.Get, authenticationCredentialsProviders);
            return client.Get<ListAllFilesResponse>(request).FileInfos.FirstOrDefault();
        }

        [Action("Download file", Description = "Download file by id")]
        public DownloadFileResponse DownloadFile(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] DownloadFileRequest input)
        {
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var fileInfo = GetFileInfo(authenticationCredentialsProviders, new GetFileInfoRequest() { FileId = input.FileId });

            var contentVersionRequest = new SalesforceRequest($"services/data/v57.0/query?q=SELECT FIELDS(ALL) FROM ContentVersion WHERE Id = '{fileInfo.LatestPublishedVersionId}'", Method.Get, authenticationCredentialsProviders);
            var contentVersion = client.Get<RecordsCollectionDto<ContentVersionDto>>(contentVersionRequest).Records.FirstOrDefault();

            var request = new SalesforceRequest($"services/data/v57.0/sobjects/ContentVersion/{fileInfo.LatestPublishedVersionId}/VersionData", Method.Get, authenticationCredentialsProviders);
            return new DownloadFileResponse() {
                File = client.Get(request).RawBytes,
                Filename = contentVersion.PathOnClient
            };
        }

        [Action("Upload file", Description = "Upload file")]
        public RecordIdDto UploadFile(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] UploadFileRequest input)
        {
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/sobjects/ContentVersion", Method.Post, authenticationCredentialsProviders);
            request.AddJsonBody(new
            {
                Title = input.Title,
                PathOnClient = input.Filename,
                VersionData = Convert.ToBase64String(input.File)
            });
            return client.Execute<RecordIdDto>(request).Data;
        }

        [Action("Delete file", Description = "Delete file")]
        public void DeleteFile(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            [ActionParameter] GetFileInfoRequest input)
        {
            var client = new SalesforceClient(authenticationCredentialsProviders);
            var request = new SalesforceRequest($"services/data/v57.0/sobjects/ContentDocument/{input.FileId}", Method.Delete, authenticationCredentialsProviders);
            client.Execute(request);
        }
    }
}

