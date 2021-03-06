﻿using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

//https://docs.google.com/spreadsheets/d/1YPcGEQC6DMhw6WX2Q13gLFpgPBOgpozA-ywmeZyHONk/edit?usp=sharing

class DataBase
{
    private readonly static string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private readonly static string ApplicationName = "Legacy Rentals DataBase";

    private readonly static String spreadsheetId = "1YPcGEQC6DMhw6WX2Q13gLFpgPBOgpozA-ywmeZyHONk";

    private static UserCredential credential;
    private static SheetsService service;

    public DataBase()
    {
        using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        {
            string credPath = "token.json";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)
                ).Result;
            }

        service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
    }

    //API Documentation
    //Method: readData(String range)
    //String range: sheetName!ColumnStart:ColumnEnd
    //RequestType: Reads all data provided in range starting at Row and going through the entire column
    public void writeData(String range, List<object> data)
    {
        var valueRange = new ValueRange();

        valueRange.Values = new List<IList<object>>{data};

        var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
        appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
        var appendReponse = appendRequest.Execute();
    }

    //API Documentation
    //Method: readData(String range)
    //String range: sheetName!ColumnStart:ColumnEnd
    //RequestType: Reads all data provided in range starting at Row and going through the entire column
    public void updateData(String range, List<object> data)
    {
        var valueRange = new ValueRange();

        valueRange.Values = new List<IList<object>>{data};

        var updateRequest = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, range);
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        var appendReponse = updateRequest.Execute();
    }

    //API Documentation
    //Method: deleteData(String range)
    //String range: sheetName!ColumnStart:ColumnEnd
    //RequestType: Deletes data at range
    public void deleteData(String range)
    {
        var requestBody = new ClearValuesRequest();

        var deleteRequest = service.Spreadsheets.Values.Clear(requestBody, spreadsheetId, range);
        var deleteReponse = deleteRequest.Execute();
    }

    //API Documentation
    //Method: readData(String range)
    //String range: sheetName!ColumnStart:ColumnEnd
    //RequestType: Reads all data in range starting at Row and going through the entire column
    public IList<IList<Object>> readData(String range)
    {
        var valueRange = new ValueRange();

        var getRequest = service.Spreadsheets.Values.Get(spreadsheetId, range);
        var getReponse = getRequest.Execute();

        return getReponse.Values;
    }
}