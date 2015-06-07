using System;
using System.ComponentModel;

namespace MyToolkit.Tests.WinRT.Mocks
{
    public class MockEventTargetClass
    {
        public Action Action { get; set; }

        public MockEventTargetClass(Action action)
        {
            Action = action; 
        }

        public void Callback(object sender, PropertyChangedEventArgs args)
        {
            Action();
        }
    }
}