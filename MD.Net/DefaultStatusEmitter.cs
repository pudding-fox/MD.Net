using System;
using System.Text.RegularExpressions;

namespace MD.Net
{
    public class DefaultStatusEmitter : StatusEmitter
    {
        public const string GROUP_POSITION = "POSITION";

        public const string GROUP_COUNT = "COUNT";

        const int COUNT = 100;

        public DefaultStatusEmitter(string message, StatusType type, Regex regex, IStatus status) : base(message, type, status)
        {
            this.Regex = regex;
            this.Percent = -1;
        }

        public Regex Regex { get; private set; }

        public int Percent { get; private set; }

        protected override void Emit()
        {
            while (this.Percent < COUNT)
            {
                this.Percent++;
                this.Status.Update(this.Message, this.Percent, COUNT, this.Type);
            }
        }

        protected override void Emit(string value)
        {
            var match = this.Regex.Match(value);
            if (!match.Success)
            {
                return;
            }
            var position = default(long);
            if (!long.TryParse(match.Groups[GROUP_POSITION].Value, out position))
            {
                return;
            }
            var count = default(long);
            if (!long.TryParse(match.Groups[GROUP_COUNT].Value, out count))
            {
                return;
            }
            var percent = Convert.ToInt32((Convert.ToDouble(position) / count) * 100);
            while (this.Percent < percent)
            {
                this.Percent++;
                this.Status.Update(this.Message, this.Percent, COUNT, this.Type);
            }
        }
    }
}
