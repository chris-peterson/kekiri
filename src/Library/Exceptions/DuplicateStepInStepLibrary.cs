using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kekiri.Impl;

namespace Kekiri.Exceptions
{
    internal class DuplicateStepInStepLibrary : Exception
    {
        private IStep[] DuplicateStepDefinitions { get; set; }
        private readonly string _message;

        public DuplicateStepInStepLibrary(IEnumerable<IStep> duplicates)
        {
            DuplicateStepDefinitions = duplicates.ToArray();
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendFormat("More than one step is defined in a step library for '{0}'. ", DuplicateStepDefinitions.First().Name);
            var duplicateSources = DuplicateStepDefinitions.Select(s => s.SourceDescription).ToArray();
            messageBuilder.AppendFormat("Duplicate definitions are: {0}", String.Join(",", duplicateSources));
            _message = messageBuilder.ToString();
        }

        public override string Message
        {
            get { return _message; }
        }
    }
}