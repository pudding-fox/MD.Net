using System.Text.RegularExpressions;

namespace MD.Net
{
    public class PercentStatusEmitter : StatusEmitter
    {
        const int COUNT = 100;

        public PercentStatusEmitter(string message, StatusType type, Regex regex, IStatus status) : base(message, type, status)
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
            var percent = default(int);
            if (!int.TryParse(match.Groups[1].Value, out percent))
            {
                return;
            }
            while (this.Percent < percent)
            {
                this.Percent++;
                this.Status.Update(this.Message, this.Percent, COUNT, this.Type);
            }
        }
    }
}
