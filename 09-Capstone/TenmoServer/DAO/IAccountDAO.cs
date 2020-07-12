using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDAO
    {
        Decimal GetBalance(string username);

        string SendTEBucks(int sendingUser, int recieveingUser, decimal transferAmount);

        List<Transfer> GetMyTransfers(string username);

    }
}

