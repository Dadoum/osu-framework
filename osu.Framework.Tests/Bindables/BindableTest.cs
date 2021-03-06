// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using osu.Framework.Bindables;

namespace osu.Framework.Tests.Bindables
{
    [TestFixture]
    public class BindableTest
    {
        /// <summary>
        /// Tests that a value provided in the constructor is used as the default value for the bindable.
        /// </summary>
        [Test]
        public void TestConstructorValueUsedAsDefaultValue()
        {
            Assert.That(new Bindable<int>(10).Default, Is.EqualTo(10));
        }

        /// <summary>
        /// Tests that a value provided in the constructor is used as the initial value for the bindable.
        /// </summary>
        [Test]
        public void TestConstructorValueUsedAsInitialValue()
        {
            Assert.That(new Bindable<int>(10).Value, Is.EqualTo(10));
        }

        [TestCaseSource(nameof(getParsingConversionTests))]
        public void TestParse(Type type, object input, object output)
        {
            IBindable bindable = (IBindable)Activator.CreateInstance(typeof(Bindable<>).MakeGenericType(type), type == typeof(string) ? "" : Activator.CreateInstance(type));

            bindable.Parse(input);
            object value = bindable.GetType().GetProperty(nameof(Bindable<object>.Value), BindingFlags.Public | BindingFlags.Instance)?.GetValue(bindable);

            Assert.That(value, Is.EqualTo(output));
        }

        private static IEnumerable<object[]> getParsingConversionTests()
        {
            var testTypes = new[]
            {
                typeof(bool),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(float),
                typeof(double),
                typeof(short),
                typeof(ushort),
                typeof(byte),
                typeof(sbyte),
                typeof(decimal),
                typeof(string)
            };

            var inputs = new object[]
            {
                1, "1", 1.0, 1.0f, 1L, 1m,
                1.5, "1.5", 1.5f, 1.5m,
                -1, "-1", -1.0, -1.0f, -1L, -1m,
                -1.5, "-1.5", -1.5f, -1.5m,
            };

            foreach (var type in testTypes)
            {
                foreach (var input in inputs)
                {
                    object expectedOutput = null;

                    try
                    {
                        expectedOutput = Convert.ChangeType(input, type);
                    }
                    catch
                    {
                        // Not worried about invalid conversions - they'll never work by the base bindable anyway
                    }

                    if (expectedOutput != null)
                        yield return new[] { type, input, expectedOutput };
                }
            }
        }
    }
}
