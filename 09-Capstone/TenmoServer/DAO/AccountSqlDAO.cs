using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDAO : IAccountDAO
    {

        private readonly string connectionString;

        public AccountSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }



        public decimal GetBalance(string username)
        {
            string name = username;
            decimal userBalance = 0.00M;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sqlCommand = "SELECT balance FROM Accounts JOIN users ON users.user_id = accounts.user_id WHERE Users.username = @name";
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlCommand, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    userBalance = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }

            catch (Exception ex)
            {
                throw;
                //Console.WriteLine($"Oops, exception while attempting to GetBalance: {ex.Message}");
            }

          
            return userBalance;
        }


        public string SendTEBucks(int sendingUser, int recieveingUser, decimal transferAmount)
        {

            string status = "";
            int transferID;
            decimal senderNewBalance;
            decimal recieverNewBalance;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string cmd_insertTransfer = "INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) VALUES (2,2,@account_from, @account_to, @amount); SELECT @@IDENTITY"; // returns a single thing, (a value that becomes @transfer_id) (scalar?)
                    string cmd_returnsTransferStatus = "select transfer_status_desc from transfer_statuses join transfers on transfers.transfer_status_id = transfer_statuses.transfer_status_id where transfer_id = @transfer_id";
                    string cmd_subtractAmountFromSender = "UPDATE accounts SET balance = (balance - @amount) FROM accounts join users on users.user_id = accounts.user_id where users.user_id = @account_from; Select balance from accounts where USER_ID = @account_from;";
                    string cmd_depositAmountToReciever = "UPDATE accounts SET balance = (balance + @amount) FROM accounts join users on users.user_id = accounts.user_id where users.user_id = @account_to; Select balance from accounts where USER_ID = @account_to;";

                    //Logic for the First Query
                    SqlCommand cmd = new SqlCommand(cmd_insertTransfer, conn);
                    cmd.Parameters.AddWithValue("@account_from", sendingUser);
                    cmd.Parameters.AddWithValue("@account_to", recieveingUser);
                    cmd.Parameters.AddWithValue("@amount", transferAmount);
                    transferID = Convert.ToInt32(cmd.ExecuteScalar());

                    //Logic for the Second Query
                    cmd = new SqlCommand(cmd_returnsTransferStatus, conn);
                    cmd.Parameters.AddWithValue("@transfer_id", transferID);
                    status = Convert.ToString(cmd.ExecuteScalar());

                    //Logic for the Third Query
                    cmd = new SqlCommand(cmd_subtractAmountFromSender, conn);
                    cmd.Parameters.AddWithValue("@amount", transferAmount);
                    cmd.Parameters.AddWithValue("@account_from", sendingUser);
                    senderNewBalance = Convert.ToDecimal(cmd.ExecuteScalar());

                    //Logic for the Fourth Query
                    cmd = new SqlCommand(cmd_depositAmountToReciever, conn);
                    cmd.Parameters.AddWithValue("@amount", transferAmount);
                    cmd.Parameters.AddWithValue("@account_to", recieveingUser);
                    recieverNewBalance = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                throw;
                //Console.WriteLine($"Oops, exception while attempting to SendTEBucks: {ex.Message}");
            }

            return status;
        }
        public List<Transfer> GetMyTransfers(string username)
        {
            List<Transfer> userTransfer = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string cmd_getMyTransfers = "select * from transfers join accounts on account_id = transfers.account_from join users on users.user_id = accounts.user_id join transfer_statuses on transfer_statuses.transfer_status_id = transfers.transfer_status_id join transfer_types on transfer_types.transfer_type_id = transfers.transfer_type_id where transfers.account_from = (select users.user_id from users where username = @userName) or transfers.account_to = (select users.user_id from users where username = @userName)";
                    //string cmd_getMyTransfers = "select * from transfers join accounts on account_id = transfers.account_from join users on users.user_id = accounts.user_id where transfers.account_from = (select users.user_id from users where username = @userName) or transfers.account_to = (select users.user_id from users where username = @userName)";

                    SqlCommand cmd = new SqlCommand(cmd_getMyTransfers, conn);
                    cmd.Parameters.AddWithValue("@userName", username);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Transfer transfer = new Transfer();
                        transfer.reciever = Convert.ToInt32(rdr["account_to"]);
                        transfer.sender = Convert.ToInt32(rdr["account_from"]);
                        transfer.balance = Convert.ToDecimal(rdr["amount"]);
                        transfer.transferId = Convert.ToInt32(rdr["transfer_id"]);
                        transfer.status = Convert.ToString(rdr["transfer_status_desc"]);
                        transfer.type = Convert.ToString(rdr["transfer_type_desc"]);

                        userTransfer.Add(transfer);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
                //Console.WriteLine($"Exception while attempting to GetMyTransfers: {ex.Message}");
            }
            return userTransfer;
        }
    }
}
