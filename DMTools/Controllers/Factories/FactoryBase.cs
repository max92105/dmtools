using Data.Objects;

namespace Controllers.Factories
{
    public abstract class FactoryBase<T> where T : class
    {
        private T _Obj { get; set; }

        public static implicit operator ObjectBase(FactoryBase<T> obj)
        {
            if (typeof(T).Equals(typeof(ObjectBase)))
            {
                return (ObjectBase)(object)obj._Obj;
            }

            return null;
        }

        public virtual void SaveObject(T obj)
        {
            if (typeof(ObjectBase).IsAssignableFrom(typeof(T)))
            {
                _Obj = obj;
            }
        }
    }
}
