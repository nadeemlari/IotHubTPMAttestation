using Microsoft.Azure.Devices.Provisioning.Security;

using var security = new SecurityProviderTpmHsm(null);
Console.WriteLine($"Your EK is {Convert.ToBase64String(security.GetEndorsementKey())}");