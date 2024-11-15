namespace Qala.Cli.Utils;

public class Enums 
{
    public enum ProvisioningStates
    {
        New,
        Provisioning,
        Provisioned,
        Deleting,
        Deleted,
        ProvisioningFailed,
        DeletionFailed
    }
}