namespace RunGetTests
{
    public class TimeTest
    {
        [Theory]
        [InlineData(-13, "")]
        [InlineData(0, "")]
        [InlineData(0.32, "320ms")]
        [InlineData(16.1, "16s 100ms")]
        [InlineData(24, "24s")]
        [InlineData(64.001, "1m 4s 001ms")]
        [InlineData(5520, "1h 32m")]
        [InlineData(18004.321, "5h 4s 321ms")]
        [InlineData(35999999.999, "9999h 59m 59s 999ms")]
        [InlineData(172432714, "")]
        public void FormatTimeTest(decimal time, string expected)
        {
            var result = Time.FormatTime(time);

            if (result != expected)
            {
                throw new Exception($"\"{expected}\" was expected but got \"{result}\" instead!");
            }
        }
    }
}