using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Mockus.Contracts.Abstractions.Options;
using Mockus.Contracts.Enums;
using Newtonsoft.Json;

namespace Mockus.Contracts.Options
{
    public class SequenceOption :IOption, IHasValue<List<string>>
    {

        public SequenceOption()
        {
            Value = new List<string>();
            Compare = SequenceCompareOption.ElementsEqual;
            ContentType = ContentType.Json;
        }
       
        public SequenceOption(IEnumerable<string> sequence, SequenceCompareOption comparer) :this()
        {
            Value.AddRange(sequence);
            Compare = comparer;
        }

        public SequenceOption(
            string sequence,
            SequenceCompareOption comparer,
            ContentType contentType)
            : this(sequence.Split(","), comparer)
        {
            ContentType = contentType;
        }

        public SequenceOption(string option) : this()
        {
            Value.Add(option);
        }

        public SequenceOption(IEnumerable<string> option) : this()
        {
            Value.AddRange(option);
        }

        public ContentType ContentType { get; set; }

        public List<string> Value { get; set; }


        public void Validate()
        {
            if (!Value.Any())
                throw new ApplicationException("Sequence shouldnot be empty");
        }

        public SequenceCompareOption Compare { get; set; }

        public bool IsMatch(string other)
        {
            var sequence = GetConvertedCollection(ContentType, other).ToList();

            switch (Compare)
            {
                case SequenceCompareOption.ElementsEqual:
                {
                    return Enumerable.SequenceEqual(Value.OrderBy(t => t), sequence.OrderBy(t => t));
                }

                case SequenceCompareOption.EqualOrder:
                {
                    return Enumerable.SequenceEqual(Value, sequence);
                }

                case SequenceCompareOption.Intersects:
                {
                    return GetIntersectionResult(ContentType, sequence);
                }
            }

            return false;
        }

        private bool GetIntersectionResult(ContentType contentType, IEnumerable<string> sequence)
        {
            switch (contentType)
            {
                case ContentType.Json:
                {
                    return sequence.Intersect(Value).Any();
                }

                case ContentType.CommaSeparated:
                {
                    return sequence.Intersect(Value).Any();
                }

                case ContentType.PlainText:
                {
                    return Value
                        .Select(x => new Regex(x))
                        .Select(r => r.IsMatch(sequence.First()))
                        .Any(x => x);
                }

                default:
                    throw new NotImplementedException(nameof(contentType));
            }
        }

        public override bool Equals(object? obj)
        {
            SequenceOption other = obj as SequenceOption;

            if (other == null)
                return false;

            if (Compare != other.Compare)
                return false;

            if (ContentType != other.ContentType)
                return false;

            return Value.SequenceEqual(other.Value);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash + 23 * Value.GetHashCode();
                return hash;
            }
        }

        public bool IsMatch(object other)
        {
            var stringObj = other as string;
            return IsMatch(stringObj);
        }


        private IEnumerable<string> GetConvertedCollection(ContentType contentType, string rawString)
        {
            switch (contentType)
            {
                case ContentType.Json:
                {
                  return  JsonConvert.DeserializeObject<string[]>(rawString);
                }
                case ContentType.CommaSeparated:
                {
                    return rawString.Split(',');
                }
                case ContentType.PlainText:
                {
                    return new[] { rawString };
                }
                
                default:
                {
                    throw new NotImplementedException($"Unknown {typeof(ContentType)} : {contentType}");
                }
            }
        }
    }
}
