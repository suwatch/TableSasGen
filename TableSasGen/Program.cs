using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;

namespace TableSasGen
{
    class Program
    {
        static void Main(string[] args)
        {
            const string PolicyName = "TableSasGenPolicy";

            try
            {
                if (args.Length < 3)
                {
                    Console.WriteLine("Usage: TableSasGen.exe <storageAccount> <tableName> <storageKey>");
                    return;
                }

                var storageAccount = args[0];
                var tableName = args[1];
                var storageKey = args[2];

                var storageUri = new Uri(String.Format("https://{0}.table.core.windows.net/", storageAccount));

                CloudTableClient client = new CloudTableClient(storageUri, new StorageCredentials(storageAccount, storageKey));
                CloudTable table = client.GetTableReference(tableName);
                TablePermissions permissions = table.GetPermissions();
                SharedAccessTablePolicy policy = null;
                if (!permissions.SharedAccessPolicies.ContainsKey(PolicyName))
                {
                    policy = new SharedAccessTablePolicy()
                    {
                        Permissions = SharedAccessTablePermissions.Add | SharedAccessTablePermissions.Delete | SharedAccessTablePermissions.Query | SharedAccessTablePermissions.Update,
                        SharedAccessExpiryTime = DateTimeOffset.MaxValue,
                    };

                    permissions.SharedAccessPolicies.Add(PolicyName, policy);

                    table.SetPermissions(permissions);

                }
                else
                {
                    policy = permissions.SharedAccessPolicies[PolicyName];
                }

                var sasToken = table.GetSharedAccessSignature(policy);
                Console.WriteLine("{0}{1}", storageUri, sasToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
