using System;
using Mockus.Contracts.Options;

namespace Mockus.Client.Extensions
{
    public static class RequestExtensions
    {
        public static MappedOption<SequenceOption> Upsert(this MappedOption<SequenceOption> bodyOption, Func<MappedOption<SequenceOption>, MappedOption<SequenceOption>> act)
        {
            bodyOption ??= new MappedOption<SequenceOption>();
            bodyOption = act.Invoke(bodyOption);
            return bodyOption;
        }

        public static SequenceOption Upsert(this SequenceOption bodyOption, Func<SequenceOption,SequenceOption> act)
        {
            bodyOption ??= new SequenceOption();
            bodyOption = act.Invoke(bodyOption);
            return bodyOption;
        }

    
    }
}
