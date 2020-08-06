using System;
using System.Text;

namespace RentDynamicsCS
{
    internal static class ByteToStringExtensions
    {
        public static string ToHexString(this byte[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            
            var stringBuilder = new StringBuilder(array.Length * 2);
            
            foreach (byte b in array)
            {
                stringBuilder.Append(b.ToString("X2"));
            }

            return stringBuilder.ToString();
        }
    }
}