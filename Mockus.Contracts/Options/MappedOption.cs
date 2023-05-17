using System;
using System.Collections.Generic;
using System.Linq;
using Mockus.Contracts.Abstractions.Options;
using Mockus.Contracts.Enums;

namespace Mockus.Contracts.Options
{
    public class MappedOption<TOptionType> : IOption, IHasValue<Dictionary<string, TOptionType>> where TOptionType:IOption
    {
        public MappedOption()
        {
            Value = new Dictionary<string, TOptionType>();
            Comparer = MappedCompareOption.Default;
        }

        public MappedCompareOption Comparer { get; set; }

        public TOptionType Upsert(string optionName)
        {
            if (!Value.ContainsKey(optionName))
            {
                Value.Add(optionName, Activator.CreateInstance<TOptionType>());
            }

            return Value[optionName];
        }

        public  Dictionary<string, TOptionType> Value { get; set; }

        public MappedOption(string key, TOptionType valueOption)
        {
            Comparer = MappedCompareOption.Default;
            Value = new Dictionary<string, TOptionType>
            {
                {key, valueOption}
            };
        }

        public MappedOption(string key, TOptionType optionValue, MappedCompareOption compareOption)
        {
            Comparer = compareOption;
            Value = new Dictionary<string, TOptionType>
            {
                {key, optionValue}
            };
        }

        public void Add(string keyOption, TOptionType valueOption)
        {
            Value.Add(keyOption, valueOption);
        }


        public void Validate()
        {
            foreach (var (key, value) in Value)
            {
                value.Validate();
            }
        }

        public bool IsMatch(object other)
        {
            var dictObj = (other as IDictionary<string, string>);
            return IsMatch(dictObj);
        }

        public bool IsMatch(IDictionary<string, string> other)
        {
            switch (Comparer)
            {
                case MappedCompareOption.NotContainsKey:
                    return !other.ContainsKey(Value.Keys.First());
                case MappedCompareOption.Default:
                {
                    foreach (var kvMocked in Value)
                    {
                        if (!other.ContainsKey(kvMocked.Key))
                        {
                            return false;
                        }

                        if (!kvMocked.Value.IsMatch(other[kvMocked.Key]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
                default:
                    throw new NotImplementedException(nameof(Comparer));
            }
        }

        public override bool Equals(object? obj)
        {
            var other = obj as MappedOption<TOptionType>;

            if (other == null)
                return false;

            if (Comparer != other.Comparer)
                return false;

            if (Value.Count == other.Value.Count && Value.Keys.SequenceEqual(other.Value.Keys))
            {
                return Value.All(kv => kv.Value.Equals(other.Value[kv.Key]));
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = 17;
            unchecked
            {
                int hashKeys = 0;

                foreach (var key in Value.Keys)
                {
                    var hashCode = key == null? 0 : key.GetHashCode();
                    hashKeys = hash * 23 + hashCode;
                }

                int hashValues = 0;

                foreach (var value in Value.Values)
                {
                    var hashCode = value == null ? 0 : value.GetHashCode();
                    hashValues = hash * 23 + hashCode;
                }
                
                hash = hash * 23 + Value.Values.GetHashCode();
                return hashKeys + hashValues;
            }
        }
    }
}