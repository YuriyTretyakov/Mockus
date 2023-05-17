namespace Mockus.WebApi.Options;

public class RequestResponseDto
{
    public string RequestPath { get; init; }
    
    public string RequestMethod { get; init; }
    
    public int ResponseCode { get; init; }
    
    public string ResponseBody { get; init; }
}