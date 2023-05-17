using System;
using System.Diagnostics;

namespace Mockus.Contracts.Abstractions.Options
{
    public interface IOption
    {
        void Validate();
        bool IsMatch(object other);
    }
}