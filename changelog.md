# Lexim MercadoPago SDK - Change Log

## 2.0.4 (Current)
  - Updated "Preapproval" with the lastest API version.
  - Added resource "AuthorizedPayment".
  - Added IPN support to "AuthorizedPayment" and "Preapproval".
  - Now GetAccessToken is thread safe.

## 2.0.3
  - Added "status, date_created, last_modified" to Preapproval.

## 2.0.2
  - Fixed `IpnHandler` incorrectly expecting the payment id to be a valid `int`-parsable string, when it is actually `long`.

## 2.0.1 
  - Moved CI to Azure Pipelines

## 2.0.0

### Async support

  Async counterparts have been added for most effectful methods in most resource classes. `SaveAsync()`, `UpdateAsync()`, `DeleteAsync()`, `FindByIdAsync()`, etc have been added to most resource classes. These are based on the async `HttpClient` API, in contrast to the legacy synchronous methods, which are based on `HttpWebRequest` and `HttpWebResponse`.
  
  > Note: These new methods are available only when using the .NET Standard version of the SDK.

### Test project now targets .NET Framework 4.5 and .NET Core 2.1
  - Most tests are now passing for both platforms.
  - Tests related to `System.Configuration` and stringly-typed configuration of credentials have been ommited for .NET Core. A new configuration API will be made available using .NET Core idioms.

### Dropped support for .NET Framework 3.5
  - Minimum .NET Framework version required has been changed to 4.0. This is due to [.NET Core tooling not supporting .NET 3.5](https://github.com/Microsoft/msbuild/issues/1333).
  - If you are still running .NET Framework 3.5, you may continue to use 1.x versions.

## 1.0.10
  - Merged latest changes from official SDK.

### Improved error handling
  - HTTP status codes in the range `400-500` will no longer result in a silent failure. An exception is now thrown for all non-successful actions performed via the SDK.

## 1.0.9
  - Fixed invalid `Id1` property in `MercadoPago.DataStructures.Payment.Order`. Partial fix for [#6](https://github.com/LeximSoluciones/Lexim-MercadoPago-Sdk/issues/6).

## 1.0.8
  - Merged latest changes from official SDK, including bug fixes related to shipment data.
  - Removed previously deprecated MPIPN class.
  - Reworked code samples.

## 1.0.7
  - Merged [latest changes](https://github.com/mercadopago/dx-dotnet/tree/7c4bc0eb493b00cd69b7dcdf0a1efb7c360b80fa) from official SDK.
  - `PaymentType.Id` is now nullable. Fixes [#4](https://github.com/LeximSoluciones/Lexim-MercadoPago-Sdk/issues/4) 

## 1.0.6

  - All static methods that perform requests against the MercadoPago API (for instance all `FindById()`, `Search()` and `Query()` methods) now have an `accessToken` parameter, which enables the use of that particular access token for that particular request.

## 1.0.5

  - Implemented `SDK.GetAccessToken(string clientId, string clientSecret)` method, which enables the use of `UserAccessToken` for API calls that traditionally use ClientId and ClientSecret.
  See [Usage example](https://github.com/LeximSoluciones/Lexim-MercadoPago-Sdk/blob/master/MercadoPagoExample/Payments/UserAccessTokenExample.cs) for details.
  - A critical bug was found in this version, hence it was unlisted from nuget.org.

## 1.0.4

  - Removed incorrect `[StringLength]` validation attribute from property `int? MercadoPago.DataStructures.Preference.Item.CategoryId`. Fixes https://github.com/LeximSoluciones/Lexim-MercadoPago-Sdk/issues/3

  - Changed all get-only properties from `MercadoPago.Resources.MerchantOrder` to get/private set. Fixes https://github.com/LeximSoluciones/Lexim-MercadoPago-Sdk/issues/2

  - Reworked validation logic in `MercadoPago.Validation.Validator` for more clear exception messages when validation fails in an unexpected way.

## 1.0.3

  - Removed unneeded Nuget reference: Microsoft.Net.Compilers, which caused an incompatibility with .NET 4.0. Fixes https://github.com/LeximSoluciones/Lexim-MercadoPago-Sdk/issues/1

## 1.0.2 

  - Merged lastest changes from official version. Introduces a new `PayerType` value: `anonymous`.
  - Improved error message when TLS 1.2 is not enabled

## 1.0.1

### Bug fixes:

  - Fixed a serialization bug that would cause an "invalid_items" error when creating a Preference for the first time (fixes https://github.com/mercadopago/dx-dotnet/issues/60)

  - Fixed incorrect endpoints in the `MerchantOrder` resource (fixes https://github.com/mercadopago/dx-dotnet/issues/62)

### New feature: Per request Access Token

  - You can now define a request-specific Access Token for each API call. This allows for scenarios where there are multiple merchants sharing a single web application, which therefore cannot rely on static properties in the `SDK` class.

      See [Usage example](https://github.com/LeximSoluciones/Lexim-MercadoPago-Sdk/blob/master/MercadoPagoExample/Payments/UserAccessTokenExample.cs) for details.

### Cleaner API surface:

  - Many methods, classes and properties in the SDK have now been marked `internal`, because they're not supposed to be used in user code. This allows for a much better discoverability and Intellisense experience. Many classes have also been marked `sealed` to better indicate that no inheritance is needed.

### New feature: [LINQ Provider](https://github.com/LeximSoluciones/Lexim-MercadoPago-Sdk/blob/master/Docs/Linq.md)

## 1.0.0 (Initial)

  - Forked from https://github.com/mercadopago/dx-dotnet
