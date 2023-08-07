using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiTestConsole
{
    internal class Helpers
    {

        internal static void PrintObjectProperties(object obj)
        {
            // Get the type of the object
            Type type = obj.GetType();

            // Get all public properties of the object
            PropertyInfo[] properties = type.GetProperties();

            // Iterate through each property and print its name and value
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj); // Get the value of the property

                if (value != null)
                {
                    if (property.PropertyType.IsPrimitive)
                    {
                        Console.WriteLine($"{property.Name}: {value}");
                    }
                    else if (property.PropertyType == typeof(string))
                    {
                        Console.WriteLine($"{property.Name}: {value}");
                    }
                    else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                    {
                        Console.WriteLine($"Property: {property.Name} - Type: {property.PropertyType.Name}");
                        // Cast the value to IEnumerable and recursively print elements' properties
                        foreach (var item in (IEnumerable)value)
                        {
                            PrintObjectProperties(item);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Property: {property.Name} - Type: {property.PropertyType.Name}");
                        PrintObjectProperties(value);
                    }
                }
            }
        }

    }
}
