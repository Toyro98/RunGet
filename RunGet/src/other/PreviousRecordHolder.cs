using System;

namespace RunGet
{
    public class PreviousRecordHolder
    {
        public int days;
        public string names;

        public DateTime? firstPlaceDate;
        public DateTime? secondPlaceDate;

        public PreviousRecordHolder()
        {
            days = -1;
            names = null;
        }

        public PreviousRecordHolder(DateTime? firstPlaceDate, DateTime? secondPlaceDate)
        {
            this.firstPlaceDate = firstPlaceDate;
            this.secondPlaceDate = secondPlaceDate;
        }
    }
}
