using System;

namespace CodeGeneratorTests
{
    public class ClassWithEvents
    {
        // These should get extension methods
        public event EventHandler<object> ObjEvent;
        public event Action ActionEvent;

        public delegate void LocalDelegate(string fooStuff);
        public event LocalDelegate LocalDelegateEvent;

        // These should not get extension methods
        private event EventHandler<object> PrivateEvent;
        protected event EventHandler<object> ProtectedEvent;
    }
}