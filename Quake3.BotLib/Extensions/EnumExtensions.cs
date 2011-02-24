using System;
using System.Globalization;
using Quake3.BotLib.Properties;

namespace Quake3.BotLib.Extensions
{
    public static class EnumerationExtensions
    {
        /// <summary>
        /// Checks if the value contains the provided type
        /// </summary>
        /// <typeparam name="T">The type of the enum</typeparam>
        /// <param name="type">The enum value to check</param>
        /// <param name="value">The flag value to compare</param>
        /// <returns>True if the enum value contains the specified flag value</returns>
        public static bool Has<T>(this System.Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the enum value is only the provided type
        /// </summary>
        /// <typeparam name="T">The type of the enum</typeparam>
        /// <param name="type">The enum value to check</param>
        /// <param name="value">The flag value to compare</param>
        /// <returns>True if the enum value is only the specified flag value</returns>
        public static bool Is<T>(this System.Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Appends an enum value
        /// </summary>
        /// <typeparam name="T">The type of the enum</typeparam>
        /// <param name="type">The enum value to modify</param>
        /// <param name="value">The flag value to append to the enum value</param>
        /// <returns>A modified enum value that has the value removed</returns>
        public static T Add<T>(this System.Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Messages.EnumAppendError, typeof(T).Name), ex);
            }
        }

        /// <summary>
        /// Completely removes a flag value from an enum value
        /// </summary>
        /// <typeparam name="T">The type of the enum</typeparam>
        /// <param name="type">The enum value to modify</param>
        /// <param name="value">The flag value to remove from the enum value</param>
        /// <returns>A modified enum value that has the value removed</returns>
        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Messages.EnumRemoveError, typeof(T).Name), ex);
            }
        }
    }
}