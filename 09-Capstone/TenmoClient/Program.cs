using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Threading;
using TenmoClient.Data;
using TenmoClient.Models;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();

        static void Main(string[] args)
        {
            Run();
        }
        private static void Run()
        {
            while (true)
            {
                int loginRegister = -1;
                while (loginRegister != 1 && loginRegister != 2)
                {
                    Console.WriteLine("Welcome to TEnmo!");
                    Console.WriteLine("1: Login");
                    Console.WriteLine("2: Register");
                    Console.WriteLine("0: Exit");
                    Console.Write("Please choose an option: ");

                    if (!int.TryParse(Console.ReadLine(), out loginRegister))
                    {
                        Console.WriteLine("Invalid input. Please enter only a number.\n");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                        Console.Clear();
                    }
                    else if (loginRegister == 0)
                    {
                        Environment.Exit(0);
                    }
                    else if (loginRegister == 1)
                    {
                        while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                        {
                            LoginUser loginUser = consoleService.PromptForLogin();
                            API_User user = authService.Login(loginUser);
                            if (user != null)
                            {
                                UserService.SetLogin(user);
                            }
                        }
                    }
                    else if (loginRegister == 2)
                    {
                        bool isRegistered = false;
                        while (!isRegistered) 
                        {
                            LoginUser registerUser = consoleService.PromptForLogin();
                            isRegistered = authService.Register(registerUser);
                            if (isRegistered)
                            {
                                Console.WriteLine("");
                                Console.WriteLine("Registration successful. You can now log in.");
                                Console.WriteLine("Press enter to continue.");
                                Console.ReadLine();
                                loginRegister = -1; //reset outer loop to allow choice for login
                                Console.Clear();
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid selection.\n");
                        Console.WriteLine("Press enter to continue.");
                        Console.ReadLine();
                        Console.Clear();

                    }
                }
                MenuSelection();
            }
        }

        private static void MenuSelection()
        {
            Console.Clear();
            APIService apiService = new APIService();
            int menuSelection = -1;
            while (menuSelection != 0) 
            {
                menuSelection = -1;
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                //Console.WriteLine("3: View your pending requests");
                Console.WriteLine("3: Send TE bucks");
                //Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("4: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                List<int> menus = new List<int>();
                menus.Add(0);
                menus.Add(1);
                menus.Add(2);
                menus.Add(3);
                menus.Add(4);

                while (!menus.Contains(menuSelection))
                {
                    if (!int.TryParse(Console.ReadLine(), out menuSelection))
                    {
                        Console.WriteLine("Invalid input. Please enter only a number.");
                        menuSelection = -1;
                    }
                }
                //if (!int.TryParse(Console.ReadLine(), out menuSelection))
                //{
                //    Console.WriteLine("Invalid input. Please enter only a number.");
                //    menuSelection = -1;
                //}
                //for (int i = 0; i < 5; i++)
                //{
                //    if (menuSelection == i) 
                //}
                if (menuSelection == 1)
                {
                    Console.Clear();
                    Console.WriteLine(" -------------");
                    Console.WriteLine(" TENMO BALANCE");
                    Console.WriteLine(" -------------\n");
                    //Console.WriteLine($"\nYour current Tenmo balance is:");
                    Console.WriteLine(" $" + apiService.GetUserBalance());
                    Console.WriteLine($"\nPress enter to continue.");
                    Console.ReadLine();
                    Console.Clear();

                }
                else if (menuSelection == 2)
                {

                    Console.Clear();
                    Console.WriteLine(" --------------------");
                    Console.WriteLine(" TENMO TRANSFER LIST");
                    Console.WriteLine(" --------------------\n");
                    List<User> users = apiService.GetAllUsers();
                    List<Transfer> myTransfers = apiService.GetMyTransfers();
                    List<int> currentTransferIDs = new List<int>();

                    foreach (Transfer transfer in myTransfers)
                    {
                        currentTransferIDs.Add(transfer.transferId);

                        string recieverName = "";
                        string senderName = "";

                        //The below block compares the incoming transfers against the list of users and pairs them to their appropriate names
                        //In retrospect, this probably also could have been doing in the transfer object itself. Good candidate for refactoring.

                        foreach (User user in users) 
                        {
                            if (user.UserId == transfer.reciever)
                            {
                                transfer.recieverName = user.Username;
                            }
                            if (user.UserId == transfer.sender)
                            {
                                transfer.senderName = user.Username;
                            }
                        }
                        Console.WriteLine($"#{transfer.transferId, -3} From: {transfer.senderName, -10}  To: {transfer.recieverName, -10}  {transfer.balance:c}");
                    }
                    Console.Write($"\nPlease enter transfer ID to view details (0 to cancel): ");

                    bool isItAnInteger = false;
                    int userId = 0;
                    while (!isItAnInteger)
                    {
                        string userInput = Console.ReadLine(); //Saves the user selection to a parsed integer
                        isItAnInteger = Int32.TryParse(userInput, out userId); //Saves the user selection to a parsed integer
                        if (!isItAnInteger || userId < 0)
                        {
                            Console.Write("Please enter a valid ID (shown above): ");
                            userId = 0;
                            isItAnInteger = false;
                        }

                        else if (!currentTransferIDs.Contains(userId) && (userId != 0))
                        {
                            Console.Write("Please enter a valid ID (shown above): ");
                            userId = 0;
                            isItAnInteger = false;
                        }

                    }

                    if (userId != 0)
                    {
                        Console.Clear();

                        foreach (Transfer transfer in myTransfers)
                        {
                            if (userId == transfer.transferId)
                            {
                                Console.WriteLine(" -------------------------------------");
                                Console.WriteLine($" TENMO TRANSFER DETAILS - TRANSFER {transfer.transferId,0}");
                                Console.WriteLine(" -------------------------------------\n");
                                Console.WriteLine($"\n Transfer Number: {transfer.transferId,0} \n From: {transfer.senderName,-10} \n To: {transfer.recieverName,-10} \n Type: {transfer.type,-10} \n Status: {transfer.status,-10} \n Amount: {transfer.balance:c}");
                            }
                        }
                        Console.WriteLine($"\nPress enter to continue.");
                        Console.ReadLine();
                    }
                    Console.Clear();
                }

                ////else if (menuSelection == 3)
                //{
                //    // View your pending requests
                //}

                else if (menuSelection == 3) // Send a user some money.
                {
                    Console.Clear();
                    Console.WriteLine("-------------------------------------------------------------");
                    Console.WriteLine($"Please select a user to whom you would like to send TE bucks!");
                    Console.WriteLine("-------------------------------------------------------------\n");
                    List<User> users = apiService.GetAllUsers();


                    int myUserIndex = 0;
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].UserId == UserService.GetUserId())
                        {
                            myUserIndex = i;
                        }
                    }
                    users.RemoveAt(myUserIndex);



                    foreach (User user in users)
                    {
                        Console.WriteLine($"User: {user.UserId} - {user.Username}\n");

                    }
                    Console.Write($"\nUser ID: ");

                    string userInput = Console.ReadLine(); //Saves the user selection to a parsed integer
                    Int32.TryParse(userInput, out int userId); //Saves the user selection to a parsed integer
                    Console.Write("How much money would you like to send? $");


                    decimal inputBalance = 0;
                    while (!(inputBalance > 0))
                    {
                        string inputBalanceString = Console.ReadLine();
                        Decimal.TryParse(inputBalanceString, out inputBalance);
                        if (inputBalance <= 0)
                        {
                            Console.Write("Please enter a decimal amount greater than zero: $");
                        }
                    }
                    Console.Clear();

                    decimal userCurrentBalance = apiService.GetUserBalance();
                    bool userMatch = false;
                    string status = "";
                    

                    if (userCurrentBalance >= inputBalance )
                    {
                        foreach (User user in users) //Ensures that the user's selection actually matches a real userid.
                        {
                            if ((userId == user.UserId) && (userId != UserService.GetUserId()))
                            {
                                Console.WriteLine("----------------------------------");
                                Console.WriteLine($"Sending TEBucks to {user.Username}!");
                                Console.WriteLine("----------------------------------\n");
                                userMatch = true;
                                Transfer transfer = new Transfer();
                                transfer.sender = UserService.GetUserId();
                                transfer.reciever = user.UserId;
                                transfer.balance = inputBalance;

                                Console.WriteLine($"Sending {inputBalance:c} to {user.Username}:\n"); 


                                Console.Write("Processing.");
                                for (int i = 0; i < 4; i++)
                                {
                                    Thread.Sleep(700);
                                    Console.Write(".");
                                }


                                status = apiService.SendMoney(transfer);
                                Console.WriteLine($"  Transfer status to {user.Username}: {status}\n");
                                Console.Write("Press enter to continue.");
                                Console.ReadLine();
                                break;
                            }
                        }

                        if (!userMatch)
                        {
                            Console.WriteLine("-----------------------------");
                            Console.WriteLine($"Error while sending TEBucks!");
                            Console.WriteLine("-----------------------------\n");
                            Console.WriteLine("\nSorry, you didn't enter a valid user ID.\n");
                            Console.Write("Press enter to continue.");
                            Console.ReadLine();
                        }

                    }
                    else
                    {
                        Console.WriteLine("-----------------------------");
                        Console.WriteLine($"Error while sending TEBucks!");
                        Console.WriteLine("-----------------------------\n");
                        Console.WriteLine("Sorry! You do not have enough TEBucks!");
                        Console.Write("Press enter to continue.");
                        Console.ReadLine();
                    }

                    Console.Clear();
                }

                //else if (menuSelection == 5) //TODO comment out this logic for this menu if we do not implement
                //{
                //    // Request TE bucks

                //}
                else if (menuSelection == 4)
                {
                    // Log in as different user
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Console.Clear();
                    return; //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
