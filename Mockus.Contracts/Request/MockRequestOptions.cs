using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using Mockus.Contracts.Abstractions.Integration;
using Mockus.Contracts.Abstractions.Options;
using Mockus.Contracts.Options;
using Mockus.Contracts.Enums;

namespace Mockus.Contracts.Request
{
    public class MockRequestOptions : IMockRequestOptions, IMockRequestBuilder
    {
        [Required]
        public RequestIdentifier RequestIdentifier { get; set; }
        public IOption Body { get; set; }
        public MappedOption<StringOption> Headers { get; set; }
        public MappedOption<StringOption> Cookies { get; set; }
        public MappedOption<SequenceOption> QueryParams { get; set; }

        public void Validate()
        {
            RequestIdentifier.Validate();
            Body?.Validate();
            Headers?.Validate();
            Cookies?.Validate();
            QueryParams?.Validate();
        }

        public IMockRequestBuilder WithPathAndMethod(string path, HttpMethod method)
        {
            RequestIdentifier = new RequestIdentifier(path, method);

            return this;
        }

        public IMockRequestBuilder AddRequestIdentification(IdentificationSources source, string name, string value)
        {
            if (RequestIdentifier == null)
            {
                throw new ArgumentNullException(
                    nameof(RequestIdentifier),
                    $"Use {nameof(WithPathAndMethod)} before {nameof(AddRequestIdentification)}");
            }

            RequestIdentifier.CustomIdentification.Add(new RequestIdentificationRule(source, name, value));
            return this;
        }

        public IMockRequestBuilder WithPathAndMethod(RequestIdentifier requestIdentifier)
        {
            RequestIdentifier = requestIdentifier;

            return this;
        }

        public IMockRequestBuilder WithBody(IOption option)
        {
            Body = option;
            return this;
        }

        public IMockRequestBuilder WithHeaders(string key, string value)
        {
            Headers = CreateOrUpdate(Headers, key, new StringOption(value));
            return this;
        }

        public IMockRequestBuilder WithHeaders(string key, StringOption value)
        {
            Headers = CreateOrUpdate(Headers, key, value);
            return this;
        }

        public IMockRequestBuilder WithoutHeader(string key)
        {
            Headers = CreateOrUpdate(Headers, key, null, MappedCompareOption.NotContainsKey);
            return this;
        }


        public IMockRequestBuilder WithHeaders(MappedOption<StringOption> option)
        {
            Headers = option;
            return this;
        }

        public IMockRequestBuilder WithCookies(string key, string value)
        {
            Cookies = CreateOrUpdate(Cookies, key, new StringOption(value));
            return this;
        }

        public IMockRequestBuilder WithCookies(string key, StringOption value)
        {
            Cookies = CreateOrUpdate(Cookies, key, value);
            return this;
        }


        public IMockRequestBuilder WithCookies(MappedOption<StringOption> option)
        {
            Cookies = option;
            return this;
        }

        private MappedOption<TOption> CreateOrUpdate<TOption>(
            MappedOption<TOption> option,
            string key,
            TOption value,
            MappedCompareOption comparer = MappedCompareOption.Default
            ) where TOption:IOption
        {
            if (option == null)
            {
                option = new MappedOption<TOption>(key, value, comparer);
            }
            else
            {
                option.Add(key, value);
            }

            return option;
        }
        

        public IMockRequestBuilder WithQueryParams(string key, string value)
        {
            QueryParams = CreateOrUpdate(QueryParams, key, new SequenceOption(value));
            return this;
        }

        public IMockRequestBuilder WithQueryParams(string key, SequenceOption value)
        {
            QueryParams = CreateOrUpdate(QueryParams, key, value);
            return this;
        }

        public IMockRequestBuilder WithQueryParams(string key, IEnumerable<string> collection)
        {
            QueryParams = CreateOrUpdate(QueryParams, key, new SequenceOption(collection));
            return this;
        }

        public MockRequestOptions Create()
        {
            return this;
        }

        public override bool Equals(object? obj)
        {
            var other = (obj as MockRequestOptions);

            if (other == null)
                return false;

            var isEquals = RequestIdentifier.Equals(other.RequestIdentifier);

            isEquals &= Body == other.Body || Body?.Equals(other.Body) is true;

            isEquals &= Headers == other.Headers || Headers?.Equals(other.Headers) is true;

            isEquals &= QueryParams == other.QueryParams || QueryParams?.Equals(other.QueryParams) is true;

            isEquals &= Cookies == other.Cookies || Cookies?.Equals(other.Cookies) is true;

            return isEquals;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + RequestIdentifier.GetHashCode();
                hash = hash * 23 + (Body == null ? 0 : Body.GetHashCode());
                hash = hash * 23 + (Headers == null ? 0 : Headers.GetHashCode());
                hash = hash * 23 + (QueryParams == null ? 0 : QueryParams.GetHashCode());
                hash = hash * 23 + (Cookies == null ? 0 : Cookies.GetHashCode());
                return hash;
            }
        }

        public override string ToString()
        {
            return RequestIdentifier.ToString();
        }
    }
}