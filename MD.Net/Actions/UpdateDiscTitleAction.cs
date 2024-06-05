using MD.Net.Resources;

namespace MD.Net
{
    public class UpdateDiscTitleAction : DiscAction
    {
        public UpdateDiscTitleAction(IDevice device, IDisc currentDisc, IDisc updatedDisc) : base(device, currentDisc, updatedDisc)
        {

        }

        public override string Description
        {
            get
            {
                return string.Format(Strings.UpdateDiscTitleAction_Description, this.UpdatedDisc.Title);
            }
        }

        public override void Apply(IToolManager toolManager, IStatus status)
        {
            var output = default(string);
            var error = default(string);
            var process = toolManager.Start(Tools.NETMDCLI, string.Format("{0} \"{1}\"", Constants.NETMDCLI_SETTITLE, this.UpdatedDisc.Title));
            var code = toolManager.Exec(process, out output, out error);
            if (code != 0)
            {
                toolManager.Throw(process, error);
            }
        }

        public override void Commit()
        {
            this.CurrentDisc.Title = this.UpdatedDisc.Title;
            base.Commit();
        }
    }
}
