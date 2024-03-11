using System.Numerics;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.HdWallet;

namespace AlephVault.Unity.EVMGames.Wallets
{
    namespace Types
    {
        /// <summary>
        ///   A local wallet takes the mnemonic as a string, an eventual
        ///   password (which works well to make deniable wallets) and
        ///   the account index inside a wallet. The returned Web3 client
        ///   (which might use a default client with a particular gateway
        ///   or a custom client) is signing-enabled.
        /// </summary>
        public static class LocalWallet
        {
            private static Account GetAccount(
                string mnemonic, string password,
                int accountIndex = 0, BigInteger? chainId = null
            )
            {
                Wallet wallet = new Wallet(mnemonic, password);
                return wallet.GetAccount(accountIndex, chainId);
            }
            
            /// <summary>
            ///   Creates a Web3 client, with signing capabilities, using an endpoint
            ///   (which should use https) and a default client, a given mnemonic and
            ///   password, an account index and finally a chain id.
            /// </summary>
            /// <param name="mnemonic">The mnemonic to use (words separated by spaces)</param>
            /// <param name="password">The password to use, if any</param>
            /// <param name="gateway">The gateway URL to use</param>
            /// <param name="accountIndex">The account index. Must be >= 0 (0 by default)</param>
            /// <param name="chainId">The chain id</param>
            /// <returns>A Web3 client</returns>
            public static async Task<Web3> GetWeb3(
                string mnemonic, string password, string gateway = "http://localhost:8545",
                int accountIndex = 0, BigInteger? chainId = null
            )
            {
                return new Web3(GetAccount(
                    mnemonic, password, accountIndex, chainId
                ), gateway);
            }

            /// <summary>
            ///   Creates a Web3 client, with signing capabilities, using some custom
            ///   client, a given mnemonic and password, an account index and finally
            ///   a chain id a given account index and finally a chain id.
            /// </summary>
            /// <param name="mnemonic">The mnemonic to use (words separated by spaces)</param>
            /// <param name="password">The password to use, if any</param>
            /// <param name="client">The custom client to use</param>
            /// <param name="accountIndex">The account index. Must be >= 0 (0 by default)</param>
            /// <param name="chainId">The chain id</param>
            /// <returns>A Web3 client</returns>
            public static async Task<Web3> GetWeb3(
                string mnemonic, string password, IClient client, int accountIndex = 0,
                BigInteger? chainId = null
            )
            {
                return new Web3(GetAccount(
                    mnemonic, password, accountIndex, chainId
                ), client);
            }
        }
    }
}
