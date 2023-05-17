using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Mockus.Contracts.ExecutionPolicy;
using Mockus.Contracts.Request;
using Mockus.Contracts.Response;
using Mockus.Contracts.Enums;
using Mockus.WebApi.Abstractions.Data;
using Mockus.WebApi.Contracts;
using Mockus.WebApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Serilog;

namespace Mockus.WebApi.Controller
{
    [Route("/api/v1/Manage")]
    [ProducesErrorResponseType(typeof(ErrorMessage))]
    public class ManagerController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IStorageRepository _storageRepository;
        private readonly ICollectedRequestRepository _collectedRequestRepository;
        private readonly ILogger _logger = Log.ForContext<ManagerController>();
        private IExecutionPolicyRepository _executionPolicyRepository;

        public ManagerController(
            IStorageRepository storageRepository,
            ICollectedRequestRepository collectedRequestRepository,
            IExecutionPolicyRepository executionPolicyRepository
        )
        {
            _storageRepository = storageRepository;
            _collectedRequestRepository = collectedRequestRepository;
            _executionPolicyRepository = executionPolicyRepository;
        }

        [HttpPost("SetMockData")]
        [ProducesResponseType(200)]
        public IActionResult SetMockData([FromBody][Required] SetMockDataRequest mockDataRequest)
        {
            ValidateModel(ModelState, nameof(SetMockDataRequest));
            _storageRepository.Add(mockDataRequest);
            return Ok();
        }

        [ProducesResponseType(typeof(MockedDataResponse), 200)]
        [HttpPost("GetMockedData")]
        public IActionResult GetMockedData([FromBody] [Required] RequestIdentifier requestIdentifier)
        {
            var data = _storageRepository.GetMockedData(requestIdentifier).FirstOrDefault();
            _logger
                .Debug(
                    "GetMockedData successfully executed for '{@req}'. Found: '{@data}'",
                    requestIdentifier,
                    data.MockedResponse);
            return Ok(data);
        }


        [ProducesResponseType(typeof(List<MockedDataResponse>), 200)]
        [HttpGet("GetMockedData")]
        public IActionResult GetMockedData()
        {
            var data = _storageRepository.GetAll();
            _logger.Debug($"GetMockedData successfully executed. Mocks found: {data.Count()}");
            return Ok(data);
        }


        [HttpPost("GetMockedResponse")]
        [ProducesResponseType(typeof(MockedResponse), 200)]
        public IActionResult GetMockedResponse([FromBody] RequestDto request)
        {
            ValidateModel(ModelState, nameof(MockRequestOptions));
            
            var mockedData = _storageRepository.GetResponse(request);
            _logger.Debug(
                "GetMockedResponse successfully executed." +
                          "Request: {@req}, Data found: {@data}", mockedData);
            return Ok(mockedData);
        }

        [HttpGet("GetIncomingRequests")]
        [ProducesResponseType(typeof(IEnumerable<CollectedRequest>), 200)]
        public IActionResult GetIncomingRequest()
        {
            var data = _collectedRequestRepository.GetAll();
            _logger.Debug($"GetIncomingRequest successfully executed. Requests found: {data.Count()}");
            return Ok(data);
        }

        [HttpGet("GetIncomingRequests/{body}")]
        [ProducesResponseType(typeof(IEnumerable<CollectedRequest>), 200)]
        public IActionResult GetIncomingRequestByBody(string body)
        {
            var data = _collectedRequestRepository.GetWithBody(body);
            _logger.Debug($"GetIncomingRequest successfully executed. Requests found: {data.Count()}");
            return Ok(data);
        }

        [HttpPost("GetIncomingRequests")]
        [ProducesResponseType(typeof(IEnumerable<CollectedRequest>), 200)]
        public IActionResult GetIncomingRequest([FromBody] RequestIdentifier identifier)
        {
            var data = _collectedRequestRepository.Get(identifier);
            _logger.Debug($"GetIncomingRequests {identifier} successfully executed. Requests found: {data.Count()}");
            return Ok(data);
        }

