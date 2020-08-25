# RentDynamics.RdClient

[![Circle CI Badge][circleci-badge]][circleci-link]
[![codecov](https://codecov.io/gh/RentDynamics/rentdynamics-cs/branch/dev/graph/badge.svg)](https://codecov.io/gh/RentDynamics/rentdynamics-cs)
[![MIT License](https://img.shields.io/npm/l/rentdynamics.svg)](LICENSE)

C# client package to access RentDynamics API

## Installation

```
Install-Package RentDynamics.RdClient
Install-Package RentDynamics.RdClient.DependencyInjection //For DI container extensions
```

## Simple usage

### Console apps with no DI container
You need a set of credentials to access API: `apiKey` and `apiSecretKey`.

```c#
string apiKey = "<your api key>";
string apiSecretKey = "<your api secret>";

bool isDevelopment = false; //Chose between production/development RD environment

var options = new RentDynamicsOptions(apiKey, apiSecretKey, isDevelopment: isDevelopment);
var settings = new RentDynamicsApiClientSettings { Options = options };
var rdApiClient = new RentDynamicsApiClient(settings); //Store API client somewhere in a `static` field and reuse it for single-user scenarios

var authenticationResource = new AuthenticationResource(rdApiClient);
await authenticationResource.LoginAsync("<your-username>", "<your-password>");

var leadCardResource = new LeadCardsResource(rdApiClient);

var leadCard = new LeadCard
{
    Email = "example@email.com",
    FirstName = "Peter",
    LastName = "Parker"
};

int communityId = "<your-community-id >";
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
      await _authenticationResource.LoginAsync("login", "password");

      var leadCard = new LeadCard { ... };
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
}

public class AnotherCustomRentDynamicsApiClientSettings : RentDynamicsApiClientSettings
{
}
```

Then in your `ConfigureServices` method
```c#
    public void ConfigureServices(IServiceCollection services)
    {
     ...
          var customSettings = new CustomRentDynamicsApiClientSettings { Options = new RentDynamicsOptions("<your-api-key>", "<your-api-secret-key>", isDevelopment: true) };
          services.AddRentDynamicsApiClient<CustomRentDynamicsApiClientSettings>(customSettings);

          var anotherCustomSettings = new AnotherCustomRentDynamicsApiClientSettings { Options = new RentDynamicsOptions("<your-another-api-key>", "<your-another-api-secret-key>", isDevelopment: true) };
          services.AddRentDynamicsApiClient<AnotherCustomRentDynamicsApiClientSettings>(anotherCustomSettings);
     ...
    }
```

Now in your services/controller you can do:

```c#
  [ApiController]
  public class RdExampleController
  {
    public RdExampleController(IRentDynamicsApiClient<CustomRentDynamicsApiClientSettings> customRdApiClient,
                               IRentDynamicsApiClient<AnotherCustomRentDynamicsApiClientSettings> anotherCustomRdApiClient)
    {
      ...
    }
  }
```





 
[circleci-badge]: https://circleci.com/gh/RentDynamics/rentdynamics-cs/tree/dev.svg?style=shield&circle-token=8ca42b3ae23f8df7f754457b3daae599f716f85c
[circleci-link]: https://circleci.com/gh/RentDynamics/rentdynamics-cs
[license-image]: https://img.shields.io/npm/l/rentdynamics.svg