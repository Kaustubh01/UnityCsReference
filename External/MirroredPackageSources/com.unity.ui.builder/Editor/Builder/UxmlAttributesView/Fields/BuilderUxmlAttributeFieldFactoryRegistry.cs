using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Unity.UI.Builder
{
    /// <summary>
    /// Manages the list of IUxmlAttributeFieldFactories.
    /// </summary>
    internal static class BuilderUxmlAttributeFieldFactoryRegistry
    {
        static List<IBuilderUxmlAttributeFieldFactory> s_Factories;

        /// <summary>
        /// The list of all registered factories.
        /// </summary>
        public static List<IBuilderUxmlAttributeFieldFactory> factories
        {
            get
            {
                if (s_Factories == null)
                {
                    s_Factories = new List<IBuilderUxmlAttributeFieldFactory>();
                    RegisterDefaultFactories();
                }
                return s_Factories;
            }
        }

        static void RegisterDefaultFactories()
        {
            // The order in which factories are added is important because we can have two factories that match a specific
            // attribute. In this case, the factory added after will be selected. This is why the default factory being the less
            // specific of all is added first.
            RegisterFactory(new BuilderDefaultUxmlAttributeFieldFactory());
            RegisterFactory(new BuilderTypedUxmlAttributeFieldFactory<float, FloatField>());
            RegisterFactory(new BuilderTypedUxmlAttributeFieldFactory<double, DoubleField>());
            RegisterFactory(new BuilderTypedUxmlAttributeFieldFactory<long, LongField>());
            RegisterFactory(new BuilderTypedUxmlAttributeFieldFactory<bool, Toggle>());
            RegisterFactory(new BuilderUxmlIntAttributeFieldFactory());
            RegisterFactory(new BuilderUxmlStringAttributeFieldFactory());
            RegisterFactory(new BuilderTypedUxmlAttributeFieldFactory<Color, ColorField>());
            RegisterFactory(new BuilderTypedUxmlAttributeFieldFactory<UnityEngine.Object, ObjectField>());
            RegisterFactory(new BuilderUxmlEnumAttributeFieldFactory());
            RegisterFactory(new BuilderUxmlTypeAttributeFieldFactory());
            RegisterFactory(new BuilderUxmlAssetAttributeFieldFactory());
        }

        /// <summary>
        /// Registers the specified factory of fields to uxml attributes.
        /// </summary>
        /// <param name="factory">The factory to register.</param>
        /// <exception cref="ArgumentException">Thrown if the factory is null or already registered.</exception>
        public static void RegisterFactory(IBuilderUxmlAttributeFieldFactory factory)
        {
            if (factory == null)
                throw new ArgumentException("Cannot add a null factory");

            if (factories.Contains(factory))
                throw new ArgumentException("The factory is already registered");

            factories.Add(factory);
        }

        /// <summary>
        /// Returns the factory that can create a ui field for the specified uxml attribute. 
        /// </summary>
        /// <param name="attributeOwner">An instance created from the uxml element that owns the related xml attribute.</param>
        /// <param name="attributeUxmlOwner">The uxml element that owns the uxml attribute related to field factory to seek.</param>
        /// <param name="attribute">The uxml attribute.</param>
        /// <returns>The factory found.</returns>
        public static IBuilderUxmlAttributeFieldFactory GetFactory(object attributeOwner, UxmlAsset attributeUxmlOwner, UxmlAttributeDescription attribute)
        {
            // Traverse factories from last to first because we can have two factories that match a specific attribute but we want
            // the one with the most specific conditions (override) to win. We ensure this factory is added after the
            // less specific one.
            for (var i = factories.Count - 1; i > 0; i--)
            {
                var factory = factories[i];

                if (factory.CanCreateField(attributeOwner, attributeUxmlOwner, attribute))
                {
                    return factory;
                }
            }
            return null;
        }
    }
}