        [HttpPost("GetIncomingRequestsByPatternBody")]
        [ProducesResponseType(typeof(IEnumerable<CollectedRequest>), 200)]
        public IActionResult GetIncomingRequestByPatternBody([FromBody] RequestBodyPattern identifier)
        {
            var data = _collectedRequestRepository.GetByPatternBody(identifier);
            _logger.Debug($"GetIncomingRequestsByPatternBody {identifier} successfully executed. Requests found: {data.Count()}");
            return Ok(data);
        }

        [HttpGet("ClearMockData")]
        [ProducesResponseType(200)]
        public IActionResult ClearMockData()
        {
            _storageRepository.Clean();
            _logger.Debug("ClearMockData successfully executed");
            return Ok();
        }

        [HttpGet("ClearCollectedRequests")]
        [ProducesResponseType(200)]
        public IActionResult ClearCollectedRequests()
        {
            _collectedRequestRepository.Clean();
            _logger.Debug("ClearCollectedRequests successfully executed");
            return Ok();
        }

        [HttpGet("ClearAll")]
        [ProducesResponseType(200)]
        public IActionResult ClearAll()
        {
            _storageRepository.Clean();
            _collectedRequestRepository.Clean();
            _executionPolicyRepository.Clear();
            _logger.Debug("ClearAll successfully executed");
            return Ok();
        }

        [ProducesResponseType(200)]
        [HttpPost("SetExecutionPolicy")]
        public IActionResult SetExecutionPolicy([FromBody] ExecutionPolicyModel executionPolicy)
        {
            ValidateModel(ModelState,nameof(ExecutionPolicyModel));
            _executionPolicyRepository.Add(executionPolicy.Identifier, executionPolicy.ExecutionPolicy);
            _logger.Debug("SetExecutionPolicy successfully executed");
            return Ok();
        }

        [ProducesResponseType(200)]
        [HttpPost("SetExecutionPolicy/{status}")]
        public IActionResult SetExecutionPolicy([FromBody] RequestIdentifier identifier, PolicyStatus status)
        {
            ValidateModel(ModelState, nameof(RequestIdentifier));
            _executionPolicyRepository.SetStatus(identifier, status);
            _logger.Debug($"SetExecutionPolicy successfully executed for identifier {identifier} and status {status}");
            return Ok();
        }

        [ProducesResponseType(200)]
        [HttpGet("ClearAllExecutionPolicy")]
        public IActionResult ClearExecutionPolicy()
        {
            _executionPolicyRepository.Clear();
            _logger.Debug("ClearAllExecutionPolicy successfully executed");
            return Ok();
        }

        [HttpGet("ready")]
        [ProducesResponseType(200)]
        public IActionResult IsReady()
        {
            return Ok();
        }

        [HttpPost("RemoveMockedRequests")]
        [ProducesResponseType(200)]
        public IActionResult RemoveMockedRequests([FromBody] RequestIdentifier identifier)
        {
            _storageRepository.RemoveMockedRequests(identifier);
            return Ok();
        }

        [HttpPost("RemoveMockedRequestsByPattern")]
        [ProducesResponseType(200)]
        public IActionResult RemoveMockedRequestsByPattern([FromBody] RequestPatternIdentifier requestPattern)
        {
            ValidateModel(ModelState, nameof(RequestPatternIdentifier));
            _storageRepository.RemoveMockedRequestsByPattern(requestPattern);
            return Ok();
        }

        [HttpPost("RemoveMockByMockRequestOptions")]
        [ProducesResponseType(200)]
        public IActionResult RemoveMockByMockRequestOptions([FromBody] MockRequestOptions mockRequest)
        {
            ValidateModel(ModelState, nameof(MockRequestOptions));
            _storageRepository.RemoveMockByMockRequestOptions(mockRequest);
            return Ok();
        }

        private void ValidateModel(ModelStateDictionary modelState, string modelName)
        {
            if (!modelState.IsValid)
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                    .Where(y => y.Count > 0)
                    .SelectMany(e => e).Select(e => e.ErrorMessage)
                    .ToArray();
                throw new InvalidModelException($"Model {modelName} is not valid", errors);
            }
        }
    }
}