using System;
using System.Reflection;
using System.Collections.Generic;

namespace Kekiri.IoC
{
    public abstract class Container
    {
        readonly List<object> _fakes = new List<object>();
        bool _registrationClosed;

        protected IEnumerable<object> Fakes => _fakes;

        public void Register(object instance)
        {
            if (_registrationClosed)
            {
                throw new Exception("Cannot call Register after Resolve");
            }

            if (ReferenceEquals(instance, null))
            {
                throw new ArgumentNullException(nameof(instance));
            }

            // check for Moq -- we don't want to take on a dependency so we use reflection
            var type = instance.GetType();
            while (type != null)
            {
                if (type.Name == "Mock")
                {
                    var objectProperty = type.GetTypeInfo().GetProperty("Object");
                    if (objectProperty != null)
                    {
                        instance = objectProperty.GetValue(instance, null);
                        break;
                    }
                }
                type = type.GetTypeInfo().BaseType;
            }

            _fakes.Add(instance);
        }

        public void Register(params object[] instances)
        {
            foreach (var instance in instances)
            {
                Register(instance);
            }
        }

        protected abstract T OnResolve<T>() where T : class;

        public T Resolve<T>() where T : class
        {
            _registrationClosed = true;

            return OnResolve<T>();
        }
    }
}