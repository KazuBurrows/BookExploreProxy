using BookExploreProxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


/*
* Return json string of object
*/
public static class JsonConverter
{

    public static string encodeQuery(string payload_type, string[] payload)
    {

        Query query = new Query
        {
            PayloadType = payload_type,
            Payload = payload
        };
        return JsonConvert.SerializeObject(query, Formatting.Indented);
    }


    public static Query decodeQuery(string json_query)
    {
        return JsonConvert.DeserializeObject<Query>(json_query);
    }





    /*public static string encodeQueryResponse(int queryID, string data)
    {

        Query query = new Query
        {
            QueryID = queryID,
            QueryType = "Response",
            QueryObject = "String",
            Data = data,
        };

        return JsonConvert.SerializeObject(query, Formatting.Indented);
    }


    public static string encodeLoginResponse(string email, string password)
    {
        string response = "Login valid";

        return JsonConvert.SerializeObject(response, Formatting.Indented);
    }


    public static string encodeRegisterResponse(string email, string password)
    {
        Register register = new Register
        {
            Email = email,
            Password = password,
        };

        return JsonConvert.SerializeObject(register, Formatting.Indented);
    }


    *//*
        * Decode json Query object
        *//*
    public static Query decodeQuery(string json_query)
    {
        return JsonConvert.DeserializeObject<Query>(json_query);
    }*/



}
