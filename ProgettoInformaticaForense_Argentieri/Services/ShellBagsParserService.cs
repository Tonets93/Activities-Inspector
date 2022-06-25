using CSharpFunctionalExtensions;
using ProgettoInformaticaForense_Argentieri.Models;
using ProgettoInformaticaForense_Argentieri.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ProgettoInformaticaForense_Argentieri.Services
{
    public class ShellBagsParserService : IShellBagsParserService
    {
        private readonly FileLocations locations;

        public ShellBagsParserService()
        {
            locations = InitPaths();
        }

        public async Task<Result<List<IShellItem>>> ParseShellBags()
        {
            var taskCompletionSource = new TaskCompletionSource<Result<List<IShellItem>>>(
                TaskCreationOptions.RunContinuationsAsynchronously);

            try
            {
                var retList = new List<IShellItem>();

                await Task.Run(() =>
                {
                    var parser = new ConfigParser(locations.GUIDFileLocation, locations.OSFileLocation,
                        locations.ScriptFileLocation);

                    var onlineReader = new OnlineRegistryReader(parser, false);
                    retList.AddRange(ShellBagParser.GetShellItems(onlineReader));

                    taskCompletionSource.SetResult(Result.Success(retList));
                });
            }
            catch (Exception ex)
            {
                taskCompletionSource.SetResult(Result.Failure<List<IShellItem>>(ex.Message));
            }

            return taskCompletionSource.Task.Result;

            return await Task.Run(() =>
            {
                List<IShellItem> retList = new List<IShellItem>();

                ConfigParser parser = new ConfigParser(locations.GUIDFileLocation, locations.OSFileLocation,
                    locations.ScriptFileLocation);

                //perform offline shellbag parsing
                OnlineRegistryReader onlineReader = new OnlineRegistryReader(parser, false);
                retList.AddRange(ShellBagParser.GetShellItems(onlineReader));

                return retList;
            });
        }

        private FileLocations InitPaths()
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            var guids = @"Assets\GUIDs.json";
            var os = @"Assets\OS.json";
            var scripts = @"Assets\Scripts.json";

            return new FileLocations(os, guids, scripts);
        }
    }
}
