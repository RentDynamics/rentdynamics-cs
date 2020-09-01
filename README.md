# RentDynamics.RdClient

[![Circle CI Badge](https://circleci.com/gh/RentDynamics/rentdynamics-cs/tree/dev.svg?style=shield&circle-token=8ca42b3ae23f8df7f754457b3daae599f716f85c)](https://circleci.com/gh/RentDynamics/rentdynamics-cs)
[![codecov](https://codecov.io/gh/RentDynamics/rentdynamics-cs/branch/dev/graph/badge.svg)](https://codecov.io/gh/RentDynamics/rentdynamics-cs)
[![MIT License](https://img.shields.io/npm/l/rentdynamics.svg)](LICENSE)
[![Nuget](https://img.shields.io/nuget/v/RentDynamics.RdClient?style=flat)](https://www.nuget.org/packages/RentDynamics.RdClient)

C# client package to access RentDynamics API

## Installation

```
Install-Package RentDynamics.RdClient
Install-Package RentDynamics.RdClient.DependencyInjection //For DI container extensions
```

## API credentials and authentication
You need a set of credentials to access API: `apiKey` and `apiSecretKey`.

There are 2 types of API credentials: normal credentials that require user authentication and by-pass user authentication credentials.

As for normal credentials, you will also need to authenticate with a RentDynamics user using `AuthenticationResource` class ([see example below](#console-apps-with-no-di-container)).


## Simple usage

### Console apps with no DI container

```c#
string apiKey = "<api-key>";
string apiSecretKey = "<api-secret-key>";

bool isDevelopment = false; //Chose between production/development RD environment

var options = new RentDynamicsOptions(apiKey, apiSecretKey, isDevelopment: isDevelopment);
var settings = new RentDynamicsApiClientSettings(options);
var rdApiClient = new RentDynamicsApiClient(settings); //Store API client somewhere in a `static` field and reuse it for single-user scenarios

var authenticationResource = new AuthenticationResource(rdApiClient);
await authenticationResource.LoginAsync("<your-username>", "<your-password>"); //Optional step for by-pass user authentication api credentials

var leadCardResource = new LeadCardsResource(rdApiClient);

var leadCard = new LeadCard("Peter", "example@email.com", phoneNumber: null) 
{
    LastName = "Parker"
};

int communityId = "<community-id>";
LeadCard leadCardResult = await leadCardResource.CreateLeadCardAsync(communityId, leadCard);
```

### AspNetCore apps with DI container
```
Install-Package RentDynamics.RdClient.DependencyInjection
```

In your `StartUp.cs` file:

```c#
services.AddDefaultRentDynamicsClient(apiKey, apiSecretKey, isDevelopment: false);
```

It will register `IRentDynamicsApiClient` interface implementation alongside some internal classes and also resource classes.

After that from your service/controller class you can consume `IRentDynamicsApiClient`, `AuthenticationResource` and other available resource classes:

```c#
[ApiController]
public class RdExampleController
{
  private readonly IRentDynamicsApiClient _rdApiClient;
  private readonly AuthenticationResource _authenticationResource;
  private readonly LeadCardsResource _leadCardsResource;

  public RdExampleController(IRentDynamicsApiClient rdApiClient,
                             AuthenticationResource authenticationResource,
                             LeadCardsResource leadCardsResource)
  {
    _rdApiClient = rdApiClient;
    _authenticationResource = authenticationResource;
    _leadCardsResource = leadCardsResource;
  }

  public async Task<LeadCard> PostLeadCard([FromQuery] int communityId, [FromBody] object input)
  {
    await _authenticationResource.LoginAsync("<username>", "<password>");

    var leadCard = new LeadCard(...) { ... };
    return await _leadCardsResource.CreateLeadCardAsync(communityId, leadCard);
  }
}
```

## Advanced usage

### Using multiple clients with different permissions
If you have several api-keys with different permissions sets, you will need to take a slightly different approach.

Api keys and user authentication information is persisted inside `IRentDynamicsApiClientSettings` objects. In order to have multiple instances of credentials in the DI container, you can implement your own settings object:
```c#
//Custom interface implementation
public class CustomRentDynamicsApiClientSettings : RentDynamicsApiClientSettings
{
    public CustomRentDynamicsApiClientSettings(RentDynamicsOptions options) : base(options)
    {
    }
}

public class AnotherCustomRentDynamicsApiClientSettings : RentDynamicsApiClientSettings
{
    public CustomRentDynamicsApiClientSettings(RentDynamicsOptions options) : base(options)
    {
    }
}
```

Then in your `ConfigureServices` method
```c#
public void ConfigureServices(IServiceCollection services)
{
  ...
  var customSettings = new CustomRentDynamicsApiClientSettings(new RentDynamicsOptions("<api-key>", "<api-secret-key>", isDevelopment: true));
  services.AddRentDynamicsApiClient<CustomRentDynamicsApiClientSettings>(customSettings);

  var anotherCustomSettings = new AnotherCustomRentDynamicsApiClientSettings(new RentDynamicsOptions("<another-api-key>", "<another-api-secret-key>", isDevelopment: true));
  services.AddRentDynamicsApiClient<AnotherCustomRentDynamicsApiClientSettings>(anotherCustomSettings);
  ...
}
```

Now in your services/controller you can do:

```c#
public class RdExampleService
{
  public RdExampleService(IRentDynamicsApiClient<CustomRentDynamicsApiClientSettings> customRdApiClient,
                          IRentDynamicsApiClient<AnotherCustomRentDynamicsApiClientSettings> anotherCustomRdApiClient)
  {
    ...
  }
}
```


### Caveats when using multiple clients 
* **Do not** inject `Resource` classes in you services directly when using multiple clients because the resource class will be created with a default `IRentDynamicsApiClient` implementation which can cause unexpected behavior
* **Do** use `IRentDynamicsResourceBySettingsFactory`or`IRentDynamicsResourceByClientFactory` interfaces to create resource classes

#### Which resource factory to use: `IRentDynamicsResourceByClientFactory` vs. `IRentDynamicsResourceBySettingsFactory`

##### `IRentDynamicsResourceBySettingsFactory<TSettings>` example
`IRentDynamicsResourceBySettingsFactory<TSettings>` is a simplified and sometimes a more convenient version of `IRentDynamicsResourceByClientFactory` interface. It requires that  you provide a `TSettings` type (which must implement `IRentDynamicsApiClientSettings` interface) that contains the API credentials you want to use.

The factory then will resolve `IRentDynamicsApiClient<TSettings>` interface implementation from the DI container and pass it to the `Resource` class you are requesting.

**When to use the factory:**
For simple scenarios paired with `AddRentDynamicsApiClient<TClientSettings>(...)` DI container extension method (see example below).

```c#
public class CustomRentDynamicsApiClientSettings : RentDynamicsApiClientSettings
{
    public CustomRentDynamicsApiClientSettings(RentDynamicsOptions options) : base(options)
    {
    }
}

//Inside your Startup.cs file
public void ConfigureServices(IServiceCollection services)
{
    var customSettings = new CustomRentDynamicsApiClientSettings(new RentDynamicsOptions("<api-key>", "<api-secret-key>"));
    services.AddRentDynamicsApiClient<CustomRentDynamicsApiClientSettings>(customSettings); //Register api client implementation that will use credentials from CustomRentDynamicsApiClientSettings class
    
    ... //other code
}

public class ResourceBySettingsFactoryExample
{
    private readonly LeadCardsResource _leadCardsResource;
    
    public ResourceBySettingsFactoryExample(IRentDynamicsResourceBySettingsFactory<CustomRentDynamicsApiClientSettings> resourceBySettingsFactory)
    {
        _leadCardsResource = resourceBySettingsFactory.CreateResource<LeadCardsResource>(); //The resource will be created with IRentDynamicsApiClient<CustomRentDynamicsApiClientSettings> api client implementation
    }
}
```

##### `IRentDynamicsResourceByClientFactory<TClient>` example
This factory is similar to `IRentDynamicsResourceBySettingsFactory<TSettings>`, but is more flexible because it allows using any custom-made api-client implementation, not only the default `IRentDynamicsApiClient`.

**When to use the factory:**
For scenarios when you define your own implementation of `IRentDynamicsApiClient` interface paired with `AddRentDynamicsApiClient<TClient, TClientImplementation, TClientSettings>(...)` DI container extension method (see example below).

```c#
public class CustomRentDynamicsApiClientSettings : RentDynamicsApiClientSettings
{
    public CustomRentDynamicsApiClientSettings(RentDynamicsOptions options) : base(options)
    {
    }
}

public interface ICustomRentDynamicsApiClient : IRentDynamicsApiClient<CustomRentDynamicsApiClientSettings>
{
}

public class CustomRentDynamicsApiClient : RentDynamicsApiClient<CustomRentDynamicsApiClientSettings>, ICustomRentDynamicsApiClient
{
    public CustomRentDynamicsApiClient(HttpClient httpClient, CustomRentDynamicsApiClientSettings settings)
      : base(httpClient, settings)
    {
    }
}

//Inside your Startup.cs file
public void ConfigureServices(IServiceCollection services)
{
    var customSettings = new CustomRentDynamicsApiClientSettings { Options = new RentDynamicsOptions("<api-key>", "<api-secret-key>") };
    services.AddRentDynamicsApiClient<ICustomRentDynamicsApiClient, CustomRentDynamicsApiClient, CustomRentDynamicsApiClientSettings>(customSettings); //Register ICustomRentDynamicsApiClient api client interface with CustomRentDynamicsApiClient as implementation and use credentials from CustomRentDynamicsApiClientSettings class
    
    ... //other code
}

public class ResourceByClientFactoryExample
{
    private readonly LeadCardsResource _leadCardsResource;
    
    public ResourceByClientFactoryExample(IRentDynamicsResourceByClientFactory<ICustomRentDynamicsApiClient> resourceByClientFactory)
    {
        _leadCardsResource = resourceByClientFactory.CreateResource<LeadCardsResource>(); //The resource will be created with ICustomRentDynamicsApiClient api client implementation
    }
}
```

## Accessing not yet implemented endpoints
This package does not yet implement support for the full set of available RentDynamics API endpoints. Only a limited set of [Resource](RentDynamics.RdClient/Resources) classes is provided as of now. The list is going to be extended in the future, however, you can still access any API endpoint using the `IRentDynamicsApiClient` interface directly.

```c#
public class RdExampleAddressService
{
  private readonly IRentDynamicsApiClient _apiClient;

  public RdExampleAddressService(IRentDynamicsApiClient defaultApiClient, // You can use default client
                                 IRentDynamicsApiClient<AnotherCustomRentDynamicsApiClientSettings> anotherCustomRdApiClient // Or any custom client
  )
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
}
```