# mockus

In order you want to mock your dependencies for integration testing purpose you can use this stuff.

It consist of:
  backedn part distributed as a docker container: ```docker pull yuriitretiakov/mockus```
  and client part that is distributed as nuget package
  

## Usage



## Additional Information


If you need to se predefined responses (such as HealthChecks) - you need to do it this way:
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
