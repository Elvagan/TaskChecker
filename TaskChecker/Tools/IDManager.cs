using System;

namespace TaskChecker.Tools
{
    internal class IDManager
    {
        #region Public methods

        public static string GetNewName(string prefix = "")
        {
            string name = prefix;
            DateTime now = DateTime.Now;

            name += now.Year.ToString("0000") + now.Month.ToString("00") + now.Day.ToString("00");
            name += "_";
            name += now.Hour.ToString("00") + now.Minute.ToString("00") + now.Second.ToString("00");
            name += "_";
            name += now.Millisecond.ToString("000");

            return name;
        }

        #endregion Public methods
    }
}