0.2.0
# Bugfixes
* Unescape whitespace characters in URL prior to nonce calculation
# Features
* Remove `RentDynamicsHttpClientFactory` because it is too difficult to support and it lacks functionality compared to `HttpClientBuilder` available in `Microsoft.Extensions.DependencyInjection`
* Change signature of `NonceCalculator` to accept `TextReader` instead of a `string`.
    *  It can save additional conversion into string: `req stream -> plain string -> json -> nonce` becomes `req stream -> json -> nonce`

