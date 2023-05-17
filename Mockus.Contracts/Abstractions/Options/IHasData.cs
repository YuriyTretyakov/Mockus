using System;

namespace Mockus.Contracts.Abstractions.Options
{
    public interface IHasValue<TDataType>
    {
        TDataType Value { get; set; }

       
    }
}
