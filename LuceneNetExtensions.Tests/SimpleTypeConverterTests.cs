namespace LuceneNetExtensions.Tests
{
    using System;
    using System.Globalization;

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
            Assert.AreEqual(convertedValue, 12);
        }

        [Test]
        public void ConvertIntToString()
        {
            int value = 12;

            var convertedValue = SimpleTypeConverter.ConvertTo<string>(value);

            Assert.AreEqual(convertedValue, "12");
        }

        [Test]
        public void ConvertStringToDecimal()
        {
            string value = 12.53m.ToString(CultureInfo.InvariantCulture);

            var convertedValue = SimpleTypeConverter.ConvertTo<decimal>(value, CultureInfo.InvariantCulture);

            Assert.IsInstanceOf<decimal>(convertedValue);
            Assert.AreEqual(convertedValue, 12.53m);
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
            Assert.AreEqual(convertedValue, new Guid(value));
        }

        [Test]
        public void ConvertGuidToString()
        {
            Guid value = Guid.NewGuid();

            var convertedValue = SimpleTypeConverter.ConvertTo<string>(value);

            Assert.IsInstanceOf<string>(convertedValue);
            Assert.AreEqual(convertedValue, value.ToString());
        }

    }
}
