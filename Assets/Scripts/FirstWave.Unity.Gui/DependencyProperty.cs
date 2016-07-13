using System;
using FirstWave.Unity.Core.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace FirstWave.Unity.Gui
{
    public class DependencyProperty
    {
        public static DependencyProperty Register(string name, Type propertyType, Type ownerType)
        {
            return Register(name, propertyType, ownerType, new PropertyMetadata(propertyType.Default()));
        }

        public static DependencyProperty Register(string name, Type propertyType, Type ownerType, PropertyMetadata metadata)
        {
            // Add a new list if this is the first time we've registered a property for this type
            if (!registeredPropertiesByOwner.ContainsKey(ownerType))
                registeredPropertiesByOwner.Add(ownerType, new List<DependencyProperty>());

            var existingList = registeredPropertiesByOwner[ownerType];

            // Now let's check to see if we've already registered a property with this type
            var existingProperty = existingList.FirstOrDefault(dp => dp.Name == name);
            if (existingProperty != null)
                throw new ArgumentException(string.Format("Dependency Property already registered with name {0}", name));

            var newProperty = new DependencyProperty(name, propertyType, ownerType, metadata);

            existingList.Add(newProperty);

            return newProperty;
        }

        internal static IDictionary<Type, List<DependencyProperty>> registeredPropertiesByOwner;

        static DependencyProperty()
        {
            registeredPropertiesByOwner = new Dictionary<Type, List<DependencyProperty>>();
        }

        public string Name { get; private set; }
        public Type PropType { get; private set; }
        public Type OwnerType { get; private set; }
        public PropertyMetadata Metadata { get; private set; }

        private DependencyProperty(string name, Type propType, Type ownerType, PropertyMetadata metadata)
        {
            Name = name;
            PropType = propType;
            OwnerType = ownerType;
            Metadata = metadata;
        }
    }
}
