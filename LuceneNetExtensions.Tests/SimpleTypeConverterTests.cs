namespace LuceneNetExtensions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class SimpleTypeConverterTests
    {
        [Test]
        public void ConvertStringToInt()
        {
            const string Value = "12";

            var convertedValue = SimpleTypeConverter.ConvertTo<int>(Value);

            Assert.IsInstanceOf<int>(convertedValue);
            Assert.AreEqual(12, convertedValue);
        }

        [Test]
        public void ConvertIntToString()
        {
            int value = 12;

            var convertedValue = SimpleTypeConverter.ConvertTo<string>(value);

            Assert.AreEqual("12", convertedValue);
        }

        [Test]
        public void ConvertStringToNullableInt()
        {
            const string Value = "12";

            var convertedValue = SimpleTypeConverter.ConvertTo<int?>(Value);

            Assert.IsInstanceOf<int?>(convertedValue);
            Assert.AreEqual(12, convertedValue);
        }

        [Test]
        public void ConvertNullableIntToString()
        {
            int? value = 12;

            var convertedValue = SimpleTypeConverter.ConvertTo<string>(value);

            Assert.AreEqual("12", convertedValue);
        }

        [Test]
        public void ConvertNullStringToNullableInt()
        {
            string value = null;

            var convertedValue = SimpleTypeConverter.ConvertTo<int?>(value);

            Assert.Null(convertedValue);
        }

        [Test]
        public void ConvertNullableIntToNullString()
        {
            int? value = null;

            var convertedValue = SimpleTypeConverter.ConvertTo<string>(value);

            Assert.Null(convertedValue);
        }

        [Test]
        public void ConvertStringToDecimal()
        {
            string value = 12.53m.ToString(CultureInfo.InvariantCulture);

            var convertedValue = SimpleTypeConverter.ConvertTo<decimal>(value, CultureInfo.InvariantCulture);

            Assert.IsInstanceOf<decimal>(convertedValue);
            Assert.AreEqual(12.53m, convertedValue);
        }

        [Test]
        public void ConvertDecimalToString()
        {
            decimal value = 12.53m;

            var convertedValue = SimpleTypeConverter.ConvertTo<string>(value, CultureInfo.InvariantCulture);

            Assert.IsInstanceOf<string>(convertedValue);
            Assert.AreEqual(12.53m.ToString(CultureInfo.InvariantCulture), convertedValue);
        }

        [Test]
        public void ConvertStringToGuid()
        {
            string value = Guid.NewGuid().ToString();

            var convertedValue = SimpleTypeConverter.ConvertTo<Guid>(value);

            Assert.IsInstanceOf<Guid>(convertedValue);
            Assert.AreEqual(new Guid(value), convertedValue);
        }

        [Test]
        public void ConvertGuidToString()
        {
            Guid value = Guid.NewGuid();

            var convertedValue = SimpleTypeConverter.ConvertTo<string>(value);

            Assert.IsInstanceOf<string>(convertedValue);
            Assert.AreEqual(value.ToString(), convertedValue);
        }

        [Test]
        public void ConvertGuidArrayToStringArray()
        {
            Guid[] value = { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            var convertedValue = SimpleTypeConverter.ConvertTo<string[]>(value);

            Assert.IsInstanceOf<string[]>(convertedValue);
            Assert.AreEqual(Array.ConvertAll(value, g => g.ToString()), convertedValue);
        }

        [Test]
        public void ConvertStringArrayToGuidArray()
        {
            string[] value = { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            var convertedValue = SimpleTypeConverter.ConvertTo<Guid[]>(value);

            Assert.IsInstanceOf<Guid[]>(convertedValue);
            Assert.AreEqual(Array.ConvertAll(value, s => new Guid(s)), convertedValue);
        }

        [Test]
        public void ConvertStringArrayToStringHashSet()
        {
            string[] value = { "apa", "bepa", "cepa" };

            var convertedValue = SimpleTypeConverter.ConvertTo<HashSet<string>>(value);

            Assert.IsInstanceOf<HashSet<string>>(convertedValue);
            Assert.AreEqual(value, convertedValue.ToArray());
        }

        [Test]
        public void ConvertStringHashSetToStringArray()
        {
            var value = new HashSet<string> { "apa", "bepa", "cepa" };

            var convertedValue = SimpleTypeConverter.ConvertTo<string[]>(value);

            Assert.IsInstanceOf<string[]>(convertedValue);
            Assert.AreEqual(value.ToArray(), convertedValue);
        }

        [Test]
        public void ConvertStringArrayToGuidHashSet()
        {
            string[] value = { Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            var convertedValue = SimpleTypeConverter.ConvertTo<HashSet<Guid>>(value);

            Assert.IsInstanceOf<HashSet<Guid>>(convertedValue);
            Assert.AreEqual(new HashSet<Guid>(Array.ConvertAll(value, s => new Guid(s))), convertedValue);
        }

        [Test]
        public void ConvertGuidHashSetToStringArray()
        {
            var value = new HashSet<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            var convertedValue = SimpleTypeConverter.ConvertTo<string[]>(value);

            Assert.IsInstanceOf<string[]>(convertedValue);
            Assert.AreEqual(Array.ConvertAll(value.ToArray(), g => g.ToString()), convertedValue);
        }   
    }
}
