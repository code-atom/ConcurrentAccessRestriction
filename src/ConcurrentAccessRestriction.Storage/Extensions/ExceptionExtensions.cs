using System;
using System.Collections.Generic;
using System.Text;

namespace ConcurrentAccessRestriction.Storage.Extensions
{
    public static class ExceptionExtensions
    {
        public static void ThrowArgumentException(this object @object, string message)
        {
            if (@object == null)
            {
                throw new ArgumentNullException(message);
            }
        }

        public static void ThrowArgumentException(this string text, string message)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(message);
            }
        }
    }
}
