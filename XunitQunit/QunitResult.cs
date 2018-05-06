namespace XunitQunit
{
    public class QunitResult
    {
        /// <summary>
        ///     The number of failed assertions
        /// </summary>
        public int Failed { get; set; }

        /// <summary>
        ///     The number of passed assertions
        /// </summary>
        public int Passed { get; set; }


        /// <summary>
        ///     The total number of assertions
        /// </summary>
        public int Total { get; set; }

        public int Runtime { get; set; }
    }
}