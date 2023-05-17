using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Mockus.Contracts.Request;
using Mockus.Contracts.Response;
using Mockus.WebApi.Options;
using Newtonsoft.Json;

namespace Mockus.WebApi.Contracts;

public class ReadOnlyStorage
{
    public ConcurrentBag<KeyValuePair<MockRequestOptions, MockedResponse>> Container { private set; get; }
    
    public ReadOnlyStorage()
    {
        var kvpFromEnvVar = Environment.GetEnvironmentVariable("PREDEFINED_RESPONSE");
        if (kvpFromEnvVar != null)
        {
            var kvpModel = JsonConvert.DeserializeObject<IEnumerable<RequestResponseDto>>(kvpFromEnvVar);
            var kvp = kvpModel
                .Select(c =>
                {
                    return new KeyValuePair<MockRequestOptions, MockedResponse>(
                        new MockRequestOptions()
                        {
                            RequestIdentifier = new RequestIdentifier(c.RequestPath, c.RequestMethod)
                        },
                        new MockedResponse()
                        {
                            Body = c.ResponseBody,
                            ResponseOptions = new ResponseOptions()
                            {
                                StatusCode = c.ResponseCode
                            }
                        });
                }).ToList();

            Container = new();
            kvp.ForEach(opt => Container.Add(opt));
        }
        else
        {
            Container = new();
        }
    }
    
    public ReadOnlyStorage(IEnumerable<KeyValuePair<MockRequestOptions, MockedResponse>> keyValues)
    {
        Container = new ConcurrentBag<KeyValuePair<MockRequestOptions, MockedResponse>>
            (keyValues);
    }
    
    public ReadOnlyStorage FilterByPathAndMethod(string path, string method)
    {
        return Filter(x => x.Key.RequestIdentifier.Equals(new RequestIdentifier(path, method)));
    }
    
    public ReadOnlyStorage Filter(Func<KeyValuePair<MockRequestOptions, MockedResponse>, bool> func)
    {
        var filteredDict = Container
            .Where(func);

        return ToMockStorage(filteredDict);
    }
    
    private ReadOnlyStorage ToMockStorage(IEnumerable<KeyValuePair<MockRequestOptions, MockedResponse>> keyValue)
    {
        return new ReadOnlyStorage(keyValue);
    }
}