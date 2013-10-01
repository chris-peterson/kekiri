using System;
using System.Collections.Generic;

namespace Kekiri.IoC
{
    public abstract class DepenencyInjectionContainer
    {
        private readonly List<object> _fakes = new List<object>();
        private bool _registrationClosed;

        protected IEnumerable<object> Fakes
        {
            get { return _fakes; }
        }

        public virtual void WithFake(object fake)
        {
            if (_registrationClosed)
            {
                throw new Exception("Cannot call WithFake after Resolve");
            }

            if (ReferenceEquals(fake, null))
            {
                throw new ArgumentNullException("fake");
            }

            // check for Moq -- we don't want to take on a dependency so we use reflection
            var type = fake.GetType();
            while (type != null)
            {
                if (type.Name == "Mock")
                {
                    var objectProperty = type.GetProperty("Object");
                    if (objectProperty != null)
                    {
                        fake = objectProperty.GetValue(fake, null);
                        break;
                    }
                }
                type = type.BaseType;
            }

            _fakes.Add(fake);
        }

        public void WithFakes(params object[] fakes)
        {
            foreach (var fake in fakes)
            {
                WithFake(fake);
            }
        }

        protected abstract T ResolveImpl<T>();

        public virtual T Resolve<T>()
        {
            _registrationClosed = true;

            return ResolveImpl<T>();
        }
    }
}