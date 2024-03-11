using System.Threading.Tasks;
using AlephVault.Unity.Support.Utils;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Paper;

namespace AlephVault.Unity.EVMGames.Wallets
{
    namespace Types
    {
        /// <summary>
        ///   This is a Paper <see cref="http://withpaper.com"/> wrapper
        ///   for login. As easy as providing an e-mail address, users
        ///   can authenticate into the app by solving a small challenge
        ///   involving a recovery key or an OTP.
        /// </summary>
        public static class PaperWallet
        {
            /// <summary>
            ///   The OTP listener. The user must initialize one listener
            ///   first, and then invoke <see cref="PaperWallet.GetWeb3" />
            ///   by passing it as argument. Once that method is running,
            ///   it will wait until the user invokes <see cref="SendOTP"/>
            ///   in this listener, with a non-null-or-empty value.
            /// </summary>
            public class OTPListener
            {
                public OTPListener()
                {
                    Password = null;
                }

                /// <summary>
                ///   Retrieves the currently set password.
                /// </summary>
                public string Password { get; private set; }
                
                /// <summary>
                ///   Sends the OTP to the handler. Invoke this only once
                ///   and only while <see cref="PaperWallet.GetWeb3"/> runs.
                /// </summary>
                /// <param name="password">The OTP to set</param>
                public void SendOTP(string password)
                {
                    password = password.Trim();
                    if (!string.IsNullOrEmpty(password) && string.IsNullOrEmpty(Password))
                    {
                        Password = password;
                    }
                }
            }
            
            private static async Task<Account> GetAccount(
                string clientAppId, string email, OTPListener otpListener, string recoveryKey
            )
            {
                PaperEmbeddedWalletSdk sdk = new PaperEmbeddedWalletSdk(clientAppId);
                // isNewUser: The user was just created.
                // currentUserMismatch: The user id is different one from the previously
                //   kept user id in this device.
                (bool isNewUser, bool currentUserMismatch) = await sdk.SendPaperEmailLoginOtp(email);
                
                // If the user is new, then recovery will be ignored.
                recoveryKey = recoveryKey?.Trim();
                if (isNewUser || string.IsNullOrEmpty(recoveryKey)) recoveryKey = null;

                // Wait until the OTP is set, and get it.
                while (otpListener.Password == null)
                {
                    await Tasks.Blink();
                }
                string otp = otpListener.Password;
                
                // End the process for good.
                User user = await sdk.VerifyPaperEmailLoginOtp(email, otp, recoveryKey);
                return user.Account;
            }
            
            /// <summary>
            ///   Creates a Web3 client, with signing capabilities, using an endpoint
            ///   (which should use https) and the data to prepare a Paper connection:
            ///   the client app ID, the user's e-mail, an eventual recovery key (null
            ///   if not set) and a listener for the user to set the OTP.
            /// </summary>
            /// <param name="clientAppId">The ID of the registered client app in Paper</param>
            /// <param name="email">The user's e-mail address</param>
            /// <param name="gateway">The gateway URL to use</param>
            /// <param name="otpListener">A listener for the user to set the OTP</param>
            /// <param name="recoveryKey">
            ///   The recovery key, if the user wants to recover the account from another
            ///   device (e.g. due to loss).
            /// </param>
            /// <returns>A Web3 client</returns>
            public static async Task<Web3> GetWeb3(
                string clientAppId, string email, OTPListener otpListener,
                string gateway = "http://localhost:8545", string recoveryKey = null
            )
            {
                return new Web3(await GetAccount(
                    clientAppId, email, otpListener, recoveryKey
                ), gateway);
            }

            /// <summary>
            ///   Creates a Web3 client, with signing capabilities, using some custom
            ///   client and the data to prepare a Paper connection: the client app ID,
            ///   the user's e-mail, an eventual recovery key (null if not set) and a
            ///   listener for the user to set the OTP.
            /// </summary>
            /// <param name="clientAppId">The ID of the registered client app in Paper</param>
            /// <param name="email">The user's e-mail address</param>
            /// <param name="client">The custom client to use</param>
            /// <param name="otpListener">A listener for the user to set the OTP</param>
            /// <param name="recoveryKey">
            ///   The recovery key, if the user wants to recover the account from another
            ///   device (e.g. due to loss).
            /// </param>
            /// <returns>A Web3 client</returns>
            public static async Task<Web3> GetWeb3(
                string clientAppId, string email, OTPListener otpListener,
                IClient client, string recoveryKey = null
            )
            {
                return new Web3(await GetAccount(
                    clientAppId, email, otpListener, recoveryKey
                ), client);
            }
        }
    }
}