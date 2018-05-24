using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Dotyk.Me.Client;
//using Dotyk.Store.Client;
using Dotyk.Store.Deployment.Packaging;
//using Dotyk.Store.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System.Net.Http;
using System.IO.Compression;

namespace Dotyk.Store.Cli
{
    class Utils
    {
        internal static Packager PreparePackager(string configuration, ILogger logger)
        {
            try
            {
                var be = new BuildEnvironment()
                {
                    NugetPaths = /*new List<string> */{ @"C:\nuget\nuget.exe", "nuget.exe" },
                    Configuration = configuration
                };
                return new Packager
                {
                    Packagers =
                {
                    new FolderPackager(logger),
                    new WinRTPackager(be, logger),
                    new Win32Packager(be, logger)
                }
                };
            }
            catch (Exception ex)
            {
                Debug.Write("Prepare Package error: " + ex.Message);
            }
            throw new NotImplementedException();

        }



        internal static async Task PushPackageAsync(PushOption options, ILogger logger, Stream packageStream)
        {
            packageStream.Seek(0, SeekOrigin.Begin);

            using (logger.BeginScope("Publishing"))
            {
                try
                {
                    var token = await GetTokenAsync(options, logger);

                    HttpClient client = new HttpClient();
                    
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.DotykMeToken);

                    using (var fileStream = new FileStream(options.PackagePath, FileMode.Open))
                    {
                        var streamContent = new StreamContent(fileStream);
                        var resoponse  = await client.PostAsync($"{options.ServerUrl}/api/Application/Publish", streamContent);
                        if (resoponse.IsSuccessStatusCode)
                        {

                        }
                    }
                   


                //    client.PostAsync("/api/Application/Publish", );



                    //    try
                    //    {
                    //            logger.LogInformation("Uploading package\r\n" +
                    //                "Package: {package}\r\n" +
                    //                "Feed: {feed}\r\n" +
                    //                "Server: {server}", options.PackagePath, options.Feed, options.ServerUrl);

                    //            await client.SubmitPackage(packageStream, options.Feed);

                    //            logger.LogInformation("Package published");
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            logger.LogError(new EventId(), ex, "Failed to publish package");
                    //            throw;
                    //        }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    throw;
                }
            }
        }

        //internal static StoreClient CreateStoreClient(ServerCommonOptions options, ILogger logger)
        //{
        //    try
        //    {
        //        var sc = new StoreClient(
        //               new StoreClientOptions
        //               {
        //                   ServerUrl = new Uri(options.ServerUrl)
        //               });
        //        return sc;
        //    }
        //    catch (Exception ex)
        //    { 
        //        throw ex;
        //    }
        //}

        internal static async Task<string> GetTokenAsync(AuthCommonOptions options, ILogger logger)
        {
            var dotykMe = new DotykClient(
                new FileCertificateStorage(
                    Path.Combine(Environment.GetEnvironmentVariable("temp"), "Kodisoft", "DotykMe", "Certificates")));

            var token = GetStoredToken(dotykMe, logger);

            if (token == null)
            {
                logger?.LogTrace("Token not found in registry. Requesting new token.");

                for (int i = 0; i < 10; i++)
                {
                    string login,
                    password;
                    Dotyk.Extension.Windows.AuthForm authForm = new Dotyk.Extension.Windows.AuthForm();
                    if (authForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        login = authForm.textBox1.Text;
                        password = authForm.textBox2.Text;
                    }
                    else
                    {
                        throw new Exception("login and Password are required");
                    }

                    try
                    {
                        logger?.LogTrace("Credentials read. Logging in");
                        await dotykMe.Login(new Me.Model.LoginModel { Email = login, Password = password });

                        logger?.LogTrace("Logged in to dotyk.me. Requesting token");
                        token = (await dotykMe.GetRestrictedToken(Me.Model.TokenDuration.Long)).Token;

                        logger?.LogTrace("Caching token");
                        Registry.SetValue(@"HKEY_CURRENT_USER\Software\Kodisoft\Dotyk\Store\DeploymentCLI", "AuthToken", token);

                        var valid = await dotykMe.ValidateTokenAsync(token);

                        logger?.LogInformation("Successfully logged in as {user}, token valid to {validTo}", valid.UserName, valid.ValidTo);
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger?.LogError(ex.Message, "Failed to login");
                    }
                }
            }

            return token;
        }

