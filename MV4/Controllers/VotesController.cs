using System.IO;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using MV4.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
//using namespace MV4.Models.Track;


public class VotesController : Controller
{
    //processes only one request at a time to prevent corruption and overwriting
    private static SemaphoreSlim _fileLock = new SemaphoreSlim(1);     //if _fileLock = 0, other request have to wait

    bool x = false;

    public async Task<IActionResult> SaveVotesAsync(string trackId)
    {
        await _fileLock.WaitAsync();
        Console.WriteLine("fileLock initiated");
        


        //getIP DOES NOT WORK ON LOCAL HOST!
        string userIpAddress = Request.Headers["X-Forwarded-For"];

        if (string.IsNullOrEmpty(userIpAddress))
        {
            userIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
        }
        else
        {
            // the X-Forwarded-For header can contain multiple IP addresses, separated by commas
            // we only want the first one, which should be the client's IP address
            userIpAddress = userIpAddress.Split(',')[0].Trim();
        }
        //check if exists
        if (string.IsNullOrEmpty(userIpAddress))
        {
            return BadRequest("Invalid IP address");
        }

        // Validate the user's IP address
        if (!IPAddress.TryParse(userIpAddress, out IPAddress ipAddress))
        {
            return BadRequest("Invalid IP address");
        }
        Console.WriteLine("IP: " + userIpAddress);



        // Read the JSON file
        string jsonFilePath = "votes.json";
        string jsonData = System.IO.File.ReadAllText(jsonFilePath);
        Console.WriteLine("JSON file read");

        JToken votes;
        if (jsonData.Trim().StartsWith("{"))
        {
            // If the file contains a JSON object, create a new array and add the object to it
            //votes = new JArray(new JObject(new JProperty("trackId", jsonData), new JProperty("count", 000), new JProperty("IPs", new JArray("userIpAdress"))));
            votes = new JArray(new JObject(new JProperty("trackId", "1jLsirPDkUS2g4gnkYua58"), new JProperty("count", 0), new JProperty("IPs", new JArray("userIpAdress")), new JProperty("downvoteIPs", new JArray("userIpAdress"))));
        }
        else
        {
            // If the file contains a JSON array, parse it as such
            votes = JArray.Parse(jsonData);
        }

        // Check if the track ID already exists in the file
        bool voteAdded = false;       
        //bool x = false;
        bool trackExists = false;
        //Console.WriteLine(votes.ToString());
        foreach (JObject vote in votes)
        {
            if ((string)vote["trackId"] == trackId)
            {
                // Check if the user's IP address is already in the list for this track ID
                JArray _ipList = (JArray)vote["downvoteIPs"];
                JArray ipList = (JArray)vote["IPs"];
                bool ipExists = false;
                bool sameIP = false;
                
                foreach (string ip in ipList)
                {
                    if (ip == userIpAddress)
                    {
                        Console.WriteLine("User already voted for this track");
                        ipExists = true;
                        trackExists = true;
                        x = true;
                        break;
                    }
                }
                // Add the user's IP address to the list if it doesn't already exist               
                if (!ipExists)
                {
                    
                    foreach (string ip in _ipList)
                    {
                        if (ip == userIpAddress)
                        {
                            _ipList = new JArray(_ipList.Where(ip => ip.Value<string>() != userIpAddress));
                            vote["downvoteIPs"] = _ipList;
                            Console.WriteLine("DIP Removed: " + userIpAddress);
                            Console.WriteLine(_ipList);

                            vote["count"] = (int)vote["count"] + 1;
                            Console.WriteLine("Vote count increased! ++1 | and IP deleted");
                            Console.WriteLine(vote["count"]);
                            //ipList.Add(userIpAddress);
                            voteAdded = true;
                            sameIP = true;
                            trackExists = true;                            
                            break;
                        }
                        
                    }
                    if (sameIP == false)
                    {
                        ipList.Add(userIpAddress);
                        trackExists = true;
                        vote["count"] = (int)vote["count"] + 1;
                        Console.WriteLine("vote added!");
                        Console.WriteLine(vote["count"]);
                        voteAdded = true;
                       
                    }
                }

                break;
            }
        }
        
        


        // If the track ID doesn't exist in the file, add it with a count of 1 and the user's IP address
        if (!trackExists)
        {
            JObject newVote = new JObject();
            newVote.Add("trackId", trackId);
            newVote.Add("count", 1);
            newVote.Add("IPs", new JArray(userIpAddress));
            newVote.Add("downvoteIPs", new JArray("0"));
            ((JArray)votes).Add(newVote);
            voteAdded = true;
            Console.WriteLine("New track added!");
        }

        // Write the updated JSON file
        Console.WriteLine("Writing JSON");
        using (StreamWriter sw = new StreamWriter(jsonFilePath))
        using (JsonTextWriter writer = new JsonTextWriter(sw))
        {
            votes.WriteTo(writer);
        }
        _fileLock.Release();
        Console.WriteLine("fileLock Released");
        return Json(new { VoteAdded = voteAdded, Xcptn = x});
        //return Ok();
    }



