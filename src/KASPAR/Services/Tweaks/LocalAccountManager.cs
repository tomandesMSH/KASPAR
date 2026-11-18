using System.Runtime.InteropServices;

namespace KASPAR.Services.Tweaks;

/// <summary>
/// Creates local Windows accounts via netapi32 (replaces LocalAccount.exe, which shelled out to `net user`/`net localgroup`).
/// </summary>
internal static class LocalAccountManager
{
    private const int UserPrivUser = 1;
    private const int UserFlagScript = 0x0001;
    private const string AdministratorsGroup = "Administrators";

    public static void Create(string username, string password, bool asAdmin)
    {
        var info = new UserInfo1
        {
            usri1_name = username,
            usri1_password = password,
            usri1_password_age = 0,
            usri1_priv = UserPrivUser,
            usri1_home_dir = null,
            usri1_comment = null,
            usri1_flags = UserFlagScript,
            usri1_script_path = null,
        };

        var status = NetUserAdd(null, 1, ref info, out var paramError);
        if (status != 0)
        {
            throw new InvalidOperationException(DescribeUserError(status, paramError));
        }

        if (!asAdmin)
        {
            return;
        }

        var member = new LocalGroupMembersInfo3 { lgrmi3_domainandname = username };
        var groupStatus = NetLocalGroupAddMembers(null, AdministratorsGroup, 3, ref member, 1);
        if (groupStatus != 0)
        {
            throw new InvalidOperationException(
                $"The account was created, but could not be added to Administrators: {DescribeGroupError(groupStatus)}");
        }
    }

    private static string DescribeUserError(int status, int paramError) => status switch
    {
        5 => "Access denied - run KASPAR as administrator.",
        2224 => "An account with that username already exists.",
        2202 => "That username is not valid.",
        2245 => "That password does not meet this computer's password policy.",
        87 => $"Invalid account details (field #{paramError}).",
        _ => $"Could not create the account (NetUserAdd error {status}).",
    };

    private static string DescribeGroupError(int status) => status switch
    {
        5 => "access denied.",
        1378 => "the account is already an administrator.",
        1376 => "the Administrators group does not exist on this computer.",
        _ => $"NetLocalGroupAddMembers error {status}.",
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct UserInfo1
    {
        public string usri1_name;
        public string? usri1_password;
        public int usri1_password_age;
        public int usri1_priv;
        public string? usri1_home_dir;
        public string? usri1_comment;
        public int usri1_flags;
        public string? usri1_script_path;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct LocalGroupMembersInfo3
    {
        public string lgrmi3_domainandname;
    }

    [DllImport("netapi32.dll", CharSet = CharSet.Unicode)]
    private static extern int NetUserAdd(string? servername, int level, ref UserInfo1 buf, out int parm_err);

    [DllImport("netapi32.dll", CharSet = CharSet.Unicode)]
    private static extern int NetLocalGroupAddMembers(string? servername, string groupname, int level, ref LocalGroupMembersInfo3 buf, int totalentries);
}
