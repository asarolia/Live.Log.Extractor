namespace Live.Log.Extractor.IndexerService.Infrastructure
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Security.Principal;

    class UserImpersonation : IDisposable
    {
        [DllImport("advapi32.dll")]
        public static extern bool LogonUser(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool CloseHandle(IntPtr handle);

        WindowsImpersonationContext wic;
        IntPtr tokenHandle;
        string _userName;
        string _domain;
        string _passWord;

        public UserImpersonation(string userName, string domain, string passWord)
        {
            _userName = userName;
            _domain = domain;
            _passWord = passWord;
        }

        const int LOGON32_PROVIDER_DEFAULT = 0;
        const int LOGON32_LOGON_INTERACTIVE = 2;

        public bool ImpersonateValidUser()
        {
            bool returnValue = LogonUser(_userName, _domain, _passWord,
                    LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT,
                    ref tokenHandle);

            Trace.WriteLine("LogonUser called.");

            if (false == returnValue)
            {
                int ret = Marshal.GetLastWin32Error();
                Trace.WriteLine(string.Format("LogonUser failed with error code : {0}", ret));
                return false;
            }

            Trace.WriteLine("Did LogonUser Succeed? " + (returnValue ? "Yes" : "No"));
            Trace.WriteLine("Value of Windows NT token: " + tokenHandle);

            // Check the identity.
            Trace.WriteLine("Before impersonation: "
                + WindowsIdentity.GetCurrent().Name);
            // Use the token handle returned by LogonUser.
            WindowsIdentity newId = new WindowsIdentity(tokenHandle);
            wic = newId.Impersonate();

            // Check the identity.
            Trace.WriteLine("After impersonation: "
                + WindowsIdentity.GetCurrent().Name);
            return true;
        }

        #region IDisposable Members
        public void Dispose()
        {
            if (wic != null)
                wic.Undo();
            if (tokenHandle != IntPtr.Zero)
                CloseHandle(tokenHandle);

        }
        #endregion
    }

}