    //DOWNVOTES

    public async Task<IActionResult> downVotesAsync(string trackId)
    {
        await _fileLock.WaitAsync();
        Console.WriteLine("fileLock initiated");
        

        //getIP DOES NOT WORK ON LOCAL HOST!
        string userIpAddress = Request.Headers["X-Forwarded-For"];

        if (string.IsNullOrEmpty(userIpAddress))
        {
            userIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
        }
        else
        {
            // the X-Forwarded-For header can contain multiple IP addresses, separated by commas
            // we only want the first one, which should be the client's IP address
            userIpAddress = userIpAddress.Split(',')[0].Trim();
        }
        //check if exists
        if (string.IsNullOrEmpty(userIpAddress))
        {
            return BadRequest("Invalid IP address");
        }

        // Validate the user's IP address
        if (!IPAddress.TryParse(userIpAddress, out IPAddress ipAddress))
        {
            return BadRequest("Invalid IP address");
        }
        Console.WriteLine("IP: " + userIpAddress);

        //DECREASE COUNT

        // Read the JSON file
        string jsonFilePath = "votes.json";
        string jsonData = System.IO.File.ReadAllText(jsonFilePath);
        Console.WriteLine("JSON file read");

        JToken votes;
        if (jsonData.Trim().StartsWith("{"))
        {
            // If the file contains a JSON object, create a new array and add the object to it
            votes = new JArray(new JObject(new JProperty("trackId", "1jLsirPDkUS2g4gnkYua58"), new JProperty("count", 0), new JProperty("IPs", new JArray("userIpAdress")), new JProperty("downvoteIPs", new JArray("userIpAdress"))));
        }
        else
        {
            // If the file contains a JSON array, parse it as such
            votes = JArray.Parse(jsonData);
        }

        // Find the track ID in the JSON file
        bool _voteAdded = false;
        bool trackExists = false;
        foreach (JObject vote in votes)
        {
            if ((string)vote["trackId"] == trackId)
            {
                // Check if the user's IP address is already in the list for this track ID
                JArray _ipList = (JArray)vote["IPs"];
                JArray ipList = (JArray)vote["downvoteIPs"];
                bool ipExists = false;
                bool sameIP = false;
                foreach (string ip in ipList)
                {
                    if (ip == userIpAddress)
                    {
                        Console.WriteLine("User already downvoted this track");
                        ipExists = true;
                        trackExists = true;
                        break;
                    }
                }
                // Add the user's IP address to the list if it doesn't already exist
                if (!ipExists)
                {
                    foreach(string ip in _ipList) 
                    {
                        if (ip == userIpAddress) 
                        {
                            _ipList = new JArray(_ipList.Where(ip => ip.Value<string>() != userIpAddress)); //removeIP
                            vote["IPs"] = _ipList;
                            Console.WriteLine("UIP Removed: " + userIpAddress);
                            Console.WriteLine(_ipList);
                           
                            vote["count"] = (int)vote["count"] - 1;
                            Console.WriteLine("Vote count decreased! --1 | and IP deleted");
                            Console.WriteLine(vote["count"]);
                            //ipList.Add(userIpAddress);
                            _voteAdded = true;
                            sameIP = true;
                            trackExists = true;
                            break;
                        }
                        
                    }  
                    if (sameIP == false)
                    {
                        ipList.Add(userIpAddress);
                        vote["count"] = (int)vote["count"] - 1;
                        Console.WriteLine("Vote count decreased!");
                        Console.WriteLine(ipList.ToString());
                        Console.WriteLine(vote["count"]);
                        _voteAdded = true;
                        break;
                    }

                }

                break;
            }
        }
        // Write the updated JSON file
        Console.WriteLine("Writing JSON");
        using (StreamWriter sw = new StreamWriter(jsonFilePath))
        using (JsonTextWriter writer = new JsonTextWriter(sw))
        {
            votes.WriteTo(writer);
        }

        _fileLock.Release();
        Console.WriteLine("fileLock Released");
        return Json(new { _VoteAdded = _voteAdded });      
        //return Ok();
    }



}

