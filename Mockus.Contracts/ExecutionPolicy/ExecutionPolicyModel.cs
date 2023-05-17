using System.ComponentModel.DataAnnotations;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Request;

namespace Mockus.Contracts.ExecutionPolicy
{
    public class ExecutionPolicyModel
    {
        [Required]
        public RequestIdentifier Identifier { get; set; }
        [Required]
        public IExecutionPolicy ExecutionPolicy { get; set; }
    }
}
