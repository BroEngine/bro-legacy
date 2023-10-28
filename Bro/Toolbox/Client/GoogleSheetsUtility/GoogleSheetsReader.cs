#if UNITY_EDITOR
using Bro.Json.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class GoogleSheetsReader : IDisposable
    {
        private string _spreadsheetId;
        private string _credentials;

        private GoogleSheetsReader(string spreadsheetId) : this(spreadsheetId, GoogleSheetsSettings.Instance.Credentials) { }

        private GoogleSheetsReader(string spreadsheetId, string credentials)
        {
            _spreadsheetId = spreadsheetId;
            _credentials = credentials;
        }


        public IEnumerable<(string title, IList<IList<object>> sheetData)> LoadAllSheets()
        {
            var sheetsService = CreateSheetsService(_credentials);
            var spreadsheet = sheetsService.Spreadsheets.Get(_spreadsheetId).Execute();
            var sheetCount = spreadsheet.Sheets.Count;
            var processed = 0;
            var progressTitle = $"Total sheet count: {sheetCount}";

            try
            {
                foreach (var sheet in spreadsheet.Sheets)
                {
                    var title = sheet.Properties.Title;
                    EditorUtility.DisplayProgressBar(progressTitle, $"Processing {title}", processed++ / (float)sheetCount);
                    if (title.StartsWith("*")) // for tables that shouldn't be processed
                    {
                        continue;
                    }
                    yield return (title, LoadSheet(sheetsService, title));
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                sheetsService.Dispose();
                Dispose();
            }
        }

        public IEnumerable<IList<object>> LoadSheet(string sheetTitle)
        {
            using (var sheetsService = CreateSheetsService(_credentials))
            {
                return LoadSheet(sheetsService, sheetTitle);
            }
        }

        private IList<IList<object>> LoadSheet(SheetsService sheetsService, string sheetTitle)
        {
            var sheetRequest = sheetsService.Spreadsheets.Values.Get(_spreadsheetId, sheetTitle);
            sheetRequest.MajorDimension = SpreadsheetsResource.ValuesResource.GetRequest.MajorDimensionEnum.ROWS;
            return sheetRequest.Execute().Values;
        }

        public static GoogleSheetsReader Create(string spreadsheetID)
        {
            return new GoogleSheetsReader(spreadsheetID);
        }

        public static GoogleSheetsReader Create(string spreadsheetID, string credentials)
        {
            return new GoogleSheetsReader(spreadsheetID, credentials);
        }

        private SheetsService CreateSheetsService(string credentials)
        {
            var json = File.ReadAllText(credentials);
            var serviceAccountEmail = JObject.Parse(json)["client_email"].Value<string>();
            var credential = (ServiceAccountCredential)GoogleCredential.FromJson(json).UnderlyingCredential;
            var initializer = new ServiceAccountCredential.Initializer(credential.Id)
            {
                User = serviceAccountEmail,
                Key = credential.Key,
                Scopes = new[] { SheetsService.Scope.Spreadsheets }
            };
            credential = new ServiceAccountCredential(initializer);
            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
            });
        }

        public void Dispose()
        {
            _spreadsheetId = default;
            _credentials = default;
        }
    }
}
#endif