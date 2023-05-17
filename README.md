# mockus

In order you want to mock your dependencies for integration testing purpose you can use this stuff.

It consist of:
  - backend part distributed as a docker container. Use `docker pull yuriitretiakov/mockus` and start container.
  - client part that is distributed as nuget package (not published yet)
  

## Usage

```
var mockus = new MockusClient("YOUR_API_URL");
var mockDataCommand = new MockData()
                .WhenRequest(new MockRequestOptions()
                    .WithPathAndMethod("/api/account/getAccountInfo", HttpMethod.Get)
                    .WithHeaders("Authorization", authorization))
                .ThenRespond(new MockedResponse()
                    .WithJsonBody(userInfo)
                    .WithStatusCode(expectedStatusCode))
                .Create();

            await mockus.SetMockDataAsync(mockDataCommand);
```


## Additional Information


If you need to set predefined responses (such as HealthChecks) - you have to do it this way:
In compose file set proper version for Mockus image and set PREDEFINED_RESPONSE env var this way:

```
mockus:
 container_name:mockus
 image: mockus:latest
 hostname: mockus
 environment: 
  - 'PREDEFINED_RESPONSE=
  [{ 
  	requestPath:"/_hc",
  	requestMethod:"GET",
  	responseCode: 200,
  	responseBody: "Healthy"
  }, 
  {
  	requestPath: "/health",
  	requestMethod: "GET",
  	responseCode: 200,
  	responseBody: "OK!" 
  }]'
```
