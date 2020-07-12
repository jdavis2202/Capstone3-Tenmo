using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;
using TenmoClient.Models;

namespace TenmoClient
{
    public class APIService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public APIService()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
        }


        public decimal GetUserBalance()
        {
            decimal userBalance; 
            RestRequest request = new RestRequest(API_BASE_URL + "Accounts/Balance");
            IRestResponse response = client.Get(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return 0.00m;
            }
            else if (!response.IsSuccessful)
            {
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode + response.ErrorException.Message);
                }
                return 0.00m;
            }
            userBalance = Convert.ToDecimal(response.Content);
            return userBalance; 
        }

        public List<User> GetAllUsers()
        {
            List<User> allUsers;
            RestRequest request = new RestRequest(API_BASE_URL + "Accounts");
            IRestResponse<List<User>> response = client.Get<List<User>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode + response.ErrorException.Message);
                }
                return null;
            }
            return response.Data;
        }

        public string SendMoney(Transfer transfer)
        {
            string status = "";
            RestRequest request = new RestRequest(API_BASE_URL + "accounts/send");
            request.AddJsonBody(transfer);
            IRestResponse response = client.Put(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return "Error Connecting to Server";
            }
            else if (!response.IsSuccessful)
            {
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode + response.ErrorException.Message);
                }
                return null;
            }
            status = Convert.ToString(response.Content);
            return status;
        }


        public List<Transfer> GetMyTransfers()
        {
            List<Transfer> myTransfers;
            RestRequest request = new RestRequest(API_BASE_URL + "accounts/mytransfers");
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode + response.ErrorException.Message);
                }
                return null;
            }
            myTransfers = response.Data;
            return myTransfers;
        }

    }
}
