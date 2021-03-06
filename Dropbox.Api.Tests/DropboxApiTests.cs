//-----------------------------------------------------------------------------
// <copyright file="DropboxApiTests.cs" company="Dropbox Inc">
//  Copyright (c) Dropbox Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Dropbox.Api.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Dropbox.Api.Auth;

    /// <summary>
    /// The test class for Dropbox API.
    /// </summary>
    [TestClass]
    public class DropboxApiTests
    {
        /// <summary>
        /// The Dropbox client.
        /// </summary>
        public static DropboxClient Client;

        /// <summary>
        /// The Dropbox team client.
        /// </summary>
        public static DropboxTeamClient TeamClient;

        /// <summary>
        /// The Dropbox app client.
        /// </summary>
        public static DropboxAppClient AppClient;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var userToken = context.Properties["userAccessToken"].ToString();
            Client = new DropboxClient(userToken);

            var teamToken = context.Properties["teamAccessToken"].ToString();
            TeamClient = new DropboxTeamClient(teamToken);

            var appKey = context.Properties["appKey"].ToString();
            var appSecret = context.Properties["appSecret"].ToString();

            AppClient = new DropboxAppClient(appKey, appSecret);
        }


        [TestCleanup]
        public void Cleanup()
        {
            var result = Client.Files.ListFolderAsync("").Result;

            foreach (var entry in result.Entries) {
                Client.Files.DeleteAsync(entry.PathLower).Wait();
            }
        }

        /// <summary>
        /// Test get metadata.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestGetMetadata()
        {
            await Client.Files.UploadAsync("/Foo.txt", body: GetStream("abc"));
            var metadata = await Client.Files.GetMetadataAsync("/Foo.txt");
            Assert.AreEqual("Foo.txt", metadata.Name);
            Assert.AreEqual("/foo.txt", metadata.PathLower);
            Assert.AreEqual("/Foo.txt", metadata.PathDisplay);
            Assert.IsTrue(metadata.IsFile);

            var file = metadata.AsFile;
            Assert.AreEqual(3, (int)file.Size);
        }

        /// <summary>
        /// Test get metadata.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestListFolder()
        {
            var files = new HashSet<string> { "/a.txt", "/b.txt", "/c.txt" };
            foreach (var file in files)
            {
                await Client.Files.UploadAsync(file, body: GetStream("abc"));
            }

            var response = await Client.Files.ListFolderAsync("");
            Assert.AreEqual(files.Count, response.Entries.Count);
            foreach (var entry in response.Entries)
            {
                Assert.IsTrue(files.Contains(entry.PathLower));
                Assert.IsTrue(entry.IsFile);
                var file = entry.AsFile;
                Assert.AreEqual(3, (int)file.Size);
            }
        }

        /// <summary>
        /// Test upload.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestUpload()
        {
            var response = await Client.Files.UploadAsync("/Foo.txt", body: GetStream("abc"));
            Assert.AreEqual(response.Name, "Foo.txt");
            Assert.AreEqual(response.PathLower, "/foo.txt");
            Assert.AreEqual(response.PathDisplay, "/Foo.txt");
            var downloadResponse = await Client.Files.DownloadAsync("/Foo.txt");
            var content = await downloadResponse.GetContentAsStringAsync();
            Assert.AreEqual("abc", content);
        }

        /// <summary>
        /// Test upload.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestDownload()
        {
            await Client.Files.UploadAsync("/Foo.txt", body: GetStream("abc"));
            var downloadResponse = await Client.Files.DownloadAsync("/Foo.txt");
            var content = await downloadResponse.GetContentAsStringAsync();
            Assert.AreEqual("abc", content);
            var response = downloadResponse.Response;
            Assert.AreEqual(response.Name, "Foo.txt");
            Assert.AreEqual(response.PathLower, "/foo.txt");
            Assert.AreEqual(response.PathDisplay, "/Foo.txt");
        }

        /// Test rate limit error handling.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestRateLimit()
        {
            var body = "{\"error_summary\": \"too_many_requests/..\", \"error\": {\"reason\": {\".tag\": \"too_many_requests\"}, \"retry_after\": 100}}";
            var mockResponse = new HttpResponseMessage((HttpStatusCode)429)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            mockResponse.Headers.Add("X-Dropbox-Request-Id", "123");

            var mockHandler = new MockHttpMessageHandler(mockResponse);
            var mockClient = new HttpClient(mockHandler);
            var client = new DropboxClient("dummy", new DropboxClientConfig { HttpClient = mockClient });
            try
            {
                await client.Files.GetMetadataAsync("/a.txt");
            }
            catch (RateLimitException ex)
            {
                Assert.AreEqual((int)ex.ErrorResponse.RetryAfter, 100);
                Assert.AreEqual(ex.RetryAfter, 100);
                Assert.IsTrue(ex.ErrorResponse.Reason.IsTooManyRequests);
            }
        }

        /// Test request id handling.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestRequestId()
        {
            var funcs = new List<Func<Task>>
            {
                () => Client.Files.GetMetadataAsync("/noob"), // 409
                () => Client.Files.GetMetadataAsync("/"), // 400
            };

            foreach (var func in funcs)
            {
                try
                {
                    await func();
                }
                catch (DropboxException ex)
                {
                    Assert.IsTrue(ex.ToString().Contains("Request Id"));
                }
            }
        }

        /// Test team auth.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestTeamAuth()
        {
            var result = await TeamClient.Team.GetInfoAsync();
            Assert.IsNotNull(result.TeamId);
            Assert.IsNotNull(result.Name);
        }

        /// Test team auth select user.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestTeamAuthSelectUser()
        {
            var result = await TeamClient.Team.MembersListAsync();
            var memberId = result.Members[0].Profile.TeamMemberId;

            var userClient = TeamClient.AsMember(memberId);
            var account = await userClient.Users.GetCurrentAccountAsync();
            Assert.AreEqual(account.TeamMemberId, memberId);
        }

        /// Test app auth.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestAppAuth()
        {
            try
            {
                var result = await AppClient.Auth.TokenFromOauth1Async("foo", "bar");
            }
            catch (ApiException<TokenFromOAuth1Error>)
            {
            }
        }

        /// Test no auth.
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        [TestMethod]
        public async Task TestNoAuth()
        {
            var result = await Client.Files.ListFolderAsync("", recursive: true);
            var cursor = result.Cursor;

            var task = Client.Files.ListFolderLongpollAsync(cursor);
            await Client.Files.UploadAsync("/foo.txt", body: GetStream("abc"));
            var response = await task;
            Assert.IsTrue(response.Changes);
        }

        /// Test APM flow.
        /// </summary>
        [TestMethod]
        public void TaskAPM()
        {
            var result = Client.Users.BeginGetCurrentAccount(null);
            var account = Client.Users.EndGetCurrentAccount(result);
            var accountId = account.AccountId;

            result = Client.Users.BeginGetAccountBatch(new string[] { accountId }, null);
            var accounts = Client.Users.EndGetAccountBatch(result);

            Assert.AreEqual(accounts.Count, 1);
            Assert.AreEqual(accounts[0].AccountId, accountId);
        }

        /// <summary>
        /// Converts string to a memory stream.
        /// </summary>
        /// <param name="content">The string content.</param>
        /// <returns>The memory stream.</returns>
        private static MemoryStream GetStream(string content) 
        {
            var buffer = Encoding.UTF8.GetBytes(content);
            return new MemoryStream(buffer);
        }
    }
}
