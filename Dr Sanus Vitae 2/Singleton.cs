using System;
using System.Reflection;

namespace SanusVitae
{
    /// <summary>
    /// Абстрактный класс для реализации шаблона "Одиночка"
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> where T : class
    {
        protected Singleton() { }
        private class SingletonFactory<S> where S : class
        {
            private static readonly S instance = (S)typeof(S).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], new ParameterModifier[0]).Invoke(null);
            public static S GetInstance()
            {
                return instance;
            }
        }
        public static T Instance
        {
            get
            {
                return SingletonFactory<T>.GetInstance();
            }
        }
    }
}
