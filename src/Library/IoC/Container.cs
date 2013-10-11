using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kekiri.IoC
{
    public abstract class Container : IHideObjectMembers
    {
        private readonly List<object> _fakes = new List<object>();
        private bool _registrationClosed;

        protected IEnumerable<object> Fakes
        {
            get { return _fakes; }
        }

        public void Register(object instance)
        {
            if (_registrationClosed)
            {
                throw new Exception("Cannot call Register after Resolve");
            }

            if (ReferenceEquals(instance, null))
            {
                throw new ArgumentNullException("instance");
            }

            // check for Moq -- we don't want to take on a dependency so we use reflection
            var type = instance.GetType();
            while (type != null)
            {
                if (type.Name == "Mock")
                {
                    var objectProperty = type.GetProperty("Object");
                    if (objectProperty != null)
                    {
                        instance = objectProperty.GetValue(instance, null);
                        break;
                    }
                }
                type = type.BaseType;
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

        protected abstract T OnResolve<T>();

        public T Resolve<T>()
        {
            _registrationClosed = true;

            return OnResolve<T>();
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IHideObjectMembers
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        Type GetType();

        [EditorBrowsable(EditorBrowsableState.Never)]
        int GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        string ToString();

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool Equals(object other);
    }
}