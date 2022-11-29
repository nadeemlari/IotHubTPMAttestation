using Microsoft.Azure.Devices.Provisioning.Security;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Provisioning.Client;
using System.Text;
using Microsoft.Azure.Devices.Client;

Console.WriteLine("Initializing security using the local TPM...");
using SecurityProviderTpm security = new SecurityProviderTpmHsm("reg-d-01");

Console.WriteLine($"Initializing the device provisioning client...");

var transportHandler = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly);

var provClient = ProvisioningDeviceClient.Create("global.azure-devices-provisioning.net", "0ne0087A2D6",security,transportHandler);
Console.WriteLine($"Initialized for registration Id {security.GetRegistrationID()}.");

Console.WriteLine("Registering with the device provisioning service... ");
DeviceRegistrationResult result = await provClient.RegisterAsync();
Console.WriteLine($"Device {result.DeviceId} registered to {result.AssignedHub}.");

Console.WriteLine("Creating TPM authentication for IoT Hub...");
using var auth = new DeviceAuthenticationWithTpm(result.DeviceId, security);

Console.WriteLine($"Testing the provisioned device with IoT Hub...");
using var iotClient = DeviceClient.Create(result.AssignedHub, auth,TransportType.Mqtt);

Console.WriteLine("Sending a telemetry message...");
using var message = new Message(Encoding.UTF8.GetBytes("TestMessage"));
await iotClient.SendEventAsync(message);

await iotClient.CloseAsync();
Console.WriteLine("Finished.");