        //internal static async Task<StoreClient> CreateAndAuthenticateStoreClientAsync(AuthCommonOptions options, ILogger logger)
        //{
            //var client = CreateStoreClient(options, logger);

            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(options.ServerUrl);
            //HttpWebResponse response;
            //request.Headers.add
            //try
            //{
            //    response = await request.GetResponseAsync() as HttpWebResponse;
            //}
            //catch (WebException e)
            //{
            //    response = (HttpWebResponse)e.Response;
            //}

            //switch (response.StatusCode)
            //{
            //    case HttpStatusCode.OK:
            //        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", options.DotykMeToken);
            //        break;
            //    default:
            //        break;
            //}

            //if (options.DotykMeToken != null)
            //{
            //    logger?.LogTrace("Using token from window");
            //    client.AuthorizationToken = options.DotykMeToken;
            //    return client;
            //}

            //var dotykMe = new DotykClient(
            //    new FileCertificateStorage(
            //        Path.Combine(Environment.GetEnvironmentVariable("temp"), "Kodisoft", "DotykMe", "Certificates")));

            ////if (options.UseDotykMe)
            ////{
            //    var token = GetStoredToken(dotykMe, logger);

            //    if (token == null)
            //    {
            //        logger?.LogTrace("Token not found in registry. Requesting new token.");

            //        for (int i = 0; i < 10; i++)
            //        {
            //            string login,
            //            password;
            //            Dotyk.Extension.Windows.AuthForm authForm = new Dotyk.Extension.Windows.AuthForm();
            //            if (authForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //            {
            //                login = authForm.textBox1.Text;
            //                password = authForm.textBox2.Text;
            //            }
            //            else
            //            {
            //                throw new Exception("login and Password are required");
            //            } 

            //            try
            //            {
            //                logger?.LogTrace("Credentials read. Logging in");
            //                await dotykMe.Login(new Me.Model.LoginModel { Email = login, Password = password });

            //                logger?.LogTrace("Logged in to dotyk.me. Requesting token");
            //                token = (await dotykMe.GetRestrictedToken(Me.Model.TokenDuration.Long)).Token;

            //                logger?.LogTrace("Caching token");
            //                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Kodisoft\Dotyk\Store\DeploymentCLI", "AuthToken", token);

            //                var valid = await dotykMe.ValidateTokenAsync(token);

            //                logger?.LogInformation("Successfully logged in as {user}, token valid to {validTo}", valid.UserName, valid.ValidTo);
            //                break;
            //            }
            //            catch (Exception ex)
            //            {
            //                logger?.LogError(ex.Message, "Failed to login");
            //            }
            //        }
            //    }

            //    client.AuthorizationToken = token;
            //    return client;
            //}

            //logger?.LogWarning("Legacy login API is deprecated. Please migrate to Dotyk.Me authentication.");

            //try
            //{
            //    logger?.LogTrace("Using legacy API");


            //    await client.Login(new LoginViewModel
            //    {
            //        Email = options.Login,
            //        Password = options.Password
            //    });

            //    logger?.LogInformation("Logged in used legacy API");
            //}
            //catch
            //{
            //    if (!options.Register) throw;

            //    logger.LogInformation("Registering new user");

            //    await client.Register(new RegisterViewModel
            //    {
            //        Email = options.Login,
            //        Password = options.Password
            //    });

            //    await client.Login(new LoginViewModel
            //    {
            //        Email = options.Login,
            //        Password = options.Password
            //    });

            //    logger?.LogInformation("Logged in used legacy API");
            //}

            //return client;
        //}

        internal static string ResolveSolutionPath(string projectFile)
        {
            var currentDir = Path.GetDirectoryName(Path.GetFullPath(projectFile));

            while (currentDir != null)
            {
                var sln = Directory.EnumerateFiles(currentDir, "*.sln").FirstOrDefault();
                if (sln != null)
                    return sln;

                currentDir = Path.GetDirectoryName(currentDir);
            }

            return null;
        }

        private static string GetPassword()
        {
            var pwd = new StringBuilder();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.Remove(pwd.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd.Append(i.KeyChar);
                    Console.Write("*");
                }
            }
            return pwd.ToString();
        }

        private static string GetStoredToken(DotykClient dotykClient, ILogger logger)
        {
            var token = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Kodisoft\Dotyk\Store\DeploymentCLI", "AuthToken", String.Empty)?.ToString();

            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                var res = dotykClient.ValidateToken(token);

                logger.LogInformation("Using cached dotyk.me token for user {username}", res.UserName);

                return token;
            }
            catch (Exception ex)
            {
                logger.LogWarning(default(EventId), ex, "Failed to validate cached token");
                return null;
            }
        }
    }
}
