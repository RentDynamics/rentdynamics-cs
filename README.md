# RentDynamics.RdClient

[![Circle CI Badge](https://circleci.com/gh/RentDynamics/rentdynamics-cs/tree/dev.svg?style=shield&circle-token=8ca42b3ae23f8df7f754457b3daae599f716f85c)](https://circleci.com/gh/RentDynamics/rentdynamics-cs)
[![codecov](https://codecov.io/gh/RentDynamics/rentdynamics-cs/branch/dev/graph/badge.svg)](https://codecov.io/gh/RentDynamics/rentdynamics-cs)
[![MIT License](https://img.shields.io/npm/l/rentdynamics.svg)](LICENSE)
[![Nuget](https://img.shields.io/nuget/v/RentDynamics.RdClient?style=flat)](https://www.nuget.org/packages/RentDynamics.RdClient)

C# client package to access the RentDynamics API

## Installation

```
Install-Package RentDynamics.RdClient
```

## API credentials and authentication
You need a set of credentials to access the API: `apiKey` and `apiSecretKey`.

There are 2 ways to authenticate. You can authenticate with a username & password, or an API key that doesn't require the login method to be invoked with a username & password.

If you authenticate with a username & password then you will need to use the `AuthenticationResource` class ([see example below](#console-apps-with-no-di-container)).


## Simple usage

### Console apps with no DI container

```c#
string apiKey = "<api-key>";
string apiSecretKey = "<api-secret-key>";

bool isDevelopment = false; //The default is production. If you want to interact with the development environment set this equal to true

var options = new RentDynamicsOptions(apiKey, apiSecretKey, isDevelopment: isDevelopment);
var rdServices = new ServiceCollection()
                    .AddRentDynamicsApiClient<IRentDynamicsApiClient, RentDynamicsApiClient>(clientName: "default", options, clientLifetime: ServiceLifetime.Singleton) //Client has Scoped lifetime by default. When using in a console app, it makes sense to have it as a Singleton.
                    .BuildServiceProvider();
                     

var rdApiClient = rdServices.GetRequiredService<IRentDynamicsApiClient>(); //Store API client somewhere in a `static` field and reuse it for single-user scenarios

var authenticationResource = new AuthenticationResource(rdApiClient);
await authenticationResource.LoginAsync("<your-username>", "<your-password>"); //Optional step depending on how you will authenticate. This is necessary if you will be authenticating with a username & password. It is not necessary if you are using an apiKey that by-passes traditional authentication.

var leadCardResource = new LeadCardResource(rdApiClient);

var leadCard = new LeadCard("Peter", "example@email.com", phoneNumber: null) 
{
    LastName = "Parker"
};

int communityId = "<community-id>";
LeadCard leadCardResult = await leadCardResource.CreateLeadCardAsync(communityId, leadCard);
```

### AspNetCore apps with DI container

In your `Startup.cs` file:

```c#
string apiKey = "<api-key>";
string apiSecretKey = "<api-secret-key>";

bool isDevelopment = false; //The default is production. If you want to interact with the development environment set this equal to true

var options = new RentDynamicsOptions(apiKey, apiSecretKey, isDevelopment: isDevelopment);
services.AddDefaultRentDynamicsClient(clientName: "default", options, clientLifetime: ServiceLifetime.Scoped); //For ASP.NET Core apps it is better to use client with Scoped lifetime. 
```

It will register `IRentDynamicsApiClient` interface implementation alongside some internal classes and also resource classes.

After that from your service/controller class you can consume `IRentDynamicsApiClient`

```c#
[ApiController]
public class RdExampleController
{
  private readonly IRentDynamicsApiClient _rdApiClient;
  private readonly AuthenticationResource _authenticationResource;
  private readonly LeadCardResource _leadCardResource;

  public RdExampleController(IRentDynamicsApiClient rdApiClient)
  {
    _rdApiClient = rdApiClient;
    _authenticationResource = new AuthenticationResource(rdApiClient);
    _leadCardResource = new LeadCardResource(rdApiClient);
  }

  public async Task<LeadCard> PostLeadCard([FromQuery] int communityId, [FromBody] object input)
  {
    await _authenticationResource.LoginAsync("<username>", "<password>");

    var leadCard = new LeadCard(...) { ... };
    return await _leadCardResource.CreateLeadCardAsync(communityId, leadCard);
  }
}
```

## Advanced usage

### Using multiple clients with different permissions
If you have several api-keys with different permissions sets, you will need to take a slightly different approach.

Api keys and user authentication information is persisted inside `IRentDynamicsApiClientSettings` objects. In order to have multiple instances of credentials in the DI container, you can implement custom client interfaces:
```c#
  public interface IMyFirstRentDynamicsApiClient : IRentDynamicsApiClient
  {
  }
  public interface IMySecondRentDynamicsApiClient : IRentDynamicsApiClient
  {
  }
```

Then in your `ConfigureServices` method
```c#
public void ConfigureServices(IServiceCollection services)
{
  ...
  var options1 = new RentDynamicsOptions("<api-key>", "<api-secret-key>", isDevelopment: true);
  services.AddRentDynamicsApiClient<IMyFirstRentDynamicsApiClient, RentDynamicsApiClient>(clientName: "first", options1);

  var options2 = new RentDynamicsOptions("<another-api-key>", "<another-api-secret-key>", isDevelopment: true);
  services.AddRentDynamicsApiClient<IMySecondRentDynamicsApiClient, RentDynamicsApiClient>(clientName: "second", options2);
  ...
}
```

Now in your services/controller you can do:

```c#
public class RdExampleService
{
  public RdExampleService(IMyFirstRentDynamicsApiClient firstRdApiClient,
                          IMySecondRentDynamicsApiClient secondRdApiClient)
  {
    ...
  }
}
```

## Accessing general endpoints (not strongly-typed)
This package does not yet implement support for the full set of available RentDynamics API endpoints. Only a limited set of [Resource](RentDynamics.RdClient/Resources) classes is provided as of now. The list is going to be extended in the future, however, you can still access any API endpoint using the `IRentDynamicsApiClient` interface directly.

```c#
using Newtonsoft.Json.Linq;

public class RdExampleService
{
  private readonly IRentDynamicsApiClient _apiClient;

  public RdExampleService(IRentDynamicsApiClient defaultApiClient)
  {
    _apiClient = defaultApiClient;
  }

  public async Task AccessAddressEndpoint()
  {
    var newAddress = new Dictionary<string, object>
    {
      { "addressLine_1", "123 Main Street" },
      { "city", "LA" },
      { "stateId", 5 },
      { "zip", "95800" },
      { "addressTypeId", 1 }
    };

    var createdAddress = await _apiClient.PostAsync<Dictionary<string, object>, Dictionary<string, object>>("addresses", newAddress);

    string addressId = createdAddress["id"].ToString();

    var getAddress = await _apiClient.GetAsync<Dictionary<string, object>>($"addresses/{addressId}");

    var addressUpdate = new Dictionary<string, object>
    {
      { "city", "New city" }
    };
    var updatedAddress = await _apiClient.PutAsync<Dictionary<string, object>, Dictionary<string, object>>($"addresses/{addressId}", addressUpdate);

    await _apiClient.DeleteAsync($"addresses/{addressId}");
  }

  public async Task GetExamples()
  {
    // A GET request that fetches a record using a primary key will only return a single object
    var getAddress = await _apiClient.GetAsync<Dictionary<string, object>>($"addresses/{addressId}");

    // A GET request that fetches a record using the 'filters' argument will return multiple objects
    var communitiesAsArrayOfDictionaries
      = await _apiClient.GetAsync<Dictionary<string, object>[]>($"communities?filters=website__icontains=Whispering Springs");

    // For troubleshooting and debugging purposes you may want to get the response as a JSON object
    var communitiesAsJToken = await _apiClient.GetAsync<JToken>($"communities?filters=website__icontains=Whispering Springs");
  }
}
```

## Endpoint usage examples
The section covers some common endpoints that are accessible with the help of `Resource` classes. If you do not know how to obtain a `Resource` class, please read [Simple usage](#simple-usage) or [Advanced usage](#advanced-usage) sections first.

### Appointment endpoints

**Required resource class**: [`AppointmentResource`](RentDynamics.RdClient/Resources/Appointment/AppointmentResource.cs)

1. `GetAppointmentTimesAsUtc`
   
   ```c#
   AppointmentResource resource = ...;
   
   int communityGroupId = <communityGroupId>;
   DateTime appointmentDate = DateTime.Today.Add(1);
   
   UtcAppointmentTimesVM utcAppointmentTimes = await resource.GetAppointmentTimesAsUtcAsync(communityGroupId, appointmentDate);
   //OR you can use synchronous version of the method
   UtcAppointmentTimesVM utcAppointmentTimes = resource.GetAppointmentTimesAsUtc(communityGroupId, appointmentDate);
   
   //UtcAppointmentTimesVM is a List<DateTimeOffset> object
   foreach(DateTimeOffset appointmentTime in appointmentTimes)
   {
       appointmentTime.UtcDateTime //do something with the appointment time
   }
    ```
2. `GetAppointmentTimesAsLocal`
    ```c#
   IRentDynamicsApiClient client = ...;
   AppointmentResource resource = new AppointmentResource(client);
   
   int communityGroupId = <communityGroupId>;
   DateTime appointmentDate = DateTime.Today.Add(1);
   
   LocalAppointmentTimesVM localAppointmentTimes = await resource.GetAppointmentTimesAsLocalAsync(communityGroupId, appointmentDate);
   //OR you can use synchronous version of the method
   LocalAppointmentTimesVM localAppointmentTimes = resource.GetAppointmentTimesAsLocal(communityGroupId, appointmentDate);
   
   //CommunityLocalAppointmentTimesVM is a List<DateTime> object
   foreach(DateTime appointmentTime in communityLocalAppointmentTimes)
   {
       //do something with the appointment time
   }
    ```
3. `GetAppointmentDays`
    ```c#
      IRentDynamicsApiClient client = ...;
      AppointmentResource resource = new AppointmentResource(client);
      
      int communityGroupId = <communityGroupId>;
      DateTime startAppointmentDate = DateTime.Today.AddDays(-3);
      DateTime endAppointmentDate = DateTime.Today.AddDays(1);
      
      AppointmentDaysVM appointmentDays = await resource.GetAppointmentDaysAsync(communityGroupId, startAppointmentDate, endAppointmentDate);
      //OR you can use synchronous version of the method
      AppointmentDaysVM appointmentDays = resource.GetAppointmentDaysAsync(communityGroupId, startAppointmentDate, endAppointmentDate);
      
      //AppointmentDaysVM is a List<DateTime> object
      foreach(DateTime appointmentDay in appointmentDays)
      {
          //do something with the appointmentDay
      }
    ```
