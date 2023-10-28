using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Bro
{
    public static class ObjectCopier
    {
        /// <summary>
        /// Perform a deep Copy of the object. Source class must have an Serializable attribute
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <param name="surrogateSelector">Selector for custom binary serialization</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source, SurrogateSelector surrogateSelector)
        {
            if (ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            if (surrogateSelector != null)
            {
                formatter.SurrogateSelector = surrogateSelector;
            }
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }
       
        /// <summary>
        /// Clone @fieldsToClone values from @src to @dst 
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="src"></param>
        /// <param name="fieldsToClone">if null => clone all public instance fields</param>
        /// <typeparam name="T"></typeparam>
        public static void CloneFields<T>(T dst, T src, FieldInfo[] fieldsToClone = null)
        {
            if (fieldsToClone == null)
            {
                fieldsToClone = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            }
            foreach (var field in fieldsToClone)
            {
                field.SetValue(dst, field.GetValue(src));
            }
        }
    }
}