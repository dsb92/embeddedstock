using System;
using System.DirectoryServices;

namespace Web.Models
{
    public class Authenticate
    {
        public static bool IsAuthenticated(string usr, string pwd)
{
    bool authenticated = false;
 
    try
    {
        DirectoryEntry entry = new DirectoryEntry("LDAP://ldap.iha.dk", usr, pwd);
        object nativeObject = entry.NativeObject;
        authenticated = true;
    }
    catch (DirectoryServicesCOMException cex)
    {
        Console.WriteLine(cex);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
    return authenticated;
}
    }
}